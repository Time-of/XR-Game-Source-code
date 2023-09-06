using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;



/**
 * @author 이성수
 * @brief 불굴 보스가 거리에 따라 공격을 시도하는 BTTask입니다.
 * @since 2023-06-10
 */
public class BTTask_FortitudeTryAttackByDistance : BT_NodeBase
{
	public BTTask_FortitudeTryAttackByDistance(BT_ATree Tree) : base(Tree) { }



	public override ENodeState Evaluate()
	{
		object DistData = AttachedTree.GetData("DistSqrToPlayer");
		object IsInActionData = AttachedTree.GetData("IsInAction");

		float DistSqr = (float)DistData;
		bool bCanAtk = !(bool)IsInActionData;

		if (!bCanAtk)
		{
			NodeState = ENodeState.FAILURE;

			return NodeState;
		}

		if (DistSqr <= 5.0f)
		{
			AttachedTree.GetComponent<ABossBase>().TryAttack(0);

			NodeState = ENodeState.SUCCESS;

			return NodeState;
		}
		else if (DistSqr <= 20.0f)
		{
			AttachedTree.GetComponent<ABossBase>().TryAttack(1);

			NodeState = ENodeState.SUCCESS;

			return NodeState;
		}
		else if (DistSqr <= 47.0f)
		{
			AttachedTree.GetComponent<ABossBase>().TryAttack(2);

			NodeState = ENodeState.SUCCESS;

			return NodeState;
		}
		else if (DistSqr <= 80.0f)
		{
			AttachedTree.GetComponent<ABossBase>().TryAttack(3);

			NodeState = ENodeState.SUCCESS;

			return NodeState;
		}


		NodeState = ENodeState.FAILURE;

		return NodeState;
	}
}
