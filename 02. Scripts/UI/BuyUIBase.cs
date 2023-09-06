using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



/**
 * @author 이성수
 * @brief 구매 UI 베이스 클래스입니다.
 * @since 2023-04-02
 */
public class BuyUIBase : MonoBehaviour
{
	protected uint Price = 500;

	[SerializeField]
	protected TMP_Text PriceText;



	protected bool TryUseMoney()
	{
		if (InGameManager.instance.HUDManager == null) return false;

		return GameManager.instance.TryUseMoney(Price);
	}
}
