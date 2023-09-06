using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 게임플레이에 필요한 UI를 관리하는 클래스입니다.
 * @details 플레이어(왼손)에 직접 붙는 HUD들은 제외
 * @since 2023-03-26
 */
public class GameplayUIManager : MonoBehaviour
{
	public static GameplayUIManager instance;

	[SerializeField]
	private ObjectInfoUI ObjectInfoUIRef;



	private void Awake()
	{
		if (instance == null) instance = this;
	}



	public void OpenObjectInfoUI(IStatHandler NewInfoTarget)
	{
		ObjectInfoUIRef.SetInfoTarget(NewInfoTarget);

		Vector3 PlayerPosition = GameplayHelperLibrary.GetPlayer().transform.position + Vector3.up * GameplayHelperLibrary.GetPlayer().GetCapsuleHalfHeight();
		ObjectInfoUIRef.transform.position = PlayerPosition + GameplayHelperLibrary.GetPlayer().transform.forward * 0.7f;
		ObjectInfoUIRef.transform.rotation = 
			Quaternion.LookRotation((ObjectInfoUIRef.transform.position - PlayerPosition).normalized,
				Vector3.up);

		ObjectInfoUIRef.gameObject.SetActive(true);
	}
}
