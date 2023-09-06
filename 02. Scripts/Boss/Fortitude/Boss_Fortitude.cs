using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
struct FBossFortitudeSkillWeight
{
	[SerializeField]
	public string SkillTriggerName;

	[SerializeField]
	public float Weight;
}



/**
 * @author 이성수
 * @brief "불굴" 보스 몬스터 클래스
 * @since 2023-06-10
 */
public class Boss_Fortitude : ABossBase
{
	[SerializeField]
	private MeleeWeaponTrail TrailComp;

	[SerializeField]
	private List<FBossFortitudeSkillWeight> SuperShortMeleeSkillsList = new();

	[SerializeField]
	private List<FBossFortitudeSkillWeight> ShortMeleeSkillsList = new();

	[SerializeField]
	private List<FBossFortitudeSkillWeight> MiddleShortMeleeSkillsList = new();

	[SerializeField]
	private List<FBossFortitudeSkillWeight> MiddleMeleeSkillsList = new();



	public override void TryAttack(int AttackType)
	{
		if (bIsDead) return;

		OnBeginAction();

		TryLookAtPlayer(true);

		/**
		 * 0: (초근거리) 
		 * 1: (근거리) 
		 * 2: (중근거리) 
		 * 3: (중거리) 
		 */
		switch (AttackType)
		{
			case 0:
				SetTrigger(SelectSkillFromWeight(SuperShortMeleeSkillsList));
				break;
			case 1:
				SetTrigger(SelectSkillFromWeight(ShortMeleeSkillsList));
				break;
			case 2:
				SetTrigger(SelectSkillFromWeight(MiddleShortMeleeSkillsList));
				break;
			case 3:
				SetTrigger(SelectSkillFromWeight(MiddleMeleeSkillsList));
				break;
			default:
				break;
		}

		Debug.Log("<color=yellow>불굴 보스의 공격</color> : " + AttackType);
	}



	private string SelectSkillFromWeight(List<FBossFortitudeSkillWeight> SkillsList)
	{
		float TotalWeight = 0.0f;

		foreach (var Info in SkillsList)
		{
			TotalWeight += Info.Weight;
		}

		if (TotalWeight <= 0.0f) return null;

		float Pivot = Random.Range(0.0f, TotalWeight);

		foreach (var Info in SkillsList)
		{
			if (Pivot < Info.Weight)
			{
				return Info.SkillTriggerName;
			}
			else
			{
				Pivot -= Info.Weight;
			}
		}

		return SkillsList[SkillsList.Count - 1].SkillTriggerName;
	}



	public void OnBeginAttack()
	{
		TrailComp.Emit = true;

		WeaponCollision.enabled = true;
		WeaponCollision.SetCanAttack(true);
	}



	public void OnEndAttack()
	{
		TrailComp.Emit = false;

		WeaponCollision.enabled = false;
		WeaponCollision.SetCanAttack(false);
	}



	public override void OnParried()
	{
		OnEndAttack();

		base.OnParried();
	}
}