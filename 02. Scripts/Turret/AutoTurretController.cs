using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;



/**
 * @author 이성수
 * @brief 자동포탑 컨트롤러 클래스입니다.
 * @since 2023-03-21
 */
[RequireComponent(typeof(StatBase))]
public class AutoTurretController : PoolableObject, IDamageable, IPurchaseable
{
	[SerializeField]
	private TurretBase ControlledTurret;

	private StatBase StatComp;

	private static WaitForSeconds FindEnemyWaitForSeconds;

	private float FindEnemyRadius = 5.0f;

	[SerializeField, ReadOnlyProperty]
	private Transform AttackTarget;

	private Quaternion TurretTargetRotation;

	[SerializeField]
	private float TurretRotationSpeed = 25.0f;

	private bool bIsDead = false;

	[SerializeField]
	private PoolableParticle DeathVfx;

	[SerializeField]
	private AudioClip DeathSfx;

	private uint Price = 100;



	public AutoTurretController() : base()
	{
		InternalObjectType = EObjectType.CHARACTER;
		FindEnemyWaitForSeconds = new WaitForSeconds(0.5f);
	}



	private void Awake()
	{
		StatComp = GetComponent<StatBase>();
	}



	private void Update()
	{
		if (AttackTarget == null) return;

		ControlledTurret.transform.rotation = Quaternion.Slerp(ControlledTurret.transform.rotation, TurretTargetRotation, Time.deltaTime * TurretRotationSpeed);

		ControlledTurret.TryFire();
	}



	public override void OnPreSpawnedFromPool()
	{
		SetInstigator(GameplayHelperLibrary.GetPlayer().gameObject);

		//float[] StatInfo = ActorStatCache.instance.Deprecated_AutoTurretStatDictionary[InternalName];
		//StatComp.InitializeInitStats(StatInfo[0], StatInfo[1], StatInfo[2], StatInfo[3], 0.0f);
		//Price = (uint)Mathf.RoundToInt(StatInfo[4]);
		//FindEnemyRadius = StatInfo[5];

		StatComp.InitializeStats();
		bIsDead = false;

		OnStrengthChanged(StatComp.GetStrength());
		OnAttackSpeedChanged(StatComp.GetAttackSpeed());
	}



	public override void OnPostSpawnedFromPool()
	{
		StatComp.StrengthChangedEvent.AddListener(OnStrengthChanged);
		StatComp.AttackSpeedChangedEvent.AddListener(OnAttackSpeedChanged);
		StatComp.HealthChangedEvent.AddListener(OnHealthChanged);

		StartCoroutine(FindEnemyCoroutine());
	}



	public override void OnReturnedToPool()
	{
		StatComp.StrengthChangedEvent.RemoveListener(OnStrengthChanged);
		StatComp.AttackSpeedChangedEvent.RemoveListener(OnAttackSpeedChanged);
		StatComp.HealthChangedEvent.RemoveListener(OnHealthChanged);
	}



	void IDamageable.TakeDamage(float Damage, GameObject Instigator, GameObject DamageCauser)
	{
		float CalculatedDamage = GameplayHelperLibrary.CalculateDefense(Damage, StatComp);
		StatComp.SetHealth(StatComp.GetHealth() - CalculatedDamage);
	}



	uint IPurchaseable.GetPrice()
	{
		return Price;
	}



	private IEnumerator FindEnemyCoroutine()
	{
		while (!bIsDead)
		{
			yield return FindEnemyWaitForSeconds;

			Collider NearestCollider = null;
			float MinDistance = Mathf.Infinity;

			foreach (Collider collider in Physics.OverlapSphere(transform.position, FindEnemyRadius, 1 << LayerMask.NameToLayer("Hostile"), QueryTriggerInteraction.Ignore))
			{
				// Hostile 또는 Friendly 가 아닌 대상에 대해 라인캐스트하여, 적중 시 해당 대상 넘기기 (즉, 중간에 장애물이 있는 경우)
				if (Physics.Linecast(transform.position, collider.transform.position, out RaycastHit HitResult, ~(1 << LayerMask.NameToLayer("Hostile") | 1 << LayerMask.NameToLayer("Friendly")), QueryTriggerInteraction.Ignore))
				{
					continue;
				}

				float DistSqr = Vector3.SqrMagnitude(collider.transform.position - transform.position);

				if (MinDistance > DistSqr)
				{
					NearestCollider = collider;
					MinDistance = DistSqr;
				}
			}

			AttackTarget = NearestCollider?.transform ?? null;

			if (AttackTarget != null)
			{
				TurretTargetRotation = Quaternion.LookRotation((AttackTarget.transform.position - transform.position).normalized, Vector3.up);
			}
		}
	}



#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.1f, 0.6f, 0.1f, 0.7f);

		Gizmos.DrawWireSphere(transform.position, FindEnemyRadius);
	}
#endif



	private void OnStrengthChanged(float NewStrength)
	{
		ControlledTurret.GetStatComp().SetStrength(NewStrength);
	}



	private void OnAttackSpeedChanged(float NewAttackSpeed)
	{
		ControlledTurret.GetStatComp().SetAttackSpeed(NewAttackSpeed);
	}



	private void OnHealthChanged(float NewHealth)
	{
		if (!bIsDead && NewHealth <= 0.0f)
		{
			bIsDead = true;

			if (DeathVfx != null)
			{
				GameplayHelperLibrary.SpawnParticle(DeathVfx.InternalName, transform.position, transform.rotation);
			}

			SoundHelperLibrary.SpawnSoundAtLocation(transform.position, Quaternion.identity, DeathSfx, false);

			ReturnToPool();
		}
	}
}
