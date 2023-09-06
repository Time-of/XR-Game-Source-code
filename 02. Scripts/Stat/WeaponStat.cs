using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * @author 이성수
 * @brief 무기의 능력치 클래스입니다.
 * @since 2023-03-21
 */
public class WeaponStat : MonoBehaviour, IStatHandler
{
	private float Strength = 0.0f;
	private float AttackSpeed = 0.0f;



	// 내부적으로 사용되는 변수들
	[SerializeField, ReadOnlyProperty]
	private float InternalStrength;
	[SerializeField, ReadOnlyProperty]
	private float InternalAttackSpeed;

	public UnityEvent<EStatType, float> AnyStatChangedEvent = new UnityEvent<EStatType, float>();
	public UnityEvent<float> StrengthChangedEvent = new UnityEvent<float>();
	public UnityEvent<float> AttackSpeedChangedEvent = new UnityEvent<float>();



	/// <summary>
	/// 스탯 초기화 기능
	/// </summary>
	public void InitializeStats()
	{
		InternalStrength = Strength;
		InternalAttackSpeed = AttackSpeed;
	}



	/// <summary>
	/// 파라미터로부터 초기 스탯을 초기화합니다.
	/// </summary>
	/// <param name="Difficulty">난이도 배수</param>
	public void InitializeInitStats(float NewStrength, float NewAttackSpeed)
	{
		Strength = NewStrength;
		AttackSpeed = NewAttackSpeed;
	}



	float IStatHandler.GetStat(EStatType StatType)
	{
		switch (StatType)
		{
			case EStatType.STRENGTH:
				return InternalStrength;
			case EStatType.ATTACK_SPEED:
				return InternalAttackSpeed;
			case EStatType.DEFENSE:
			case EStatType.HEALTH:
			case EStatType.MAX_HEALTH:
			case EStatType.MOVE_SPEED:
			case EStatType.NONE:
			default:
				return -1.0f;
		}
	}

	UnityEvent<EStatType, float> IStatHandler.GetOnAnyStatChangedEvent()
	{
		return AnyStatChangedEvent;
	}

	public void OpenObjectInfoUI()
	{
		GameplayUIManager.instance.OpenObjectInfoUI(this);
	}

	GameObject IStatHandler.GetOwner()
	{
		return gameObject;
	}

	void IStatHandler.SetStat(EStatType StatType, float Value)
	{
		switch (StatType)
		{
			case EStatType.STRENGTH:
				SetStrength(Value);
				break;
			case EStatType.ATTACK_SPEED:
				SetAttackSpeed(Value);
				break;
			case EStatType.DEFENSE:
			case EStatType.HEALTH:
			case EStatType.MAX_HEALTH:
			case EStatType.MOVE_SPEED:
			case EStatType.NONE:
			default:
				break;
		}
	}



	public float GetStrength() { return InternalStrength; }
	public float GetAttackSpeed() { return InternalAttackSpeed; }



	public void SetStrength(float NewStrength) 
	{
		InternalStrength = NewStrength;
		StrengthChangedEvent.Invoke(InternalStrength);
		AnyStatChangedEvent.Invoke(EStatType.STRENGTH, InternalStrength);
	}

	public void SetAttackSpeed(float NewAttackSpeed) 
	{
		InternalAttackSpeed = NewAttackSpeed;
		AttackSpeedChangedEvent.Invoke(InternalAttackSpeed);
		AnyStatChangedEvent.Invoke(EStatType.ATTACK_SPEED, InternalAttackSpeed);
	}
}
