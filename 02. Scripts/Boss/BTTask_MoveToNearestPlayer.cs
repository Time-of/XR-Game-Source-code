using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
using UnityEngine.AI;



/**
 * (이전에 했던 프로젝트에서 가져옴)
 * 작성자: 20181220 이성수
 * 가장 가까운 플레이어가 있다면, 그 플레이어에게 이동하는 비헤이비어 트리 태스크입니다.
 */
public class BTTask_MoveToNearestPlayer : BT_NodeBase
{
	private float ElapsedTime = 0.0f;

	private float TickActionCycle = 1.0f;

	private ABossBase Boss;



	public BTTask_MoveToNearestPlayer(BT_ATree Tree, ABossBase NewBoss) : base(Tree) { Boss = NewBoss;	}



	public override ENodeState Evaluate()
	{
		ElapsedTime += Time.deltaTime;

		if (ElapsedTime >= TickActionCycle)
		{
			ElapsedTime -= TickActionCycle;
			TickActionCycle = Random.Range(0.8f, 1.3f);

			object CharacterData = AttachedTree.GetData("NearestPlayer");

			if (CharacterData != null)
			{
				PlayerController PC = (PlayerController)CharacterData;
				object IsInActionData = AttachedTree.GetData("IsInAction");
				bool bIsInAction = (bool)IsInActionData;

				if (PC != null)
				{
					NavMeshAgent Agent = AttachedTree.GetComponent<NavMeshAgent>();

					object IsWaitingData = AttachedTree.GetData("bIsWaiting");
					bool bIsWaiting = (bool)IsWaitingData;

					if (!bIsInAction)
					{
						Agent.SetDestination(PC.transform.position);
					}
					else
					{
						if (bIsWaiting)
						{
							Agent.updateRotation = false;
							Boss.TryLookAtPlayer(true);
							Agent.isStopped = false;
							Agent.SetDestination(AttachedTree.Tf.position + Quaternion.AngleAxis(Random.Range(-360.0f, 360.0f), Vector3.up) * (Vector3.forward * 5.0f));
						}
						else
						{
							Agent.updateRotation = true;
							Agent.isStopped = true;
						}
					}
				}
			}
			// 캐릭터가 없는 경우
			else
			{
				AttachedTree.GetComponent<NavMeshAgent>().isStopped = true;
			}
		}

		NodeState = ENodeState.RUNNING;

		return NodeState;
	}
}
