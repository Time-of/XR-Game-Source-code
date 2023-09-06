using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/**
 * @author 이성수
 * @brief 무기를 드는 손 클래스입니다.
 * @since 2023-04-16
 */
public class HandWeaponPresense : HandPresense
{
	[SerializeField, ReadOnlyProperty]
	private WeaponBase CurrentWeapon;

	[SerializeField]
	private CustomXRRayInteractor RayInteractor;

	[SerializeField]
	private ControllerDirectInteractor DirectInteractor;

	[SerializeField]
	private WeaponInventory WeaponInventoryUI;

	[SerializeField]
	private List<WeaponBase> DefaultWeaponPrefabs;



	protected override void Start()
	{
		base.Start();

		foreach (var WeaponPrefab in DefaultWeaponPrefabs)
		{
			SpawnWeapon(WeaponPrefab);
		}
	}



	protected override void Update()
	{
		base.Update();

		if (TargetDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool bInventoryOpenButtonClicked))
		{
			// 버튼을 눌렀을 때
			if (bInventoryOpenButtonClicked)
			{
				// 인벤토리 UI가 비활성화인 상태에서만 작동
				if (!WeaponInventoryUI.gameObject.activeSelf)
				{
					// 활성화하면서 위치를 손의 위치로 변경, 회전값은 카메라 회전값으로 설정
					WeaponInventoryUI.transform.position = transform.position;
					WeaponInventoryUI.transform.rotation = Quaternion.Euler(0.0f, GameplayHelperLibrary.GetPlayer().GetCameraRotation().eulerAngles.y, 0.0f);

					WeaponInventoryUI.gameObject.SetActive(true);
					WeaponInventoryUI.SetMoneyText(GameManager.instance.GetMoney());
				}
			}
			// 뗐을 때
			else
			{
				// 인벤토리 UI가 활성화인 상태에서만 작동
				if (WeaponInventoryUI.gameObject.activeSelf)
				{
					WeaponBase InventoryWeapon = WeaponInventoryUI.TryBuyWeapon();

					// 인벤토리 무기가 유효하다면 장착한다. 유효하지 않다면 손으로 변경된다.
					EquipWeapon(InventoryWeapon);

					WeaponInventoryUI.gameObject.SetActive(false);
				}
			}
		}

		if (!bUseHand && TargetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool bTriggerButton))
		{
			if (bTriggerButton) CurrentWeapon?.OnActivated();
			else CurrentWeapon?.OnDeactivated();
		}
	}



	private void EquipWeapon(WeaponBase WeaponToEquip)
	{
		if (WeaponToEquip == null)
		{
			SetUseHand(true);

			RayInteractor.gameObject.SetActive(true);
			DirectInteractor.enabled = true;

			UnEquipWeapon();

			return;
		}
		else
		{
			SetUseHand(false);

			RayInteractor.gameObject?.SetActive(false);
			DirectInteractor.enabled = false;

			UnEquipWeapon();

			CurrentWeapon = WeaponToEquip;

			CurrentWeapon.gameObject.SetActive(true);
			CurrentWeapon.OnEquipped();
		}
	}



	private void UnEquipWeapon()
	{
		CurrentWeapon?.OnDeactivated();
		CurrentWeapon?.OnUnEquipped();
		CurrentWeapon?.gameObject.SetActive(false);
	}



	public void SpawnWeapon(WeaponBase WeaponPrefab)
	{
		if (WeaponPrefab == null) return;

		WeaponBase SpawnedWeapon = Instantiate(WeaponPrefab, transform);

		SpawnedWeapon.transform.SetParent(this.transform);

		// XR Grab Interactable에서 일부 따온 부분 (Pivot 설정)
		var AttachOffset = SpawnedWeapon.transform.position - SpawnedWeapon.GetAttachTransform().position;
		var LocalAttachOffset = SpawnedWeapon.GetAttachTransform().InverseTransformDirection(AttachOffset);

		SpawnedWeapon.transform.localPosition = LocalAttachOffset;
		SpawnedWeapon.transform.localRotation = Quaternion.Inverse(Quaternion.Inverse(SpawnedWeapon.transform.rotation) * SpawnedWeapon.GetAttachTransform().rotation);

		SpawnedWeapon.gameObject.SetActive(false);

		WeaponInventoryUI.AddWeaponToList(SpawnedWeapon);
	}
}
