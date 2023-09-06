using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
struct FEnemyRandomWeightInfo
{
	[SerializeField]
	public EnemyCharacterBase Enemy;
	
	[SerializeField]
	public float Weight;
}



/**
 * @author 이성수
 * @brief 적 캐릭터를 스폰하는 관리자 클래스입니다.
 * @since 2023-05-30
 */
public class EnemySpawner : MonoBehaviour
{
	[SerializeField]
	private List<FEnemyRandomWeightInfo> EnemyWeightInfos = new();

	[SerializeField]
	private ABossBase BossEnemyPrefab;



	public EnemyCharacterBase SpawnEnemyAtTransform(Transform SpawnTransform)
	{
		EnemyCharacterBase SelectedEnemy = SelectEnemyFromWeight();

		if (SelectedEnemy == null) return null;

		// 이 프로젝트는 오랜만에 코딩하다 보니 인스턴시에이트로 해버렸네....
		/**
		EnemyCharacterBase SpawnedEnemy = Instantiate(
			SelectedEnemy,
			SpawnTransform.position,
			SpawnTransform.rotation
			);
		*/

		EnemyCharacterBase SpawnedEnemy = (EnemyCharacterBase)GameplayHelperLibrary.SpawnObject(
			SelectedEnemy.InternalName,
			SpawnTransform.position,
			SpawnTransform.rotation,
			null
			);

		return SpawnedEnemy;
	}



	public ABossBase SpawnBossAtTransform(Transform SpawnTransform)
	{
		if (BossEnemyPrefab == null) return null;

		ABossBase SpawnedEnemy = (ABossBase)GameplayHelperLibrary.SpawnObject(
			BossEnemyPrefab.InternalName,
			SpawnTransform.position,
			SpawnTransform.rotation,
			null
			);

		return SpawnedEnemy;
	}



	private EnemyCharacterBase SelectEnemyFromWeight()
	{
		float TotalWeight = 0.0f;

		foreach (var Info in EnemyWeightInfos)
		{
			TotalWeight += Info.Weight;
		}

		if (TotalWeight <= 0.0f) return null;

		float Pivot = Random.Range(0.0f, TotalWeight);

		foreach (var Info in EnemyWeightInfos)
		{
			if (Pivot < Info.Weight)
			{
				return Info.Enemy;
			}
			else
			{
				Pivot -= Info.Weight;
			}
		}

		return EnemyWeightInfos[EnemyWeightInfos.Count - 1].Enemy;
	}
}