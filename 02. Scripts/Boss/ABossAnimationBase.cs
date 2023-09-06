using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using Photon.Pun;



/**
 * (이전에 했던 프로젝트에서 가져옴)
 * 작성자: 20181220 이성수
 * 보스 몬스터의 애니메이션 베이스입니다.
 */
public abstract class ABossAnimationBase : MonoBehaviour
{
	protected ABossBase OwnerBossEnemy;

	protected Animator AnimComponent;

	public Animator GetAnimator() { return AnimComponent; }

	protected NavMeshAgent NavAgentComponent;


	
	//[Header("네트워킹")]

	//[SerializeField, ReadOnlyProperty]
	//protected bool bIsMine = false;

	//[SerializeField, ReadOnlyProperty]
	//private float NetworkingMoveSpeed = 0.0f;



	protected virtual void Awake()
	{
		OwnerBossEnemy = GetComponentInParent<ABossBase>();

		AnimComponent = GetComponent<Animator>();

		NavAgentComponent = GetComponentInParent<NavMeshAgent>();

		//bIsMine = OwnerBossEnemy.photonView.IsMine;
	}



	protected virtual void Update()
	{
		Vector3 VelocityXZ = NavAgentComponent.velocity;
		VelocityXZ.y = 0;

		AnimComponent.SetFloat("SpeedSqr", VelocityXZ.sqrMagnitude);
		AnimComponent.SetFloat("Direction", CalculateDirection(VelocityXZ, OwnerBossEnemy.transform.rotation));
	}



	#region 이벤트
	public void Event_AttackStart()
	{
		OwnerBossEnemy.OnBeginAction();
	}



	public void Event_AttackEnd()
	{
		OwnerBossEnemy.OnEndAction();
	}



	public void Event_EnableParry()
	{
		OwnerBossEnemy.SetCanBeParried(true);
	}



	public void Event_DisableParry()
	{
		OwnerBossEnemy.SetCanBeParried(false);
	}



	public void Event_SpawnSound(AudioClip Sfx)
	{
		SoundHelperLibrary.SpawnSoundAtLocation(transform.position, Quaternion.identity, Sfx, false, 1.0f, 0.05f);
	}
	#endregion



	protected void OnAnimatorMove()
	{
		OwnerBossEnemy.transform.position += AnimComponent.deltaPosition;
	}



	protected float CalculateDirection(Vector3 Velocity, Quaternion CurrentRotation)
	{
		if (Velocity == Vector3.zero) return 0.0f;

		float SideCheck = Vector3.Dot(Vector3.up, Vector3.Cross(transform.forward, Velocity));

		return Quaternion.Angle(Quaternion.LookRotation(Velocity, Vector3.up), CurrentRotation) * ((SideCheck >= 0.0f) ? 1 : -1);
	}
}
