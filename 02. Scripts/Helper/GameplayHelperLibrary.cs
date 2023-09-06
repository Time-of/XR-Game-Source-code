using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 게임플레이를 만드는 데 도움이 되는 기능들을 모아 놓은 라이브러리입니다.
 * @since 2023-03-12
 */
public class GameplayHelperLibrary : MonoBehaviour
{
	/// <summary>
	/// 오브젝트를 원하는 위치와 회전값으로 스폰합니다.
	/// </summary>
	/// <param name="InternalName">PoolableObject의 내부적으로 사용되는 이름</param>
	/// <param name="Location">위치</param>
	/// <param name="Rotation">회전</param>
	/// <returns>스폰된 오브젝트</returns>
	public static PoolableObject SpawnObject(string InternalName, Vector3 Location, Quaternion Rotation, GameObject Instigator = null)
	{
		PoolableObject ObjectToSpawn = SpawnObjectDelayed(InternalName, Location, Rotation, Instigator);

		if (ObjectToSpawn == null) return null;

		FinishSpawnObject(ObjectToSpawn);

		return ObjectToSpawn;
	}


	/// <summary>
	/// 오브젝트를 원하는 위치와 회전값으로 **지연된 스폰**을 수행합니다.
	/// 스폰이 마무리되기 위해 **반드시 GameplayHelperLibrary.FinishSpawnObject()를 명시적으로 호출**해야 합니다.
	/// </summary>
	/// <param name="InternalName">PoolableObject의 내부적으로 사용되는 이름</param>
	/// <param name="Location">위치</param>
	/// <param name="Rotation">회전</param>
	/// <returns>스폰된 오브젝트</returns>
	public static PoolableObject SpawnObjectDelayed(string InternalName, Vector3 Location, Quaternion Rotation, GameObject Instigator = null)
	{
		PoolableObject ObjectToSpawn = ObjectPoolingSystem.instance.GetObject(InternalName);
		
		if (ObjectToSpawn == null) return null;

		ObjectToSpawn.transform.position = Location;
		ObjectToSpawn.transform.rotation = Rotation;

		ObjectToSpawn.SetInstigator(Instigator);

		ObjectToSpawn.OnPreSpawnedFromPool();

		return ObjectToSpawn;
	}


	/// <summary>
	/// 스폰을 마무리합니다. OnSpawnedFromPool을 호출하고, 활성화 상태로 변경합니다.
	/// </summary>
	/// <param name="ObjectToSpawn">스폰할 오브젝트</param>
	public static void FinishSpawnObject(PoolableObject ObjectToSpawn)
	{
		ObjectToSpawn.gameObject.SetActive(true);
		ObjectToSpawn.OnPostSpawnedFromPool();
	}


	/// <summary>
	/// 파티클을 스폰합니다.
	/// </summary>
	/// <param name="InternalName">내부적으로 사용되는 이름</param>
	/// <param name="Location">위치</param>
	/// <param name="Rotation">회전</param>
	/// <param name="bAutoPlay">자동 재생 여부</param>
	/// <param name="bAutoReturnToPool">재생이 끝나면 자동 반납 여부</param>
	/// <returns>스폰된 파티클</returns>
	public static PoolableParticle SpawnParticle(string InternalName, Vector3 Location, Quaternion Rotation, bool bAutoPlay = true, bool bAutoReturnToPool = true)
	{
		PoolableParticle ParticleToSpawn = InternalSpawnParticle(InternalName);

		if (ParticleToSpawn == null) return null;

		ParticleToSpawn.bAutoPlayOnSpawned = bAutoPlay;
		ParticleToSpawn.bAutoReturnToPool = bAutoReturnToPool;

		ParticleToSpawn.transform.position = Location;
		ParticleToSpawn.transform.rotation = Rotation;

		ParticleToSpawn.bAttachedToObject = false;

		FinishSpawnObject(ParticleToSpawn);

		return ParticleToSpawn;
	}



	/// <summary>
	/// 파티클을 부착된 상태로 스폰합니다.
	/// </summary>
	/// <param name="InternalName">내부적으로 사용되는 이름</param>
	/// <param name="Location">위치</param>
	/// <param name="Rotation">회전</param>
	/// <param name="bAutoPlay">자동 재생 여부</param>
	/// <param name="bAutoReturnToPool">재생이 끝나면 자동 반납 여부</param>
	/// <returns>스폰된 파티클</returns>
	public static PoolableParticle SpawnParticleAttached(string InternalName, Vector3 Location, Quaternion Rotation, Transform AttachTransform, bool bAutoPlay = true, bool bAutoReturnToPool = true)
	{
		PoolableParticle ParticleToSpawn = InternalSpawnParticle(InternalName);

		if (ParticleToSpawn == null) return null;

		ParticleToSpawn.bAutoPlayOnSpawned = bAutoPlay;
		ParticleToSpawn.bAutoReturnToPool = bAutoReturnToPool;

		ParticleToSpawn.transform.position = Location;
		ParticleToSpawn.transform.rotation = Rotation;

		ParticleToSpawn.bAttachedToObject = true;
		ParticleToSpawn.transform.parent = AttachTransform;

		FinishSpawnObject(ParticleToSpawn);

		return ParticleToSpawn;
	}



	public static BuffBase ApplyBuff(string InternalName, StatBase BuffTarget, GameObject Instigator = null)
	{
		BuffBase BuffObjectToSpawn = (BuffBase)SpawnObjectDelayed(InternalName, Vector3.zero, Quaternion.identity, Instigator);

		if (BuffObjectToSpawn == null) return null;

		BuffObjectToSpawn.SetBuffTarget(BuffTarget);

		FinishSpawnObject(BuffObjectToSpawn);

		return BuffObjectToSpawn;
	}



	/// <summary>
	/// 라인 트레이스(LineCast)를 수행합니다.
	/// 트레이스를 수행하는 동안 IgnoreObjects의 레이어가 변경되므로, 암시적인 오작동을 일으킬 가능성이 높습니다.
	/// </summary>
	/// <param name="StartPos">시작 위치</param>
	/// <param name="EndPos">끝 위치</param>
	/// <param name="TraceLayer">검사를 수행할 레이어</param>
	/// <param name="IgnoreObjects">검사를 무시할 오브젝트들</param>
	/// <param name="bDrawDebug">디버그 그리기 여부</param>
	/// <param name="OutHitResult">검사 수행 결과 RaycastHit</param>
	/// <returns>적중 여부</returns>
	public static bool LineTraceByLayer(Vector3 StartPos, Vector3 EndPos, LayerMask TraceLayer, List<GameObject> IgnoreObjects, bool bDrawDebug, out RaycastHit OutHitResult)
	{
		List<LayerMask> OriginalLayerMaskList = new List<LayerMask>();

		// 원본 레이어 임시 저장
		foreach (GameObject IgnoreObject in IgnoreObjects)
		{
			OriginalLayerMaskList.Add(IgnoreObject.layer);
			IgnoreObject.layer = Physics.IgnoreRaycastLayer;
		}

		bool bHit = Physics.Linecast(StartPos, EndPos, out OutHitResult, TraceLayer, QueryTriggerInteraction.Ignore);
		if (bDrawDebug) Debug.DrawLine(StartPos, EndPos, bHit ? Color.green : Color.red, 3.0f);

		// 원본 레이어로 복구
		for (int i = 0; i < IgnoreObjects.Count; i++)
		{
			IgnoreObjects[i].layer = OriginalLayerMaskList[i];
		}

		return bHit;
	}



	public static PlayerController GetPlayer()
	{
		return InGameManager.instance.PlayerRef;
	}



    public static Camera GetPlayerCamera()
    {
        return InGameManager.instance.PlayerRef.GetCamera();
    }



    public static float CalculateDamage(float DamageAmount, IStatHandler StatComp)
	{
		return DamageAmount + StatComp.GetStat(EStatType.STRENGTH);
	}



	public static float CalculateDefense(float ReceivedDamageAmount, IStatHandler StatComp)
	{
		return ReceivedDamageAmount * (1500.0f / (1500.0f + StatComp.GetStat(EStatType.DEFENSE)));
	}



	public static float CalculateAttackSpeed(float AttackSpeedValue)
	{
		return (AttackSpeedValue + 150.0f) / 150.0f;
	}



	#region 내부 기능
	private static PoolableParticle InternalSpawnParticle(string InternalName)
	{
		PoolableParticle ParticleToSpawn = (PoolableParticle)ObjectPoolingSystem.instance.GetObject(InternalName);

		if (ParticleToSpawn == null || ParticleToSpawn.InternalType != EObjectType.PARTICLE)
		{
			Debug.LogWarning(InternalName + "은 파티클이 아니라고 판단되는데도 SpawnParticle()을 사용하려 했습니다. 따라서 해당 함수의 호출을 중지했습니다.");

			return null;
		}

		return ParticleToSpawn;
	}
	#endregion
}
