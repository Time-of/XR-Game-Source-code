using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 오브젝트 풀링이 가능한 파티클 클래스입니다.
 * @since 2023-03-12
 */
public class PoolableParticle : PoolableObject
{
	// 오브젝트 풀의 원래 부모 트랜스폼
	private static Transform PoolOriginParentTransform;

	[SerializeField]
	private ParticleSystem[] ParticlesToPlay;

	// 풀로부터 스폰 시 자동 재생 여부
	public bool bAutoPlayOnSpawned;

	// 자동으로 풀로 되돌려지는지 여부
	public bool bAutoReturnToPool;

	// 오브젝트에 부착되어 있는지 여부
	public bool bAttachedToObject;

	private float ParticleDuration = -1.0f;



	public PoolableParticle() : base()
	{
		InternalObjectType = EObjectType.PARTICLE;
		bAutoPlayOnSpawned = true;
		bAutoReturnToPool = true;
		bAttachedToObject = false;
	}



	private void Start()
	{
		if (PoolOriginParentTransform == null)
		{
			PoolOriginParentTransform = ObjectPoolingSystem.instance.transform.Find(InternalType.ToString());
		}
	}



	public void PlayParticle()
	{
		foreach (ParticleSystem particle in ParticlesToPlay)
		{
			particle.Play();
		}
	}



	public override void OnPostSpawnedFromPool()
	{
		if (bAutoPlayOnSpawned) PlayParticle();

		if (bAutoReturnToPool)
		{
			if (ParticleDuration <= 0.0f)
			{
				float MaxDuration = 0.0f;

				foreach (ParticleSystem particle in ParticlesToPlay)
				{
					if (MaxDuration < particle.main.duration) MaxDuration = particle.main.duration;
				}

				ParticleDuration = MaxDuration;
			}

			StartCoroutine(ReturnToPool(ParticleDuration + 0.3f));
		}
	}



	public override void OnReturnedToPool()
	{
		if (bAttachedToObject)
		{
			transform.parent = PoolOriginParentTransform;
			bAttachedToObject = false;
		}
	}
}
