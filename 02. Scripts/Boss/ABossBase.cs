using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
//using Photon.Pun;

//using DamageFramework;
using BehaviourTree;
using UnityEngine.Events;

/**
 * (이전에 했던 프로젝트에서 가져옴)
 * 작성자: 20181220 이성수
 * 보스 몬스터의 베이스입니다.
 */
public abstract class ABossBase : PoolableObject, IDamageable
{
	[Header("보스 몬스터 관련 내용")]

	[SerializeField, ReadOnlyProperty]
	protected bool bIsDead = false;

	[SerializeField]
	protected bool bCanBeDamaged = true;

	[SerializeField, ReadOnlyProperty]
	protected int LevelDifficulty = 1;

	[SerializeField, ReadOnlyProperty]
	protected bool bIsInAction = false;

	protected StatBase StatComp;

	[SerializeField]
	protected AudioClip DeathSfx;

	[SerializeField]
	protected AudioClip OnParriedSfx;

	public UnityEvent OnBossDiedEvent = new();

	[SerializeField]
	protected AudioClip BossBGM;



	[Header("회전")]

	[SerializeField]
	private float RotationSpeed = 5.0f;

	[SerializeField, ReadOnlyProperty]
	private Quaternion TargetRotation;

	[SerializeField, ReadOnlyProperty]
	protected bool bNeedToFixRotation = false;

	public void SetTargetRotation(Quaternion NewRotation) { TargetRotation = NewRotation; bNeedToFixRotation = true; }



	[Header("AI")]

	[SerializeField, ReadOnlyProperty]
	protected BT_ATree RunningTree;



	[Header("HUD")]

	[SerializeField]
	private Image Healthbar;

	[SerializeField]
	private TMPro.TMP_Text HealthText;



	[Header("컴포넌트")]

	protected ABossAnimationBase AnimComponent;

	protected NavMeshAgent NavAgentComponent;

	[SerializeField]
	protected BossWeaponCollision WeaponCollision;



	public ABossBase() : base()
	{
		InternalObjectName = "BOSS_DEFAULT_NONE";
		InternalObjectType = EObjectType.CHARACTER;
	}



	protected virtual void Awake()
	{
		RunningTree = GetComponent<BT_ATree>();

		StatComp = GetComponent<StatBase>();

		AnimComponent = GetComponentInChildren<ABossAnimationBase>();

		NavAgentComponent = GetComponent<NavMeshAgent>();

		if (WeaponCollision == null)
		{
			WeaponCollision = GetComponentInChildren<BossWeaponCollision>();
		}
	}



	void Update()
	{
		if (bNeedToFixRotation)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation,
				TargetRotation, Time.deltaTime * RotationSpeed);

			if (MathHelperLibrary.AlmostEquals(transform.rotation, TargetRotation, 0.35f))
			{
				bNeedToFixRotation = false;
			}
		}
	}



	#region 피해 관련 기능
	void IDamageable.TakeDamage(float Damage, UnityEngine.GameObject Instigator, UnityEngine.GameObject DamageCauser)
	{
		if (bIsDead || !bCanBeDamaged) return;

		float CalculatedDamage = GameplayHelperLibrary.CalculateDefense(Damage, StatComp);
		StatComp.SetHealth(StatComp.GetHealth() - CalculatedDamage);
	}



	void OnDied()
	{
		SetCanBeParried(false);
		bCanBeDamaged = false;
		bIsDead = true;

		NavAgentComponent.isStopped = true;

		GetComponent<Collider>().enabled = false;

		GameManager.instance.ChangeBGMToDefault();

		bIsInAction = true;
		RunningTree.SetData("IsInAction", bIsInAction);
		AnimComponent.GetAnimator().SetTrigger("Die");

		StatComp.SetMoveSpeed(0.0f);
		NavAgentComponent.SetDestination(transform.position);
		NavAgentComponent.isStopped = true;

		GameManager.instance.Victory();
	}
	#endregion



	#region 공격 관련
	public virtual void TryAttack(int AttackType)
	{
		if (bIsDead) return;

		// 자식 클래스에서 구현
	}



	public virtual void OnBeginAction()
	{
		bIsInAction = true;
		RunningTree.SetData("IsInAction", bIsInAction);

		NavAgentComponent.velocity = Vector3.zero;
		NavAgentComponent.isStopped = true;
	}



	public virtual void OnEndAction()
	{
		if (Random.Range(0.0f, 1.0f) <= 0.15f)
		{
			InternalEndAction();
		}
		else
		{
			StopCoroutine(RandomWaitForEndActionCoroutine());
			RunningTree.SetData("bIsWaiting", false);
			StartCoroutine(RandomWaitForEndActionCoroutine());
		}
	}



	private IEnumerator RandomWaitForEndActionCoroutine()
	{
		RunningTree.SetData("bIsWaiting", true);

		if (NavAgentComponent.isStopped)
		{
			NavAgentComponent.updateRotation = false;
			TryLookAtPlayer(true);
			NavAgentComponent.isStopped = false;
			NavAgentComponent.SetDestination(transform.position + Quaternion.AngleAxis(Random.Range(-360.0f, 360.0f), Vector3.up) * (Vector3.forward * 5.0f));
		}


		yield return new WaitForSeconds(Random.Range(1.0f, 2.3f));

		InternalEndAction();
		RunningTree.SetData("bIsWaiting", false);
	}



	private void InternalEndAction()
	{
		bIsInAction = false;
		RunningTree.SetData("IsInAction", bIsInAction);

		NavAgentComponent.isStopped = false;
	}
	#endregion



	protected void SetTrigger(string TriggerParamName)
	{
		AnimComponent.GetAnimator().SetTrigger(TriggerParamName);
	}



	public override void OnPreSpawnedFromPool()
	{
		base.OnPreSpawnedFromPool();

		float[] StatInfo = ActorStatCache.instance.CharacterStatDictionary[InternalName];
		StatComp.InitializeInitStats(StatInfo[0], 0.0f, StatInfo[1], StatInfo[2], StatInfo[3]);
		StatComp.InitializeStats(GameManager.instance.Difficulty);
		bIsDead = false;

		OnMoveSpeedChanged(StatComp.GetMoveSpeed());
	}



	public override void OnPostSpawnedFromPool()
	{
		StatComp.HealthChangedEvent.AddListener(OnHealthChanged);
		StatComp.MoveSpeedChangedEvent.AddListener(OnMoveSpeedChanged);

		NavAgentComponent.SetDestination(transform.position);
		NavAgentComponent.isStopped = false;

		Healthbar.fillAmount = StatComp.GetHealth() / StatComp.GetMaxHealth();
		HealthText.text = StatComp.GetHealth().ToString() + " / " + StatComp.GetMaxHealth().ToString();

		GameManager.instance.ChangeBGM(BossBGM);
	}



	public override void OnReturnedToPool()
	{
		NavAgentComponent.SetDestination(transform.position);
		NavAgentComponent.isStopped = true;

		StatComp.HealthChangedEvent.RemoveListener(OnHealthChanged);
		StatComp.MoveSpeedChangedEvent.RemoveListener(OnMoveSpeedChanged);
	}



	protected void OnMoveSpeedChanged(float NewMoveSpeed)
	{
		NavAgentComponent.speed = NewMoveSpeed;
	}



	protected void OnHealthChanged(float NewHealth)
	{
		if (!bIsDead && NewHealth <= 0.0f)
		{
			bIsDead = true;

			GameManager.instance.AddScore((uint)StatComp.GetMaxHealth() * 2);
			GameManager.instance.AddMoney(1000);

			/*
			if (DeathParticle != null)
			{
				GameplayHelperLibrary.SpawnParticle(DeathParticle.InternalName,
					transform.position,
					transform.rotation;
			}
			*/

			SoundHelperLibrary.SpawnSoundAtLocation(transform.position, Quaternion.identity, DeathSfx, false);

			OnBossDiedEvent.Invoke();

			Healthbar.fillAmount = 0.0f;
			HealthText.text = "0 / " + StatComp.GetMaxHealth().ToString();

			OnDied();
		}
		else
		{
			AnimComponent.GetAnimator().SetTrigger("Hit");
			
			Healthbar.fillAmount = StatComp.GetHealth() / StatComp.GetMaxHealth();
			HealthText.text = StatComp.GetHealth().ToString() + " / " + StatComp.GetMaxHealth().ToString();
		}
	}



	public void TryLookAtPlayer(bool NewActive)
	{
		if (NewActive)
		{
			Quaternion LookAtRot = Quaternion.Euler(0,
			Quaternion.LookRotation(GameplayHelperLibrary.GetPlayer().transform.position - transform.position, Vector3.up).eulerAngles.y,
			0);

			SetTargetRotation(LookAtRot);
		}
		// false인 경우 강제로 회전 멈춤
		else
		{
			bNeedToFixRotation = false;
		}
	}



	public virtual void OnParried()
	{
		SetCanBeParried(false);
		OnBeginAction();
		SetTrigger("Parried");
		SoundHelperLibrary.SpawnSoundAtLocation(transform.position, Quaternion.identity, OnParriedSfx, false, 1.2f, 0.05f);
	}



	public void SetCanBeParried(bool NewActive)
	{
		WeaponCollision.bCanBeParried = NewActive;
	}
}
