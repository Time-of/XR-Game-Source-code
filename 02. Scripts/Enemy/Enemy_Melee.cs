using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 근접 몬스터 클래스입니다.
 * @since 2023-03-20
 */
public class Enemy_Melee : EnemyCharacterBase
{
	[SerializeField]
	private float AttackRadius = 2.0f;

	[SerializeField]
	private Transform AttackPosition;

	[SerializeField]
	private float AttackDamage;

	[SerializeField]
	private PoolableParticle AttackVfx;

	[SerializeField]
	private AudioClip AttackSfx;

	[SerializeField]
	private float AttackSfxVolume = 0.8f;



	protected override void Awake()
	{
		base.Awake();

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
		foreach (Collider collider in Physics.OverlapSphere(AttackPosition.position, AttackRadius, (1 << LayerMask.NameToLayer("Friendly") | 1 << LayerMask.NameToLayer("Pawn")), QueryTriggerInteraction.Ignore))
		{
			IDamageable damageable = collider.GetComponent<IDamageable>();

			damageable?.TakeDamage(GameplayHelperLibrary.CalculateDamage(AttackDamage, StatComp), gameObject, gameObject);
		}

		if (AttackVfx != null)
		{
			GameplayHelperLibrary.SpawnParticle(AttackVfx.InternalName, AttackPosition.position, transform.rotation);
		}

		SoundHelperLibrary.SpawnSoundAtLocation(AttackPosition.position, Quaternion.identity, AttackSfx, false, AttackSfxVolume, 0.05f);
	}



#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if (AttackPosition == null) return;

		Gizmos.color = new Color(0.7f, 0.1f, 0.1f, 0.7f);

		Gizmos.DrawWireSphere(AttackPosition.position, AttackRadius);
	}
#endif
}
