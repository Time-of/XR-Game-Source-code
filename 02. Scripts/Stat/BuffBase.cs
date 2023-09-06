using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public enum EBuffOperator
{
	ADD, SUB, MULTIPLY, DIVIDE
}



/**
 * @author 이성수
 * @brief 스탯을 조절하는 버프 클래스입니다.
 * @since 2023-03-20
 * @see GameplayHelperLibrary의 ApplyBuff() 참고.
 */
public class BuffBase : PoolableObject
{
	private StatBase BuffTarget;

	[SerializeField]
	private bool bInfinityDuration = false;

	[SerializeField]
	private float Duration = 1.0f;

	[SerializeField]
	private EStatType ModifierStat;

	[SerializeField]
	private EBuffOperator ModifyOperator;

	/// <summary>
	/// ModifyOperator의 반대 연산자
	/// </summary>
	private EBuffOperator NegativeOperator;

	[SerializeField]
	private float ModifyAmount;



	public BuffBase() : base()
	{
		InternalObjectType = EObjectType.BUFF;
	}



	public void SetBuffTarget(StatBase NewBuffTarget)
	{
		BuffTarget = NewBuffTarget;
	}



	public override void OnPreSpawnedFromPool()
	{
		base.OnPreSpawnedFromPool();

		SetNegativeOperator();
	}



	public override void OnPostSpawnedFromPool()
	{
		if (BuffTarget != null)
		{
			AddBuffToTarget();
		}
		else
		{
			Debug.LogWarning("버프 " + gameObject.name + "의 대상이 없어, 바로 오브젝트 풀로 돌아감");
			ReturnToPool();
		}
	}



	public override void OnReturnedToPool()
	{
		if (BuffTarget != null)
		{
			RemoveBuffFromTarget();
		}

		BuffTarget = null;
	}



	private void SetNegativeOperator()
	{
		switch (ModifyOperator)
		{
			case EBuffOperator.ADD:
				NegativeOperator = EBuffOperator.SUB;
				break;
			case EBuffOperator.SUB:
				NegativeOperator = EBuffOperator.ADD;
				break;
			case EBuffOperator.MULTIPLY:
				NegativeOperator = EBuffOperator.DIVIDE;
				break;
			case EBuffOperator.DIVIDE:
				NegativeOperator = EBuffOperator.MULTIPLY;
				break;
			default:
				break;
		}
	}



	private void AddBuffToTarget()
	{
		BuffTarget.BuffList.Add(this);

		switch (ModifierStat)
		{
			case EStatType.STRENGTH:
				BuffTarget.SetStrength(GetModifiedStatValue(BuffTarget.GetStrength()));
				break;
			case EStatType.ATTACK_SPEED:
				BuffTarget.SetAttackSpeed(GetModifiedStatValue(BuffTarget.GetAttackSpeed()));
				break;
			case EStatType.DEFENSE:
				BuffTarget.SetDefense(GetModifiedStatValue(BuffTarget.GetDefense()));
				break;
			case EStatType.HEALTH:
				BuffTarget.SetHealth(GetModifiedStatValue(BuffTarget.GetHealth()));
				break;
			case EStatType.MAX_HEALTH:
				BuffTarget.SetMaxHealth(GetModifiedStatValue(BuffTarget.GetMaxHealth()));
				break;
			case EStatType.MOVE_SPEED:
				BuffTarget.SetMoveSpeed(GetModifiedStatValue(BuffTarget.GetMoveSpeed()));
				break;
			default:
			case EStatType.NONE:
				break;
		}

		// 지속시간이 무한이 아닌 경우, 버프 지속시간 돌리기
		if (!bInfinityDuration)
			StartCoroutine(ReturnToPool(Duration));
	}



	private void RemoveBuffFromTarget()
	{
		switch (ModifierStat)
		{
			case EStatType.STRENGTH:
				BuffTarget.SetStrength(GetModifiedStatValue(BuffTarget.GetStrength(), true));
				break;
			case EStatType.ATTACK_SPEED:
				BuffTarget.SetAttackSpeed(GetModifiedStatValue(BuffTarget.GetAttackSpeed(), true));
				break;
			case EStatType.DEFENSE:
				BuffTarget.SetDefense(GetModifiedStatValue(BuffTarget.GetDefense(), true));
				break;
			case EStatType.HEALTH:
				BuffTarget.SetHealth(GetModifiedStatValue(BuffTarget.GetHealth(), true));
				break;
			case EStatType.MAX_HEALTH:
				BuffTarget.SetMaxHealth(GetModifiedStatValue(BuffTarget.GetMaxHealth(), true));
				break;
			case EStatType.MOVE_SPEED:
				BuffTarget.SetMoveSpeed(GetModifiedStatValue(BuffTarget.GetMoveSpeed(), true));
				break;
			default:
			case EStatType.NONE:
				break;
		}

		BuffTarget.BuffList.Remove(this);
	}



	private float GetModifiedStatValue(float OriginValue, bool bUseNegativeOperator = false)
	{
		switch (bUseNegativeOperator ? NegativeOperator : ModifyOperator)
		{
			case EBuffOperator.ADD:
				return OriginValue + ModifyAmount;
			case EBuffOperator.SUB:
				return OriginValue - ModifyAmount;
			case EBuffOperator.MULTIPLY:
				return OriginValue * ModifyAmount;
			case EBuffOperator.DIVIDE:
				return OriginValue / ModifyAmount;
			default:
				return OriginValue;
		}
	}
}
