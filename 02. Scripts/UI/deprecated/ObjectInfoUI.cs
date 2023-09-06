using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



/**
 * @author 이성수
 * @brief 오브젝트의 정보 UI를 나타내는 클래스입니다.
 * @since 2023-03-26
 */
public class ObjectInfoUI : MonoBehaviour
{
	/// <summary>
	/// 스탯이 존재하는, UI가 가리키는 대상
	/// </summary>
	public IStatHandler InfoTarget;

	[SerializeField]
	private TMP_Text StatText_Strength;

	[SerializeField]
	private TMP_Text StatText_AttackSpeed;

	[SerializeField]
	private TMP_Text StatText_Defense;

	[SerializeField]
	private TMP_Text StatText_Health;

	[SerializeField]
	private TMP_Text MoneyText;

	private uint MoneyAmount = 0;

	[SerializeField]
	private TMP_Text ExpectedMoneyText;

	private uint InfoTargetPrice = 0;



	private void Start()
	{
		GameManager.instance.OnMoneyUpdatedEvent.AddListener(SetMoneyText);
	}



	public void SetInfoTarget(IStatHandler NewInfoTarget)
	{
		OnInfoTargetRemoved();

		InfoTarget = NewInfoTarget;
		ForceUpdateStatText();
		InfoTarget.GetOnAnyStatChangedEvent().AddListener(UpdateStatText);

		IPurchaseable Purchaseable = InfoTarget.GetOwner().GetComponent<IPurchaseable>();

		if (Purchaseable != null)
		{
			SetPrice(Purchaseable.GetPrice());
		}

		Outline OutlineComp = InfoTarget.GetOwner().GetComponent<Outline>();

		if (OutlineComp != null)
		{
			OutlineComp.enabled = true;
		}

		SetMoneyText(GameManager.instance.GetMoney());
	}



	private void SetPrice(uint NewPrice)
	{
		InfoTargetPrice = NewPrice;
	}



	public void CloseInfo()
	{
		OnInfoTargetRemoved();

		InfoTarget = null;

		SetPrice(0);
		ClearExpectedMoneyText();
		gameObject.SetActive(false);
	}



	public void OnSellButtonHovered()
	{
		ExpectedMoneyText.text = "<color=green>(+ " + InfoTargetPrice / 2 + ")";
	}



	public void SellItem()
	{
		if (InfoTarget == null) return;

		PoolableObject Poolable = InfoTarget.GetOwner().GetComponent<PoolableObject>();

		if (Poolable != null)
		{
			Poolable.ReturnToPool();
			GameManager.instance.AddMoney(InfoTargetPrice / 2);

			CloseInfo();
		}
	}



	public void OnRepairButtonHovered()
	{
		ExpectedMoneyText.text = "<color=red>(- " + InfoTargetPrice / 2 + ")";
	}



	public void RepairObject()
	{
		if (InfoTarget != null && GameManager.instance.TryUseMoney(InfoTargetPrice / 2))
		{
			InfoTarget.SetStat(EStatType.HEALTH, InfoTarget.GetStat(EStatType.MAX_HEALTH));
		}
	}



	public void ClearExpectedMoneyText()
	{
		ExpectedMoneyText.text = "";
	}



	private void OnInfoTargetRemoved()
	{
		if (InfoTarget != null)
		{
			InfoTarget.GetOnAnyStatChangedEvent().RemoveListener(UpdateStatText);

			Outline OutlineComp = InfoTarget.GetOwner().GetComponent<Outline>();

			if (OutlineComp != null)
			{
				OutlineComp.enabled = false;
			}
		}
	}



	private void SetMoneyText(uint NewMoneyAmount)
	{
		MoneyAmount = NewMoneyAmount;
		MoneyText.text = MoneyAmount.ToString();
	}



	private void ForceUpdateStatText()
	{
		if (InfoTarget == null) return;

		float Strength = InfoTarget.GetStat(EStatType.STRENGTH);
		float AttackSpeed = InfoTarget.GetStat(EStatType.ATTACK_SPEED);
		float Defense = InfoTarget.GetStat(EStatType.DEFENSE);
		float Health = Mathf.Round(InfoTarget.GetStat(EStatType.HEALTH));
		float MaxHealth = Mathf.Round(InfoTarget.GetStat(EStatType.MAX_HEALTH));

		// 유효한 능력치만 보여주기
		StatText_Strength.gameObject.SetActive(Strength >= 0.0f);
		StatText_AttackSpeed.gameObject.SetActive(AttackSpeed >= 0.0f);
		StatText_Defense.gameObject.SetActive(Defense >= 0.0f);
		StatText_Health.gameObject.SetActive(Health >= 0.0f);

		StatText_Strength.text = "Strength: " + Strength;
		StatText_AttackSpeed.text = "Attack Speed: " + AttackSpeed;
		StatText_Defense.text = "Defense: " + Defense;
		StatText_Health.text = "Health: " + Health + " / " + MaxHealth;
	}



	private void UpdateStatText(EStatType StatType, float StatValue)
	{
		if (InfoTarget == null) return;

		switch (StatType)
		{
			case EStatType.STRENGTH:
				StatText_Strength.text = "Strength: " + StatValue;
				break;
			case EStatType.ATTACK_SPEED:
				StatText_AttackSpeed.text = "Attack Speed: " + StatValue;
				break;
			case EStatType.DEFENSE:
				StatText_Defense.text = "Defense: " + StatValue;
				break;
			case EStatType.HEALTH:
				if (StatValue > 0.0f)
				{
					StatText_Health.text = "Health: " + Mathf.Round(StatValue) + " / " + Mathf.Round(InfoTarget.GetStat(EStatType.MAX_HEALTH));
				}
				else
				{
					CloseInfo();
				}
				break;
			case EStatType.MAX_HEALTH:
				StatText_Health.text = "Health: " + Mathf.Round(InfoTarget.GetStat(EStatType.HEALTH)) + " / " + Mathf.Round(StatValue);
				break;
			default:
			case EStatType.MOVE_SPEED:
			case EStatType.NONE:
				break;
		}
	}
}
