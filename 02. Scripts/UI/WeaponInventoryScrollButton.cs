using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 무기 인벤토리 UI의 스크롤 버튼입니다.
 * @since 2023-04-16
 */
public class WeaponInventoryScrollButton : MonoBehaviour
{
	[SerializeField]
	private float ScrollPower = 0.0f;



	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("WeaponHand"))
		{
			InGameManager.instance.HUDManager.WeaponInventoryUI.ScrollScrollbar(ScrollPower * Time.deltaTime);
		}
	}
}
