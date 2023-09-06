using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author 이성수
 * @brief 무기 인벤토리 클래스입니다.
 * @since 2023-04-16
 */
public class WeaponInventory : MonoBehaviour
{
	public List<WeaponBase> WeaponList { get; private set; }

	public BuyWeaponUI SelectedWeaponUI { get; private set; }

	[SerializeField]
	private Scrollbar ScrollbarComp;

	[SerializeField]
	private TMP_Text MoneyText;



	public WeaponInventory() : base()
	{
		WeaponList = new List<WeaponBase>();
	}



	private void Start()
	{
		GameManager.instance.OnMoneyUpdatedEvent.AddListener(SetMoneyText);
	}



	public void SetMoneyText(uint MoneyAmount)
	{
		MoneyText.text = MoneyAmount.ToString();
	}



	public void ScrollScrollbar(float Power)
	{
		ScrollbarComp.value = Mathf.Clamp(ScrollbarComp.value + Power, 0.0f, 1.0f);
	}



	public void AddWeaponToList(WeaponBase WeaponToAdd)
	{
		WeaponList.Add(WeaponToAdd);
	}



	public void SetSelectedWeaponUI(BuyWeaponUI WeaponUI)
	{
		SelectedWeaponUI = WeaponUI;
	}



	public WeaponBase TryBuyWeapon()
	{
		if (SelectedWeaponUI == null) return null;
		else if (SelectedWeaponUI.GetWeaponInternalName() == "NONE") return null;

		if (SelectedWeaponUI.bAlreadyBought) return FindWeaponByInternalName(SelectedWeaponUI.GetWeaponInternalName());
		else
		{
			return SelectedWeaponUI.BuyWeapon() ? FindWeaponByInternalName(SelectedWeaponUI.GetWeaponInternalName()) : null;
		}
	}



	private WeaponBase FindWeaponByInternalName(string WeaponInternalName)
	{
		return WeaponList.Find(x => x.InternalName == WeaponInternalName);
	}
}
