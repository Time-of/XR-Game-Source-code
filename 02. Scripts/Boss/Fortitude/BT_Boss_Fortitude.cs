using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;



/**
 * @author 이성수
 * @brief "불굴" 보스 비헤이비어 트리 클래스
 * @since 2023-06-10
 */
public class BT_Boss_Fortitude : BT_ATree
{
	protected override void SetupBlackboardData()
	{
		SetData("IsInAction", false);

		SetData("bIsWaiting", false);

		SetData("NearestPlayer", null);
		SetData("DistSqrToPlayer", 10000000.0f);
	}



	protected override BT_NodeBase SetupRootNode()
	{
		BT_NodeBase NewRoot = new BT_Selector(this, new List<BT_NodeBase>
		{
			new BTTask_FortitudeTryAttackByDistance(this),
			new BTTask_MoveToNearestPlayer(this, GetComponent<ABossBase>())
		}).AttachServices(new List<BT_Service>
		{
			new BTService_UpdateDist(this)
		});

		return NewRoot;
	}
}
