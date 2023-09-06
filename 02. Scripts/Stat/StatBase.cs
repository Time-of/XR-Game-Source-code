using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



[System.Serializable]
public enum EStatType
{
	NONE, STRENGTH, ATTACK_SPEED, DEFENSE, HEALTH, MAX_HEALTH, MOVE_SPEED
}



/**
 * @author 이성수
 * @brief 능력치 베이스 클래스입니다.
 * @details Internal이 안 붙은 스탯들은 기본값, Internal이 붙은 스탯들은 실제 능력치 값
 * @since 2023-03-20
 */
public class StatBase : MonoBehaviour, IStatHandler
{
	[ReadOnlyProperty]
	public List<BuffBase> BuffList = new List<BuffBase>();

	private float Strength;
	private float AttackSpeed;
	private float Defense;
	private float MaxHealth;
	private float MoveSpeed;

	// 내부적으로 사용되는 변수들

	/// <summary>
	/// 공격력, 적용 공식: 피해량 + 공격력
	/// </summary>
	[SerializeField, ReadOnlyProperty]
	private float InternalStrength;
	/// <summary>
	/// 공격속도, 적용 공식: (1500 + 공격속도) / 1500
	/// </summary>
	[SerializeField, ReadOnlyProperty]
	private float InternalAttackSpeed;
	/// <summary>
	/// 방어력, 피해량 적용 공식: 1500 / (방어력 + 1500)
	/// </summary>
	[SerializeField, ReadOnlyProperty]
	private float InternalDefense;
	[SerializeField, ReadOnlyProperty]
	private float InternalMaxHealth;
	[SerializeField, ReadOnlyProperty]
	private float InternalHealth;
	[SerializeField, ReadOnlyProperty]
	private float InternalMoveSpeed;

	public UnityEvent<EStatType, float> AnyStatChangedEvent = new UnityEvent<EStatType, float>();
	public UnityEvent<float> StrengthChangedEvent = new UnityEvent<float>();
	public UnityEvent<float> AttackSpeedChangedEvent = new UnityEvent<float>();
	public UnityEvent<float> DefenseChangedEvent = new UnityEvent<float>();
	public UnityEvent<float> MaxHealthChangedEvent = new UnityEvent<float>();
	public UnityEvent<float> HealthChangedEvent = new UnityEvent<float>();
	public UnityEvent<float> MoveSpeedChangedEvent = new UnityEvent<float>();



	//public async void LoadInitialStatData(string InternalName)
	//{
		//await ActorStatCache.instance.GetActorStatInfoAsync(InternalName);
	//}



	/// <summary>
	/// 스탯 초기화 기능
	/// </summary>
	public void InitializeStats(float Difficulty = 1.0f)
	{
		InternalStrength = Strength * Difficulty;
		InternalAttackSpeed = AttackSpeed;
		InternalDefense = Defense * Difficulty;
		InternalMaxHealth = MaxHealth * Difficulty;
		InternalHealth = MaxHealth * Difficulty;
		InternalMoveSpeed = MoveSpeed;
	}



	/// <summary>
	/// 파라미터로부터 초기 스탯을 초기화합니다.
	/// </summary>
	/// <param name="Difficulty">난이도 배수</param>
	public void InitializeInitStats(float NewStrength, float NewAttackSpeed, float NewDefense, float NewMaxHealth, float NewMoveSpeed)
	{
		Strength = NewStrength;
		AttackSpeed = NewAttackSpeed;
		Defense = NewDefense;
		MaxHealth = NewMaxHealth;
		MoveSpeed = NewMoveSpeed;
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
				return InternalDefense;
			case EStatType.MAX_HEALTH:
				return InternalMaxHealth;
			case EStatType.HEALTH:
				return InternalHealth;
			case EStatType.MOVE_SPEED:
				return InternalMoveSpeed;

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
				SetDefense(Value);
				break;
			case EStatType.HEALTH:
				SetHealth(Value);
				break;
			case EStatType.MAX_HEALTH:
				SetMaxHealth(Value);
				break;
			case EStatType.MOVE_SPEED:
				SetMoveSpeed(Value);
				break;
			case EStatType.NONE:
			default:
				break;
		}
	}



	public float GetStrength() { return InternalStrength; }
	public float GetAttackSpeed() { return InternalAttackSpeed; }
	public float GetDefense() { return InternalDefense; }
	public float GetMaxHealth() { return InternalMaxHealth; }
	public float GetHealth() { return InternalHealth; }
	public float GetMoveSpeed() { return InternalMoveSpeed; }



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

	public void SetDefense(float NewDefense)
	{ 
		InternalDefense = NewDefense;
		DefenseChangedEvent.Invoke(InternalDefense);
		AnyStatChangedEvent.Invoke(EStatType.DEFENSE, InternalDefense);
	}

	public void SetMaxHealth(float NewMaxHealth) 
	{
		InternalMaxHealth = NewMaxHealth;
		MaxHealthChangedEvent.Invoke(InternalMaxHealth);
		AnyStatChangedEvent.Invoke(EStatType.MAX_HEALTH, InternalMaxHealth);
	}

	public void SetHealth(float NewHealth)
	{
		InternalHealth = Mathf.Clamp(NewHealth, 0.0f, InternalMaxHealth);
		HealthChangedEvent.Invoke(InternalHealth);
		AnyStatChangedEvent.Invoke(EStatType.HEALTH, InternalHealth);
	}

	public void SetMoveSpeed(float NewMoveSpeed) 
	{
		InternalMoveSpeed = NewMoveSpeed;
		MoveSpeedChangedEvent.Invoke(InternalMoveSpeed);
		AnyStatChangedEvent.Invoke(EStatType.MOVE_SPEED, InternalMoveSpeed);
	}
}
