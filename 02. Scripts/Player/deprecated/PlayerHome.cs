using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 지켜야 할 오브젝트 클래스입니다. (게임 목표, 부숴질 경우 패배)
 * @since 2023-03-20
 */
[RequireComponent(typeof(StatBase))]
public class PlayerHome : MonoBehaviour, IDamageable
{
	private StatBase StatComp;

	private bool bIsDead = false;



	void Start()
	{
		StatComp = GetComponent<StatBase>();
		StatComp.InitializeInitStats(0.0f, 0.0f, 50.0f, 1000.0f, 0.0f);
		StatComp.InitializeStats();
		StatComp.HealthChangedEvent.AddListener(OnHealthChanged);
	}



	void IDamageable.TakeDamage(float Damage, GameObject Instigator, GameObject DamageCauser)
	{
		float CalculatedDamage = GameplayHelperLibrary.CalculateDefense(Damage, StatComp);

		StatComp.SetHealth(StatComp.GetHealth() - CalculatedDamage);
	}



	private void OnHealthChanged(float NewHealth)
	{
		if (!bIsDead && NewHealth <= 0.0f)
		{
			bIsDead = true;
			GameManager.instance.Defeat();
		}
	}
}
