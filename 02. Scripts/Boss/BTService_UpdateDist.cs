using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;



/**
 * (이전에 했던 프로젝트에서 가져옴)
 * 작성자: 20181220 이성수
 * @brief 플레이어와의 거리를 재는 서비스 클래스
 */
public class BTService_UpdateDist : BT_Service
{
	public BTService_UpdateDist(BT_ATree Tree) : base(Tree, 1.0f)
	{
		TickAction();
	}



	public override void TickAction()
	{
		PlayerController PlayerCharacter = GameplayHelperLibrary.GetPlayer();

		float DistSqr = (AttachedTree.Tf.position - PlayerCharacter.transform.position).sqrMagnitude;

		// 찾았든 찾지 못했든 저장
		AttachedTree.SetData("NearestPlayer", PlayerCharacter);
		AttachedTree.SetData("DistSqrToPlayer", DistSqr);
	}
}
