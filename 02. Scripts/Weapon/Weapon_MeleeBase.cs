using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 근접 무기 베이스 클래스입니다.
 * @since 2023-04-15
 */
public class Weapon_MeleeBase : WeaponBase
{
	// 타격에 필요한 속도 값
	[SerializeField]
	private float HitSpeedThreshold = 0.8f;

	// 타격을 했을 때 최대치로 피해를 입히기까지 필요한 시간 (타격 후 이 시간이 지나야 최대 피해량이 들어감)
	[SerializeField]
	private float ChargeTimeThreshold = 0.3f;

	// 피해량이 최대치가 아닐 때 피해 배율
	[SerializeField]
	private float NoChargedDamageRate = 0.2f;

	// 최대 피해량 입힐 수 있음을 의미
	private bool bIsCharged;

	[SerializeField]
	private List<AudioClip> HitSfxs;

	[SerializeField]
	private PoolableParticle HitVfx;

	[SerializeField]
	private AudioClip EquipSfx;

    private Vector3 Velocity;

	private Vector3 LastPosition;



	private void Update()
	{
		Velocity = (LastPosition - transform.position) / Time.deltaTime;
	}



	private void LateUpdate()
	{
		LastPosition = transform.position;
	}



	public override void OnEquipped()
	{
		SoundHelperLibrary.SpawnSoundAtLocation(transform.position, transform.rotation, EquipSfx, false, 1.0f, 0.05f);
	}



	public override void OnUnEquipped()
	{

	}



	private void ApplyDamageToTarget(IDamageable Damageable)
	{
		PlayerController PC = GameplayHelperLibrary.GetPlayer();

		float CalculatedDamage = GameplayHelperLibrary.CalculateDamage(PC.GetStrength(), StatComp);
		CalculatedDamage *= bIsCharged ? 1.0f : NoChargedDamageRate;

		Damageable.TakeDamage(CalculatedDamage, PC.gameObject, this.gameObject);

		ProcessEffects();

		GameplayHelperLibrary.GetPlayer().SendHapticToHand(false, 0.7f, 0.1f);

		StopCoroutine("ChargeFullDamageCoroutine");
		StartCoroutine(ChargeFullDamageCoroutine());
	}



	private void ProcessEffects()
	{
		SoundHelperLibrary.SpawnSoundAtLocation(transform.position, transform.rotation, GetRandomHitSfx(), false, 1.0f, 0.05f);

		if (HitVfx != null)
		{
			GameplayHelperLibrary.SpawnParticle(HitVfx.InternalName, transform.position, Quaternion.identity);
		}
	}



	private void OnTriggerEnter(Collider other)
	{
		// 적에게 HitSpeedThreshold 이상의 속력으로 트리거 진입이 발생한다면
		if (other.gameObject.CompareTag("Hostile") && Velocity.sqrMagnitude >= HitSpeedThreshold * HitSpeedThreshold)
		{
			IDamageable Damageable = other.gameObject.GetComponent<IDamageable>();

			if (Damageable != null)
			{
				ApplyDamageToTarget(Damageable);
			}
		}
	}



	private IEnumerator ChargeFullDamageCoroutine()
	{
		bIsCharged = false;

		PlayerController PC = GameplayHelperLibrary.GetPlayer();
		float CalculatedAttackSpeed = GameplayHelperLibrary.CalculateAttackSpeed(PC.GetAttackSpeed());

		// 플레이어의 공격속도에 따라 충전 시간이 짧아짐
		yield return new WaitForSeconds(ChargeTimeThreshold / CalculatedAttackSpeed);

		bIsCharged = true;
	}



	private AudioClip GetRandomHitSfx()
	{
		int Length = HitSfxs.Count;

		if (Length <= 0) return null;
		else if (Length == 1) return HitSfxs[0];

		return HitSfxs[Random.Range(0, Length - 1)];
	}
}
