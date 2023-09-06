using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 터렛 베이스 클래스입니다.
 * @since 2023-03-12
 */
[RequireComponent(typeof(WeaponStat))]
public class TurretBase : MonoBehaviour
{
	[SerializeField]
	private Transform FirePosition;

	[SerializeField]
	private ProjectileBase ProjectileToShot;

	[SerializeField]
	private PoolableParticle MuzzleFlashVfx;

	[SerializeField]
	private AudioClip ShotSfx;

	[SerializeField]
	private float ShotSfxVolume = 1.0f;

	private float CalculatedAttackSpeed = 1.0f;

	private float FireDelayRemaining = 0.0f;

	[SerializeField, Tooltip("피해량, WeaponStat의 Strength와 가산됨")]
	private float Damage;

	private WeaponStat StatComp;



	private void Awake()
	{
		StatComp = GetComponent<WeaponStat>();
		StatComp.AttackSpeedChangedEvent.AddListener(OnAttackSpeedChanged);
		OnAttackSpeedChanged(StatComp.GetAttackSpeed());
	}



	private void Update()
	{
		if (FireDelayRemaining <= 0.0f) return;

		FireDelayRemaining -= Time.deltaTime * CalculatedAttackSpeed;
	}



	public void OnAttackSpeedChanged(float NewAttackSpeed)
	{
		CalculatedAttackSpeed = GameplayHelperLibrary.CalculateAttackSpeed(NewAttackSpeed);
	}



	public bool TryFire()
	{
		if (FireDelayRemaining <= 0.0f)
		{
			Fire();
			return true;
		}

		return false;
	}



	private void Fire()
	{
		FireDelayRemaining = 1.0f;

		GameplayHelperLibrary.SpawnParticleAttached(MuzzleFlashVfx.InternalName, FirePosition.position, FirePosition.rotation, FirePosition);

		SoundHelperLibrary.SpawnSoundAttached(FirePosition.position, FirePosition.rotation, ShotSfx, FirePosition, ShotSfxVolume, 0.05f);

		ProjectileBase SpawnedProjectile = (ProjectileBase)GameplayHelperLibrary.SpawnObjectDelayed(ProjectileToShot.InternalName,
				FirePosition.position, FirePosition.rotation);

		if (SpawnedProjectile != null)
		{
			SpawnedProjectile.SetupProjectile(new List<GameObject> { gameObject }, GameplayHelperLibrary.CalculateDamage(Damage, StatComp));
			GameplayHelperLibrary.FinishSpawnObject(SpawnedProjectile);
		}
	}



	public WeaponStat GetStatComp()
	{
		return StatComp;
	}
}
