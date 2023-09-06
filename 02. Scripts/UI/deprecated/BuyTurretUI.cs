using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/**
 * @author 이성수
 * @brief 포탑 구매 UI입니다.
 * @since 2023-03-26
 */
public class BuyTurretUI : BuyUIBase
{
	[SerializeField]
	private PlacePreviewHologram PlacePreviewPrefab;

	/// <summary>
	/// 터렛 구매 시 발생하는 이벤트, 인스펙터에서 지정할 것!!
	/// </summary>
	[SerializeField]
	private UnityEvent<PlacePreviewHologram> OnBoughtTurretEvent = new UnityEvent<PlacePreviewHologram>();



	protected void Start()
	{
		//Price = (uint)Mathf.RoundToInt(ActorStatCache.instance.
		//	Deprecated_AutoTurretStatDictionary[PlacePreviewPrefab.ObjectPrefabToPlace.InternalName][4]);

		PriceText.text = "Price: " + Price;
	}



	public void TryBuyTurret()
	{
		if (TryUseMoney())
		{
			InGameManager.instance.HUDManager.ForceCloseMenuUI();

			OnBoughtTurretEvent.Invoke(PlacePreviewPrefab);

			Debug.Log("<color=green>포탑 구매 UI:</color> 포탑 " + PlacePreviewPrefab.ObjectPrefabToPlace + "구매 완료");
		}
	}
}
