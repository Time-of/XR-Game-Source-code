using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 투사체 베이스 클래스입니다.
 * @since 2023-03-12
 * @todo 어차피 레이어 단위로 사용하는 걸로 바꿨는데 IgnoreObjects 빼도 될 듯?
 */
public class ProjectileBase : PoolableObject
{
	[SerializeField]
	private float Speed = 30.0f;

	[SerializeField]
	private LayerMask TargetLayerMask = 1;

	[SerializeField]
	private string TargetTag = "Hostile";

	[SerializeField]
	private float MaxLifetime = 5.0f;

	private RaycastHit HitInfo;

	private bool bHit = false;

	[SerializeField]
	private PoolableParticle HitParticle;

	[SerializeField]
	private AudioClip HitSfx;

	private List<GameObject> IgnoreObjects = new List<GameObject>();

	private float Damage = 0.0f;



	public void SetupProjectile(List<GameObject> NewIgnoreObjects, float NewDamage)
	{
		IgnoreObjects = NewIgnoreObjects;
		Damage = NewDamage;
	}



	public override void OnPreSpawnedFromPool()
	{
		base.OnPreSpawnedFromPool();

		bHit = false;
		IgnoreObjects.Clear();
	}



	public override void OnPostSpawnedFromPool()
	{
		StartCoroutine(ReturnToPool(MaxLifetime));
	}



	public override void OnReturnedToPool()
	{
		if (bHit)
		{
			if (HitParticle != null)
				GameplayHelperLibrary.SpawnParticle(HitParticle.InternalName, HitInfo.point, Quaternion.FromToRotation(Vector3.up, HitInfo.normal));

			SoundHelperLibrary.SpawnSoundAtLocation(HitInfo.point, transform.rotation, HitSfx, false, 1.0f, 0.05f);
		}
	}



	private void Update()
	{
		Vector3 Velocity = Speed * Time.deltaTime * transform.forward;
		//bool bTraceHit = GameplayHelperLibrary.LineTraceByLayer(transform.position, transform.position + Velocity,
		//	TargetLayerMask, IgnoreObjects, true, out HitInfo);
		bool bTraceHit = Physics.Linecast(transform.position, transform.position + Velocity,
			out HitInfo, TargetLayerMask, QueryTriggerInteraction.Ignore);

		if (!bHit && bTraceHit)
		{
			bHit = true;

			if (HitInfo.collider.gameObject.CompareTag(TargetTag))
			{
				IDamageable damageable = HitInfo.collider.gameObject.GetComponent<IDamageable>();

				if (damageable != null) damageable.TakeDamage(Damage, Instigator, gameObject);
			}

			StopAllCoroutines();
			ReturnToPool();
		}

		transform.position += Velocity;
	}
}
