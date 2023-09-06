using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



/**
 * @author 이성수
 * @brief 탄알집 정보를 나타내는 HUD입니다.
 * @since 2023-03-31
 */
public class MagazineInfoHUD : MonoBehaviour
{
	[SerializeField]
	private TMP_Text BulletQuantityText;

	private int MagazineCapacity;



	public void SetMagazineCapacity(int NewCapacity)
	{
		MagazineCapacity = NewCapacity;
	}



	public void UpdateBulletQuantity(int Quantity)
	{
		if (Quantity <= 0)
		{
			BulletQuantityText.text = "<color=red>0</color>/" + MagazineCapacity.ToString();
		}
		else
		{
			BulletQuantityText.text = Quantity.ToString() + "/" + MagazineCapacity.ToString();
		}
	}
}
