using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 라운드 시스템에서 사용할 적 스폰 팩토리 클래스입니다.
 * @since 2023-03-23
 */
public class RoundEnemySpawnFactory
{
	public static void SpawnEnemies(List<EnemySpawnInfo> EnemySpawnInfos)
	{
		foreach (EnemySpawnInfo EnemySpawnInfo in EnemySpawnInfos)
		{
			for (int i = 0; i < EnemySpawnInfo.SpawnCount; i++)
			{
				GameplayHelperLibrary.SpawnObject(EnemySpawnInfo.Enemy.InternalName,
				EnemySpawnInfo.SpawnPosition.position,
				Quaternion.identity);
			}
		}
	}
}
