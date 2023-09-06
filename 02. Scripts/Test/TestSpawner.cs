using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 테스트용 스포너 클래스입니다.
 * @since 2023-03-21
 */
public class TestSpawner : MonoBehaviour
{
	[SerializeField]
	private PoolableObject SpawnObject;

	[SerializeField]
	private float SpawnTime = 3.0f;



	private void Start()
	{
		Invoke("SpawnPoolableObject", SpawnTime);
	}



	private void SpawnPoolableObject()
	{
		if (SpawnObject == null) return;
		GameplayHelperLibrary.SpawnObject(SpawnObject.InternalName, transform.position, transform.rotation);
	}
}
