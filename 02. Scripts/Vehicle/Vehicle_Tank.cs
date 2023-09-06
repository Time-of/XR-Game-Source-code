using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/**
 * @author 이성수
 * @brief Rigidbody 기반으로 이동하는 탱크 탈것입니다.
 * @since 2023-03-16
 */
public class Vehicle_Tank : PoolableObject, IMoveable
{
	[SerializeField]
	private float MoveSpeed = 2.0f;

	[SerializeField]
	private float RotationSpeed = 10.0f;

	[SerializeField]
	private float TurretRotationSpeed = 7.5f;

	[SerializeField]
	private Transform PlayerSeat;

	private Rigidbody RigidComp;

	private bool bMoveForwardStickEnabled;

	private bool bRotationStickEnabled;

	private bool bTurretControlStickEnabled;

	[SerializeField]
	private TankTurret TankTurretComp;

	private float TankTurretRotationYaw;

	[SerializeField]
	private HingeJoint MoveForwardDriveStick;

	private float MoveForwardStickValue;

	[SerializeField]
	private HingeJoint RotationDriveStick;

	[SerializeField]
	private HingeJoint TurretControlStick;

	private float TurretControlStickValue;

	[SerializeField]
	private LineRenderer TurretLineRenderer;

	private Animator AnimComp;

	[SerializeField]
	private AudioClip TankEngineSfx;

	[SerializeField]
	private AudioClip TankMoveSfx;

	[SerializeField]
	private AudioClip TurretRotationSfx;

	private PoolableSound TankEngineSoundRef;

	private PoolableSound TankMoveSoundRef;

	private PoolableSound TurretRotationSoundRef;

	// 내린 후 일정 시간동안 탑승 불가
	bool bCannotTake = false;



	public Vehicle_Tank() : base()
	{

	}



	private void Start()
	{
		RigidComp = GetComponent<Rigidbody>();
		AnimComp = GetComponentInChildren<Animator>();
		TankTurretComp.RigidComp.ResetCenterOfMass();
	}



	private void FixedUpdate()
	{
		if (bMoveForwardStickEnabled)
		{
			MoveForwardStickValue = MathHelperLibrary.MapRangeClamped(MoveForwardDriveStick.angle,
					-45.0f, 45.0f, -1.0f, 1.0f);

			RigidComp.MovePosition(RigidComp.position +
				MoveForwardStickValue * MoveSpeed * Time.fixedDeltaTime * transform.forward);

			TankMoveSoundRef?.SetSoundVolume(Mathf.Abs(MoveForwardStickValue));
		}
		else
		{
			if (MoveForwardStickValue > 0.9f) AnimComp.SetTrigger("MoveStop");

			TankMoveSoundRef?.SetSoundVolume(0.0f);
			MoveForwardStickValue = 0.0f;
		}

		AnimComp.SetFloat("MoveForwardAxis", MoveForwardStickValue, 10.0f * Time.deltaTime, Time.deltaTime);

		if (bRotationStickEnabled)
		{
			float RotationInput = RotationDriveStick.angle;

			RigidComp.MoveRotation(RigidComp.rotation *
				Quaternion.Euler(0.0f,
					MathHelperLibrary.MapRangeClamped(RotationInput,
						-45.0f, 45.0f, -1.0f, 1.0f) * RotationSpeed * Time.fixedDeltaTime,
						0.0f));
		}

		if (bTurretControlStickEnabled)
		{
			TurretControlStickValue = MathHelperLibrary.MapRangeClamped(TurretControlStick.angle,
						-45.0f, 45.0f, -1.0f, 1.0f);

			TankTurretRotationYaw += TurretControlStickValue
					 * TurretRotationSpeed * Time.fixedDeltaTime;

			TankMoveSoundRef?.SetSoundVolume(Mathf.Abs(TurretControlStickValue));
		}

		TankTurretComp.RigidComp.rotation = RigidComp.rotation * Quaternion.Euler(0.0f, TankTurretRotationYaw, 0.0f);
	}



	public override void OnPostSpawnedFromPool()
	{
		TankTurretRotationYaw = RigidComp.rotation.eulerAngles.y;
	}



	public override void OnReturnedToPool()
	{
		TankEngineSoundRef?.ReturnToPool();
		TankMoveSoundRef?.ReturnToPool();
		TurretRotationSoundRef?.ReturnToPool();
	}



	void IMoveable.Move(Vector3 InputAxisValue)
	{
		//RigidComp.velocity = MoveSpeed * Time.fixedDeltaTime * InputAxisValue;
	}



	void IMoveable.OnMovementMethodReleased(PlayerController Player)
	{
		Player.GetPlayerCharacterMovement().SetAppliedGravityToDefault();
		Player.transform.SetParent(null);

		//StopAllCoroutines();
	}



	void IMoveable.OnMovementMethodSet(PlayerController Player)
	{
		Player.GetPlayerCharacterMovement().AppliedGravity = 0.0f;
		Player.transform.position = PlayerSeat.position;
		Player.transform.SetParent(PlayerSeat, true);
		Player.transform.rotation = Quaternion.Euler(0.0f, TankTurretComp.RigidComp.rotation.eulerAngles.y, 0.0f);
	}



	public void TakeTank()
	{
		if (bCannotTake) return;
		
		GameplayHelperLibrary.GetPlayer().ChangeMovementMethod(this);

		TankEngineSoundRef?.ReturnToPool();
		TankMoveSoundRef?.ReturnToPool();
		TurretRotationSoundRef?.ReturnToPool();

		TankEngineSoundRef = SoundHelperLibrary.SpawnSoundAttached(transform.position, transform.rotation,
			TankEngineSfx, transform, 1.0f, 0.0f, true);

		TankMoveSoundRef = SoundHelperLibrary.SpawnSoundAttached(transform.position, transform.rotation,
			TankMoveSfx, transform, 0.0f, 0.0f, true);

		TurretRotationSoundRef = SoundHelperLibrary.SpawnSoundAttached(TankTurretComp.transform.position, TankTurretComp.transform.rotation,
			TurretRotationSfx, TankTurretComp.transform, 0.0f, 0.0f, true);

		TurretLineRenderer.enabled = true;
	}



	public void LeaveTank()
	{
		StartCoroutine(ResetCannotTakeCoroutine());

		GameplayHelperLibrary.GetPlayer().ChangeMovementToCharacterMovement();

		// 사운드 모두 반납하기
		OnReturnedToPool();

		TurretLineRenderer.enabled = false;
	}



	private IEnumerator ResetCannotTakeCoroutine()
	{
		bCannotTake = true;
		yield return new WaitForSeconds(2.0f);
		bCannotTake = false;
	}



	public void SetMoveForwardStickEnabled(bool NewEnabled)
	{
		bMoveForwardStickEnabled = NewEnabled;

		TankMoveSoundRef?.SetSoundVolume(0.0f);
	}



	public void SetRotationStickEnabled(bool NewEnabled)
	{
		bRotationStickEnabled = NewEnabled;
	}



	public void SetTurretControlStickEnabled(bool NewEnabled)
	{
		bTurretControlStickEnabled = NewEnabled;

		TurretRotationSoundRef?.SetSoundVolume(0.0f);
	}



	public void FireTurret()
	{
		if (bTurretControlStickEnabled)
		{
			if (TankTurretComp.TryFire()) AnimComp.SetTrigger("Fire");
		}
	}



	/// <summary>
	/// 스틱 스케일이 자꾸 늘어나는 버그 있어서 사용
	/// </summary>
	public void ForceSetStickScaleToOne(GameObject StickObject)
	{
		StickObject.transform.localScale = Vector3.one;
	}
}
