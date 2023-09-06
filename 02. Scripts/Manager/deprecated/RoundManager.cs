using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public struct RoundInfo
{
	public List<EnemySpawnInfo> EnemySpawnInfos;

	/// <summary>
	/// 라운드 지속시간(이 시간이 끝나면 다음 라운드)
	/// </summary>
	public float RoundDuration;

	/// <summary>
	/// 라운드 끝날 때 받는 돈
	/// </summary>
	public uint EarnMoneyAmountOnRoundEnd;
}



[System.Serializable]
public struct EnemySpawnInfo
{
	public Transform SpawnPosition;
	public EnemyCharacterBase Enemy;
	public uint SpawnCount;
}



/**
 * @author 이성수
 * @brief 라운드를 관리하는 매니저 클래스입니다.
 * @since 2023-03-23
 */
public class RoundManager : MonoBehaviour
{
	[SerializeField, ReadOnlyProperty]
	private int CurrentRound = -1;

	[SerializeField, ReadOnlyProperty]
	private int MaxRound;

	// 최초 라운드 시작 시간
	[SerializeField]
	private float InitialRoundStartTime = 30.0f;

	[SerializeField]
	private List<RoundInfo> RoundInfos = new List<RoundInfo>();



	private void Start()
	{
		MaxRound = RoundInfos.Count;

		CurrentRound = -1;
		StartCoroutine(StartNextRoundCoroutine(InitialRoundStartTime));
	}



	private IEnumerator StartNextRoundCoroutine(float WaitTime)
	{
		yield return new WaitForSeconds(WaitTime);

		if (CurrentRound >= 0)
		{
			// 라운드 완료 돈 지급
			GameManager.instance.AddMoney(RoundInfos[CurrentRound].EarnMoneyAmountOnRoundEnd);
		}

		// 다음 라운드 시작
		if (++CurrentRound < MaxRound)
		{
			SpawnEnemies();

			Debug.Log("<color=purple>" + CurrentRound + " 라운드 시작: " + WaitTime + "초 후 다음 라운드 시작 예정</color>");

			StartCoroutine(StartNextRoundCoroutine(RoundInfos[CurrentRound].RoundDuration));
		}
		else
		{
			Debug.Log("<color=purple>라운드가 모두 종료됨!</color>");
		}
	}



	private void SpawnEnemies()
	{
		RoundEnemySpawnFactory.SpawnEnemies(RoundInfos[CurrentRound].EnemySpawnInfos);
	}
}
