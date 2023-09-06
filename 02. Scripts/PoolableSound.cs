using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 오브젝트 풀링이 가능한 사운드 클래스입니다.
 * @details 루프 사운드인 경우 적당한 타이밍에 직접 반환을 해 줘야 합니다.
 * @since 2023-03-20
 */
[RequireComponent(typeof(AudioSource))]
public class PoolableSound : PoolableObject
{
	// 오브젝트 풀의 원래 부모 트랜스폼
	private static Transform PoolOriginParentTransform;

	private AudioSource AudioSourceComp;

	public bool bAttachedToObject;

	public bool bIsLoopingSound;



	public PoolableSound() : base()
	{
		InternalObjectName = "Sound";
		InternalObjectType = EObjectType.SOUND;
		//PoolQuantity = 20;
	}



	private void Awake()
	{
		AudioSourceComp = GetComponent<AudioSource>();
	}



	private void Start()
	{
		if (PoolOriginParentTransform == null)
		{
			PoolOriginParentTransform = ObjectPoolingSystem.instance.transform.Find(InternalType.ToString());
		}
	}



	public override void OnPostSpawnedFromPool()
	{
		AudioSourceComp.Play();

		if (!bIsLoopingSound)
			StartCoroutine(ReturnToPool(AudioSourceComp.clip.length));
	}



	public override void OnReturnedToPool()
	{
		AudioSourceComp.Stop();
		AudioSourceComp.clip = null;
		bIsLoopingSound = false;

		if (bAttachedToObject)
		{
			transform.parent = PoolOriginParentTransform;
			bAttachedToObject = false;
		}
	}



	public void SetSoundInfo(AudioClip SoundToPlay, bool bIs2DSound, float Volume = 1.0f, float PitchRandomizer = 0.0f, bool bIsLooping = false)
	{
		AudioSourceComp.clip = SoundToPlay;
		AudioSourceComp.spatialBlend = bIs2DSound ? 0.0f : 1.0f;
		AudioSourceComp.volume = Volume;
		AudioSourceComp.pitch = 1 + (PitchRandomizer != 0.0f ? Random.Range(-PitchRandomizer, PitchRandomizer) : 0.0f);
		bIsLoopingSound = bIsLooping;
		AudioSourceComp.loop = bIsLoopingSound;
	}



	public void SetSoundVolume(float NewVolume)
	{
		AudioSourceComp.volume = NewVolume;
	}
}
