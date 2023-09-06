using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 사운드를 사용하기 쉽게 해 주는 헬퍼 라이브러리 클래스입니다.
 * @since 2023-03-20
 */
public class SoundHelperLibrary : MonoBehaviour
{
	public static PoolableSound SpawnSoundAtLocation(Vector3 Location, Quaternion Rotation, AudioClip SoundToPlay, bool bIs2DSound, float Volume = 1.0f, float PitchRandomizer = 0.0f, bool bIsLooping = false)
	{
		if (SoundToPlay == null) return null;

		PoolableSound SoundToSpawn = (PoolableSound)GameplayHelperLibrary.SpawnObjectDelayed("PoolingSound", Location, Rotation, null);
	
		SoundToSpawn.SetSoundInfo(SoundToPlay, bIs2DSound, Volume, PitchRandomizer, bIsLooping);

		GameplayHelperLibrary.FinishSpawnObject(SoundToSpawn);

		return SoundToSpawn;
	}



	public static PoolableSound SpawnSoundAttached(Vector3 Location, Quaternion Rotation, AudioClip SoundToPlay, Transform AttachTransform, float Volume = 1.0f, float PitchRandomizer = 0.0f, bool bIsLooping = false)
	{
		if (SoundToPlay == null) return null;

		PoolableSound SoundToSpawn = (PoolableSound)GameplayHelperLibrary.SpawnObjectDelayed("PoolingSound", Location, Rotation, null);

		SoundToSpawn.SetSoundInfo(SoundToPlay, false, Volume, PitchRandomizer, bIsLooping);
		SoundToSpawn.bAttachedToObject = true;
		SoundToSpawn.transform.parent = AttachTransform;

		GameplayHelperLibrary.FinishSpawnObject(SoundToSpawn);

		return SoundToSpawn;
	}
}
