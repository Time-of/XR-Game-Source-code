using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 피해를 받을 수 있는 인터페이스 클래스입니다.
 * @since 2023-03-12
 */
public interface IDamageable
{
	/// <summary>
	/// 피해를 받습니다.
	/// </summary>
	/// <param name="Damage">피해량</param>
	/// <param name="Instigator">피해를 준 주체 (수류탄인 경우, 수류탄을 던진 객체)</param>
	/// <param name="DamageCauser">피해를 준 직접적인 대상 (수류탄인 경우, 수류탄)</param>
	public abstract void TakeDamage(float Damage, GameObject Instigator, GameObject DamageCauser);
}
