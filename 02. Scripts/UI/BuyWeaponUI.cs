using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



/**
 * @author 이성수
 * @brief 무기 구매 UI 클래스입니다.
 * @details 한 번 구매한 이후에는 소환 버튼으로 변경됩니다.
 * @details 하나의 무기는 하나의 인스턴스만 존재할 수 있습니다.
 * @since 2023-04-02
 */
public class BuyWeaponUI : BuyUIBase
{
	[SerializeField]
	private WeaponBase WeaponToBuy;

	public bool bAlreadyBought = false;

	private Button BuyButton;

	private BoxCollider BoxComp;



	protected void Start()
	{
		BuyButton = GetComponent<Button>();

		BoxComp = GetComponent<BoxCollider>();

		if (PriceText != null && WeaponToBuy != null)
		{
			Price = (uint)Mathf.RoundToInt(ActorStatCache.instance.
			WeaponStatDictionary[WeaponToBuy.InternalName][2]);

			PriceText.text = "Price: " + Price;
		}
	}



	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("WeaponHand"))
		{
			InGameManager.instance.HUDManager.WeaponInventoryUI.SetSelectedWeaponUI(this);

			PointerEventData PointerData = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(BuyButton.gameObject, PointerData, ExecuteEvents.pointerEnterHandler);
		}
	}



	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("WeaponHand"))
		{
			//HUDManagerComp.WeaponInventoryUI.SetSelectedWeaponUI(null);

			PointerEventData PointerData = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(BuyButton.gameObject, PointerData, ExecuteEvents.pointerExitHandler);
		}
	}



	public bool BuyWeapon()
	{
		if (TryUseMoney())
		{
			PriceText?.SetText("<color=green>꺼내기</color>");
			bAlreadyBought = true;

			return true;
		}

		return false;
	}



	public string GetWeaponInternalName()
	{
		return WeaponToBuy?.InternalName ?? "NONE";
	}
}
