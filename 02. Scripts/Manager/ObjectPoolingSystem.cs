using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



/**
 * @author 이성수
 * @brief 오브젝트 풀링 시스템 싱글톤 클래스입니다.
 * @since 2023-03-12
 * @see PoolableObjectBase 참고.
 */
public class ObjectPoolingSystem : MonoBehaviour
{
	public static ObjectPoolingSystem instance;

	private Dictionary<string, Queue<PoolableObject>> ObjectPoolDictionary = new Dictionary<string, Queue<PoolableObject>>();

	private Dictionary<string, PoolableObject> PoolableObjectInfoDictionary = new Dictionary<string, PoolableObject>();



	private void Awake()
	{
		if (instance == null) instance = this;

		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		InitializePoolableObjectInfos();

		InitializePools();
	}



	#region 외부 기능
	/// <summary>
	/// 오브젝트를 가져옵니다. 직접 호출하기보다는 GameplayHelperLibrary.SpawnObject 이용을 추천합니다.
	/// </summary>
	/// <param name="InternalObjectName">오브젝트의 내부 이름</param>
	/// <returns>가져온 오브젝트</returns>
	/** @see GameplayHelperLibrary 참고. */
	public PoolableObject GetObject(string InternalObjectName)
	{
		if (ObjectPoolDictionary.ContainsKey(InternalObjectName) && ObjectPoolDictionary[InternalObjectName].Count > 0)
		{
			PoolableObject AvailableObject = ObjectPoolDictionary[InternalObjectName].Dequeue();
			//AvailableObject.gameObject.SetActive(true);
			//AvailableObject.OnSpawnedFromPool();

			return AvailableObject;
		}
		else
		{
			PoolableObject SpawnedObject = SpawnNewObject(InternalObjectName);
			//SpawnedObject.gameObject.SetActive(true);
			//SpawnedObject.OnSpawnedFromPool();

			Debug.Log("큐 " + InternalObjectName + "에 사용 가능한 오브젝트가 부족해 " + SpawnedObject + " 새로 생성!");

			return SpawnedObject;
		}
	}



	/// <summary>
	/// 오브젝트 풀로 오브젝트를 반환합니다.
	/// </summary>
	/// <param name="ObjectToReturn">반환할 오브젝트</param>
	public void ReturnObjectToPool(PoolableObject ObjectToReturn)
	{
		ObjectToReturn.OnReturnedToPool();

		ObjectToReturn.StopAllCoroutines();
		ObjectToReturn.gameObject.SetActive(false);
		ObjectToReturn.transform.position = Vector3.zero;
		ObjectToReturn.transform.rotation = Quaternion.identity;

		ObjectPoolDictionary[ObjectToReturn.InternalName].Enqueue(ObjectToReturn);
	}
	#endregion



	#region 내부 기능
	/// <summary>
	/// 모든 PoolableObject의 기본 정보를 딕셔너리에 저장합니다.
	/// </summary>
	private void InitializePoolableObjectInfos()
	{
		PoolableObjectInfoDictionary.Clear();

		// 프리팹 폴더에서 모든 PoolableObject를 긁어와 딕셔너리에 추가하기
		foreach (PoolableObject FoundObject in Resources.LoadAll<PoolableObject>("PoolablePrefabs/"))
		{
			PoolableObjectInfoDictionary.Add(FoundObject.InternalName, FoundObject);

			if (!ObjectPoolDictionary.ContainsKey(FoundObject.InternalName))
			{
				ObjectPoolDictionary.Add(FoundObject.InternalName, new Queue<PoolableObject>());
			}
		}
	}



	/// <summary>
	/// 오브젝트 풀에 오브젝트들을 넣습니다.
	/// </summary>
	private void InitializePools()
	{
		/*
		foreach (string InternalName in PoolableObjectInfoDictionary.Keys)
		{
			// PoolQuantity만큼 스폰하여 큐에 집어넣기
			for (int i = 0; i < PoolableObjectInfoDictionary[InternalName].PoolQuantity; i++)
			{
				PoolableObject SpawnedObject = SpawnNewObject(InternalName);

				if (!ObjectPoolDictionary.ContainsKey(InternalName))
				{
					ObjectPoolDictionary.Add(InternalName, new Queue<PoolableObject>());
				}

				ObjectPoolDictionary[InternalName].Enqueue(SpawnedObject);
			}
		}
		*/
	}



	/// <summary>
	/// 새 PoolableObject를 생성하고, 자식 트랜스폼으로 분류합니다. 큐에는 집어넣지 않습니다.
	/// </summary>
	/// <param name="InternalObjectName">새롭게 생성할 오브젝트의 내부 이름</param>
	/// <returns>생성된 오브젝트</returns>
	private PoolableObject SpawnNewObject(string InternalObjectName)
	{
		if (!PoolableObjectInfoDictionary.ContainsKey(InternalObjectName)) Debug.LogError("PoolableObject 정보가 등록되어 있지 않아요!!: " + InternalObjectName);

		PoolableObject SpawnedObject = Instantiate(PoolableObjectInfoDictionary[InternalObjectName]);
		SpawnedObject.gameObject.SetActive(false);

		Transform ChildTransform = transform.Find(SpawnedObject.InternalType.ToString());

		SpawnedObject.transform.SetParent(ChildTransform ?? gameObject.transform);

		return SpawnedObject;
	}
	#endregion
}
