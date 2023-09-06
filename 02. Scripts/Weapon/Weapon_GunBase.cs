using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/**
 * @author 이성수
 * @brief 총 무기 베이스 클래스입니다.
 * @since 2023-03-16
 */
[RequireComponent(typeof(WeaponStat))]
public class Weapon_GunBase : WeaponBase
{
	/// <summary>
	/// 총을 꺼낼 때 사운드
	/// </summary>
	[SerializeField]
	private AudioClip DrawSfx;

	[SerializeField]
	private Transform FirePosition;

	[SerializeField]
	private ProjectileBase ProjectileToShot;

	[SerializeField]
	private PoolableParticle MuzzleFlashVfx;

	[SerializeField]
	private AudioClip ShotSfx;

	private float CalculatedAttackSpeed = 1.0f;

	private float FireDelayRemaining = 0.0f;

	[SerializeField, Tooltip("피해량, WeaponStat의 Strength와 가산됨")]
	private float Damage;

	[SerializeField, ReadOnlyProperty]
	private GunMagazine Magazine;

	/// <summary>
	/// 탄알집 장착 여부
	/// </summary>
	public bool bIsMagazineEquipped = true;

	/// <summary>
	/// 총을 발사중인지 여부 (트리거를 누르고 있는지 여부)
	/// </summary>
	private bool bIsFiring;

	private Animator AnimComp;



	protected override void Awake()
	{
		base.Awake();
		Magazine = GetComponentInChildren<GunMagazine>();
		Magazine.TargetGun = this;
		bIsMagazineEquipped = true;
	}



	private void Update()
	{
		if (Magazine == null || !bIsMagazineEquipped) return;

		if (FireDelayRemaining > 0.0f)
		{
			FireDelayRemaining -= Time.deltaTime * CalculatedAttackSpeed;
		}

		if (bIsFiring)
		{
			TryFire();
		}

		// 프라이머리 버튼을 누르면 탄알집 분리
		if (GameplayHelperLibrary.GetPlayer().bRightHandPrimaryButtonPressed)
		{
			Magazine.RemoveFromGun();
		}
	}



	protected override void Start()
	{
		base.Start();

		AnimComp = GetComponent<Animator>();

		OnAttackSpeedChanged(StatComp.GetAttackSpeed());
		StatComp.AttackSpeedChangedEvent.AddListener(OnAttackSpeedChanged);
	}



	private void OnDestroy()
	{
		StatComp.AttackSpeedChangedEvent.RemoveListener(OnAttackSpeedChanged);
	}



	public void StartFiring()
	{
		TryFire();

		bIsFiring = true;
	}



	public void StopFiring()
	{
		bIsFiring = false;
	}



	private void TryFire()
	{
		if (FireDelayRemaining <= 0.0f && Magazine.BulletCount > 0)
		{
			Fire();

			return;
		}

		return;
	}



	private void Fire()
	{
		FireDelayRemaining = 1.0f;
		Magazine.SetBulletCount(Magazine.BulletCount - 1);

		AnimComp?.SetTrigger("Fire");

		GameplayHelperLibrary.GetPlayer().SendHapticToHand(false, 0.3f, 0.1f);

		GameplayHelperLibrary.SpawnParticleAttached(MuzzleFlashVfx.InternalName, FirePosition.position, FirePosition.rotation, FirePosition);

		SoundHelperLibrary.SpawnSoundAttached(FirePosition.position, FirePosition.rotation, ShotSfx, FirePosition, 1.0f, 0.05f);

		ProjectileBase SpawnedProjectile = (ProjectileBase)GameplayHelperLibrary.SpawnObjectDelayed(ProjectileToShot.InternalName,
				FirePosition.position, FirePosition.rotation);

		if (SpawnedProjectile != null)
		{
			SpawnedProjectile.SetupProjectile(new List<GameObject> { gameObject }, GameplayHelperLibrary.CalculateDamage(Damage, StatComp));
			GameplayHelperLibrary.FinishSpawnObject(SpawnedProjectile);
		}
	}



	public void SetMagazineEquipped(bool NewEquipped)
	{
		bIsMagazineEquipped = NewEquipped;
	}



	#region 오버라이드 기능들
	public override void OnEquipped()
	{
		Magazine.GetInfoHUD()?.gameObject.SetActive(true);

		SoundHelperLibrary.SpawnSoundAtLocation(transform.position, transform.rotation, DrawSfx, false, 1.0f, 0.05f);
	}



	public override void OnUnEquipped()
	{
		Magazine.ForceAttachToGun();
		Magazine.GetInfoHUD()?.gameObject.SetActive(false);
	}



	public override void OnActivated()
	{
		StartFiring();
	}



	public override void OnDeactivated()
	{
		StopFiring();
	}
	#endregion



	private void OnAttackSpeedChanged(float NewAttackSpeed)
	{
		CalculatedAttackSpeed = GameplayHelperLibrary.CalculateAttackSpeed(NewAttackSpeed);
	}
}
