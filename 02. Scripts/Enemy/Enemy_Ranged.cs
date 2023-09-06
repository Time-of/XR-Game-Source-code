using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 원거리 몬스터 클래스입니다.
 * @since 2023-03-27
 */
public class Enemy_Ranged : EnemyCharacterBase
{
	[SerializeField]
	private Transform AttackPosition;

	[SerializeField]
	private PoolableParticle AttackVfx;

	[SerializeField]
	private AudioClip AttackSfx;

	[SerializeField]
	private ProjectileBase AttackProjectile;

	[SerializeField]
	private float AttackSfxVolume = 0.8f;



	protected override void Awake()
	{
		base.Awake();

		AnimComp = GetComponent<Animator>();
	}



	protected override void ExecuteAttack()
	{
		transform.rotation = Quaternion.Euler(0.0f, Quaternion.LookRotation((CurrentAITarget.transform.position - transform.position).normalized, Vector3.up).eulerAngles.y, 0.0f);
		AnimComp.SetTrigger("Attack");
	}



	protected override void OnAttackSpeedChanged(float NewAttackSpeed)
	{
		float CalculatedAttackSpeed = GameplayHelperLibrary.CalculateAttackSpeed(NewAttackSpeed);
		AnimComp.SetFloat("AttackSpeedMult", CalculatedAttackSpeed);
	}



	public void AttackTrigger()
	{
		if (AttackVfx != null)
		{
			GameplayHelperLibrary.SpawnParticle(AttackVfx.InternalName, AttackPosition.position, transform.rotation);
		}

		SoundHelperLibrary.SpawnSoundAtLocation(AttackPosition.position, Quaternion.identity, AttackSfx, false, AttackSfxVolume, 0.05f);

		ProjectileBase SpawnedProjectile = (ProjectileBase)GameplayHelperLibrary.SpawnObjectDelayed(AttackProjectile.InternalName,
				AttackPosition.position, Quaternion.LookRotation((GameplayHelperLibrary.GetPlayerCamera().transform.position - transform.position).normalized, Vector3.up));

		if (SpawnedProjectile != null)
		{
			SpawnedProjectile.SetupProjectile(new List<GameObject> { gameObject }, GameplayHelperLibrary.CalculateDamage(0.0f, StatComp));
			GameplayHelperLibrary.FinishSpawnObject(SpawnedProjectile);
		}
	}
}
