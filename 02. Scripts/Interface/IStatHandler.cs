using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * @author 이성수
 * @brief 스탯 관리 인터페이스입니다.
 * @since 2023-03-21
 */
public interface IStatHandler
{
	public abstract UnityEvent<EStatType, float> GetOnAnyStatChangedEvent();

	public abstract void OpenObjectInfoUI();

	public abstract float GetStat(EStatType StatType);

	public abstract GameObject GetOwner();

	public abstract void SetStat(EStatType StatType, float Value);
}
