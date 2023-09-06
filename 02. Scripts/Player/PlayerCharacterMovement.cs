using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;



/**
 * @author 이성수
 * @brief 플레이어의 XR을 사용한 움직임 컴포넌트 클래스입니다.
 * @since 2023-03-14
 */
public class PlayerCharacterMovement : MonoBehaviour, IMoveable
{
	[SerializeField]
	private float AdditionalCharacterHeight = 0.2f;

	[Header("캐릭터 움직임 관련")]
	[HideInInspector]
	public float MaxWalkSpeed = 3.0f;

	public Vector3 WalkVelocity { get; private set; }

	[SerializeField]
	private float WalkAcceleration = 0.15f;

	[SerializeField]
	private float GroundFriction = 5.0f;

	[SerializeField]
	private float Gravity = -0.49f;

	public float AppliedGravity;

	private float FallingSpeed = 0.0f;

	private const float MaxFallingSpeed = -60.0f;

	private bool bIsFalling = false;

	[SerializeField]
	private float GroundCheckOffset = 0.1f;

	[SerializeField]
	private LayerMask GroundLayer;

	[SerializeField]
	private CharacterController CharacterControllerComp;

	private XROrigin Origin;



	private void Start()
	{
		CharacterControllerComp = GetComponent<CharacterController>();
		Origin = GetComponent<XROrigin>();
		
		SetAppliedGravityToDefault();
	}



	public void SetAppliedGravityToDefault()
	{
		AppliedGravity = Gravity;
	}



	private void FixedUpdate()
	{
		AdjustCapsuleHeight();
		CheckIsFalling();
		
		ApplyGravity();

		WalkVelocity = MathHelperLibrary.VLerp(WalkVelocity, Vector3.zero, GroundFriction * Time.fixedDeltaTime);
		
		CharacterControllerComp.Move(WalkVelocity);
	}



	private void ApplyGravity()
	{
		if (!bIsFalling)
		{
			FallingSpeed = 0.0f;
		}
		else
		{
			FallingSpeed = Mathf.Max(FallingSpeed + AppliedGravity * Time.fixedDeltaTime, MaxFallingSpeed);
			CharacterControllerComp.Move(FallingSpeed * Vector3.up);
		}
	}



	private void CheckIsFalling()
	{
		Vector3 Downside = transform.TransformPoint(new Vector3(CharacterControllerComp.center.x, CharacterControllerComp.center.y - (CharacterControllerComp.height - CharacterControllerComp.radius) * 0.5f - CharacterControllerComp.skinWidth + GroundCheckOffset, CharacterControllerComp.center.z));
		bIsFalling = !Physics.CheckSphere(Downside,
			CharacterControllerComp.radius, GroundLayer, QueryTriggerInteraction.Ignore);
	}



	/// <summary>
	/// 현재 카메라 위치에 따라 캡슐의 높이를 동적으로 조절, 그에 따른 센터 위치도 함께 조절
	/// </summary>
	private void AdjustCapsuleHeight()
	{
		if (Origin == null) return;

		CharacterControllerComp.height = Mathf.Max(CharacterControllerComp.radius, Origin.CameraInOriginSpaceHeight) + AdditionalCharacterHeight;

		Vector3 capsuleCenterPos = transform.InverseTransformPoint(Origin.Camera.transform.position);
		capsuleCenterPos.y = CharacterControllerComp.height * 0.5f + CharacterControllerComp.skinWidth;

		CharacterControllerComp.center = capsuleCenterPos;
	}



	void IMoveable.Move(Vector3 InputAxisValue)
	{
		WalkVelocity = MathHelperLibrary.LimitVector(WalkVelocity + InputAxisValue * MaxWalkSpeed * Time.fixedDeltaTime * WalkAcceleration,
				MaxWalkSpeed);
	}



	void IMoveable.OnMovementMethodReleased(PlayerController Player)
	{

	}



	void IMoveable.OnMovementMethodSet(PlayerController Player)
	{

	}



	public float GetCapsuleHalfHeight()
	{
		return CharacterControllerComp.height * 0.5f;
	}
}
