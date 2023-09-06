using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 보스 무기 콜리전 클래스
 * @since 2023-06-10
 */
public class BossWeaponCollision : MonoBehaviour
{
	// 공격마다 수정 가능
	[SerializeField, ReadOnlyProperty]
	public float AttackDamage;

	[SerializeField]
	private PoolableParticle AttackVfx;

	[SerializeField]
	private Transform AttackVfxPosition;

	[SerializeField]
	private List<AudioClip> AttackSfxs = new();

	[SerializeField]
	private float AttackSfxVolume = 0.8f;

	[SerializeField]
	private StatBase StatComp;

	private ABossBase Boss;

	private bool bCanAttack = false;

	public bool bCanBeParried = false;



	private void Start()
	{
		if (StatComp == null)
		{
			StatComp = GetComponentInParent<StatBase>();
		}

		Boss = StatComp.GetComponent<ABossBase>();
	}



	public void SetCanAttack(bool NewActive)
	{
		bCanAttack = NewActive;
	}



	private void OnTriggerEnter(Collider other)
	{
		if (!bCanAttack) return;

		if (other.CompareTag("MeleeWeapon") && bCanBeParried)
		{
			Boss.OnParried();
			GameplayHelperLibrary.GetPlayer().SendHapticToHand(false, 3.0f, 0.22f);

			if (AttackVfx != null)
			{
				GameplayHelperLibrary.SpawnParticle(AttackVfx.InternalName, AttackVfxPosition.position, transform.rotation);
			}

			return;
		}

		IDamageable damageable = other.GetComponent<IDamageable>();

		if (damageable != null && other.CompareTag("Friendly"))
		{
			damageable.TakeDamage(GameplayHelperLibrary.CalculateDamage(AttackDamage, StatComp), gameObject, gameObject);

			if (AttackVfx != null)
			{
				GameplayHelperLibrary.SpawnParticle(AttackVfx.InternalName, other.transform.position, transform.rotation);
			}

			int Length = AttackSfxs.Count;
			if (Length > 0)
			{
				SoundHelperLibrary.SpawnSoundAtLocation(other.transform.position, Quaternion.identity, GetRandomAttackSfx(Length), false, AttackSfxVolume, 0.05f);
			}
		}
	}



	private AudioClip GetRandomAttackSfx(int Length)
	{
		return AttackSfxs[Random.Range(0, Length - 1)];
	}
}
