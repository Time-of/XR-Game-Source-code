using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EEnemyState
{
	IDLE_MOVE, ATTACK
}



/**
 * @author 이성수
 * @brief 적 캐릭터 베이스입니다. 태그를 반드시 Hostile로 설정할 것 !!!
 * @since 2023-03-20
 */
[RequireComponent(typeof(NavMeshAgent), typeof(StatBase), typeof(Rigidbody))]
public class EnemyCharacterBase : PoolableObject, IDamageable
{
	private static WaitForSeconds AIFindWaitForSeconds;

	protected NavMeshAgent NavAIComp;

    protected Animator AnimComp;

	[SerializeField]
	protected bool bUseWalkAnim = false;

    [SerializeField, ReadOnlyProperty]
	protected Transform CurrentAITarget;

	private static WaitForSeconds AICheckStateWaitForSeconds;

	protected StatBase StatComp;

	[SerializeField, ReadOnlyProperty]
	protected bool bIsDead = false;

	[SerializeField, ReadOnlyProperty]
	protected EEnemyState CurrentState;

	[SerializeField]
	private float CheckAttackDistance = 1.0f;

	[SerializeField, Tooltip("공격에서 IDLE_MOVE로 가는 데 걸리는 시간")]
	private float ChangeStateAttackToIdleDelay = 3.0f;

	[SerializeField]
	private PoolableParticle DeathParticle;

	[SerializeField]
	private AudioClip DeathSfx;

	private GameObject LastDamageCauserRef;

	public UnityEvent OnEnemyDiedEvent = new();



	public EnemyCharacterBase() : base()
	{
		InternalObjectType = EObjectType.CHARACTER;

		AIFindWaitForSeconds = new WaitForSeconds(1.1f);
		AICheckStateWaitForSeconds = new WaitForSeconds(0.5f);
	}



	protected virtual void Awake()
	{
		NavAIComp = GetComponent<NavMeshAgent>();
		StatComp = GetComponent<StatBase>();
        AnimComp = GetComponent<Animator>();
    }



	protected void Start()
	{
		GetComponent<Rigidbody>().drag = 100.0f;
		GetComponent<Rigidbody>().angularDrag = 100.0f;
	}



	private void Update()
	{
		if (bUseWalkAnim)
		{
			AnimComp.SetFloat("WalkSpeed", NavAIComp.velocity.magnitude);
		}
	}



	private void OnCollisionEnter(Collision collision)
	{
		// 그랩 물체를 맞은 경우
		if (collision.gameObject.CompareTag("XRGrabThrowable"))
		{
			float Velocity = collision.rigidbody.velocity.magnitude;

			if (Velocity > 1.3f)
			{
				TakeDamage(Mathf.Clamp(Velocity, 0.0f, 50.0f), null, collision.gameObject);
			}
		}
	}



	#region 오버라이드 기능
	public override void OnPreSpawnedFromPool()
	{
		base.OnPreSpawnedFromPool();

		float[] StatInfo = ActorStatCache.instance.CharacterStatDictionary[InternalName];
		StatComp.InitializeInitStats(StatInfo[0], 0.0f, StatInfo[1], StatInfo[2], StatInfo[3]);
		StatComp.InitializeStats(GameManager.instance.Difficulty);
		bIsDead = false;
		CurrentState = EEnemyState.IDLE_MOVE;

		OnAttackSpeedChanged(StatComp.GetAttackSpeed());
		OnMoveSpeedChanged(StatComp.GetMoveSpeed());
	}



	public override void OnPostSpawnedFromPool()
	{
		StatComp.HealthChangedEvent.AddListener(OnHealthChanged);
		StatComp.AttackSpeedChangedEvent.AddListener(OnAttackSpeedChanged);
		StatComp.MoveSpeedChangedEvent.AddListener(OnMoveSpeedChanged);

		ChangeState(EEnemyState.IDLE_MOVE);

		NavAIComp.SetDestination(transform.position);
		NavAIComp.isStopped = false;

		CurrentAITarget = GameplayHelperLibrary.GetPlayer().transform;
		NavAIComp.SetDestination(CurrentAITarget.position);

		StartCoroutine(RefreshDestinationCoroutine());
		StartCoroutine(CheckStateCoroutine());
	}



	public override void OnReturnedToPool()
	{
		CurrentAITarget = null;

		NavAIComp.SetDestination(transform.position);
		NavAIComp.isStopped = true;

		StatComp.HealthChangedEvent.RemoveListener(OnHealthChanged);
		StatComp.AttackSpeedChangedEvent.RemoveListener(OnAttackSpeedChanged);
		StatComp.MoveSpeedChangedEvent.RemoveListener(OnMoveSpeedChanged);
	}



	public void TakeDamage(float Damage, GameObject Instigator, GameObject DamageCauser)
	{
		LastDamageCauserRef = DamageCauser;

		float CalculatedDamage = GameplayHelperLibrary.CalculateDefense(Damage, StatComp);
		StatComp.SetHealth(StatComp.GetHealth() - CalculatedDamage);
	}
	#endregion



	#region 스탯 관련 기능
	protected void OnHealthChanged(float NewHealth)
	{
		if (!bIsDead && NewHealth <= 0.0f)
		{
			bIsDead = true;

			GameManager.instance.AddScore((uint)StatComp.GetMaxHealth());
			GameManager.instance.AddMoney(1);

			if (DeathParticle != null)
			{
				GameplayHelperLibrary.SpawnParticle(DeathParticle.InternalName,
					transform.position,
					LastDamageCauserRef ? Quaternion.LookRotation((LastDamageCauserRef.transform.position - transform.position).normalized, transform.up) : transform.rotation);
			}

			SoundHelperLibrary.SpawnSoundAtLocation(transform.position, Quaternion.identity, DeathSfx, false);

			OnDied();
		}
		else
		{
			AnimComp?.SetTrigger("Hit");
		}
	}



	protected virtual void OnAttackSpeedChanged(float NewAttackSpeed)
	{

	}



	protected void OnMoveSpeedChanged(float NewMoveSpeed)
	{
		NavAIComp.speed = NewMoveSpeed;
	}
	#endregion



	#region 재정의 가능
	protected virtual void OnDied()
	{
		OnEnemyDiedEvent.Invoke();
		ReturnToPool();
	}



	protected virtual void ExecuteAttack()
	{
		
	}
	#endregion



	#region AI 관련 기능
	private IEnumerator RefreshDestinationCoroutine()
	{
		while (!bIsDead)
		{
			yield return AIFindWaitForSeconds;

			NavAIComp.SetDestination(CurrentAITarget?.transform.position ?? transform.position);
		}
	}



	private IEnumerator CheckStateCoroutine()
	{
		while (!bIsDead)
		{
			yield return AICheckStateWaitForSeconds;

			if (CurrentAITarget != null && CurrentState != EEnemyState.ATTACK)
			{
				float DistSqr = Vector3.SqrMagnitude(CurrentAITarget.transform.position - transform.position);

				if (DistSqr <= CheckAttackDistance * CheckAttackDistance)
				{
					ChangeState(EEnemyState.ATTACK);
				}
			}
		}
	}
	#endregion



	#region 상태 관련 기능
	protected void ChangeState(EEnemyState NewState)
	{
		if (CurrentState == NewState) return;

		// 상태 퇴장
		switch (CurrentState)
		{
			case EEnemyState.IDLE_MOVE:
				break;
			case EEnemyState.ATTACK:
				break;
			default:
				break;
		}

		// 상태 변경
		CurrentState = NewState;

		// 상태 진입
		switch (CurrentState)
		{
			case EEnemyState.IDLE_MOVE:
				NavAIComp.isStopped = false;
				break;
			case EEnemyState.ATTACK:
				NavAIComp.isStopped = true;
				NavAIComp.velocity = Vector3.zero;
				ExecuteAttack();
				StartCoroutine(ReturnToIdleMoveCoroutine());
				break;
			default:
				break;
		}
	}



	private IEnumerator ReturnToIdleMoveCoroutine()
	{
		yield return new WaitForSeconds(ChangeStateAttackToIdleDelay + UnityEngine.Random.Range(0.0f, 0.5f));

		ChangeState(EEnemyState.IDLE_MOVE);
	}
	#endregion
}
