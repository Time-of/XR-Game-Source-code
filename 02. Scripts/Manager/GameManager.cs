using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * @author 이성수
 * @brief 게임 매니저 클래스입니다.
 * @since 2023-03-16
 */
public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public float Difficulty = 1.0f;

	[SerializeField, ReadOnlyProperty]
	private uint Score = 0;

	[SerializeField, ReadOnlyProperty]
	private uint Money = 0;

	[SerializeField]
	private AudioClip UseMoneySfx;

	public UnityEvent<uint> OnMoneyUpdatedEvent = new UnityEvent<uint>();

	public EnemySpawner SpawnerComp { get; private set; }

	public UnityEvent<uint> OnDefeatEvent = new UnityEvent<uint>();

	public UnityEvent<uint> OnVictoryEvent = new UnityEvent<uint>();



	[SerializeField]
	private AudioClip DefaultBGM;


	[SerializeField, ReadOnlyProperty]
	private PoolableSound BGMNowPlaying;



	private void Awake()
	{
		SpawnerComp = GetComponent<EnemySpawner>();

		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this);
		}
		else Destroy(this);

		DOTween.Init(true, true, LogBehaviour.ErrorsOnly);
	}



	private void Start()
	{
		// BGM 재생
		ChangeBGMToDefault();

		AddMoney(5);
	}



	public void ChangeBGMToDefault()
	{
		ChangeBGM(DefaultBGM);
	}



	public void ChangeBGM(AudioClip NewBGM)
	{
		BGMNowPlaying?.ReturnToPool();

		BGMNowPlaying = SoundHelperLibrary.SpawnSoundAtLocation(Vector3.zero, Quaternion.identity,
			NewBGM, true, 1.0f, 0.0f, true);
	}



	public void AddScore(uint Value)
	{
		Score += (uint)(Value * Mathf.FloorToInt(Difficulty * 1.25f));
	}



	public void Defeat()
	{
		Debug.Log("<color=red>플레이어 패배!!!</color>");

		OnDefeatEvent.Invoke(Score);
	}



	public void Victory()
	{
		OnVictoryEvent.Invoke(Score);
	}



	public void AddMoney(uint MoneyAmount)
	{
		Money += MoneyAmount;
		OnMoneyUpdatedEvent.Invoke(Money);
	}



	public bool TryUseMoney(uint MoneyAmount)
	{
		if (Money >= MoneyAmount)
		{
			Money -= MoneyAmount;
			OnMoneyUpdatedEvent.Invoke(Money);
			SoundHelperLibrary.SpawnSoundAtLocation(Vector3.zero, Quaternion.identity, UseMoneySfx, true, 1.0f, 0.05f);

			return true;
		}
		else return false;
	}



	public uint GetMoney()
	{
		return Money;
	}
}
