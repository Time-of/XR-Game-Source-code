using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 인게임 매니저 클래스입니다.
 * @since 2023-04-12
 */
public class InGameManager : MonoBehaviour
{
	public static InGameManager instance;

	[HideInInspector]
	public PlayerController PlayerRef;

	public PlayerHUDManager HUDManager { get; private set; }



	private void Awake()
	{
		if (instance == null) { instance = this; }

		if (PlayerRef == null) StartCoroutine(FindPlayerUntilPlayerSpawned());
	}



	private IEnumerator FindPlayerUntilPlayerSpawned()
	{
		WaitForSeconds WaitSecondsUntilFind = new(0.3f);

		while (PlayerRef == null)
		{
			yield return WaitSecondsUntilFind;

			PlayerRef = FindObjectOfType<PlayerController>();
		}

		HUDManager = PlayerRef.GetComponentInChildren<PlayerHUDManager>();
	}
}
