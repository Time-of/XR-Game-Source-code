using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief "불굴" 보스 몬스터 애니메이션 클래스
 * @since 2023-06-10
 */
public class BossAnim_Fortitude : ABossAnimationBase
{
	private Boss_Fortitude Fortitude;



	protected override void Awake()
	{
		base.Awake();

		Fortitude = OwnerBossEnemy.GetComponent<Boss_Fortitude>();
	}



	public void Event_StartRotateToFacePlayer()
	{
		Fortitude.TryLookAtPlayer(true);
	}



	public void Event_EndRotateToFacePlayer()
	{
		Fortitude.TryLookAtPlayer(false);
	}



	public void Event_BeginAttack()
	{
		Fortitude.OnBeginAttack();
	}



	public void Event_EndAttack()
	{
		Fortitude.OnEndAttack();
	}
}
