using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using Unity.XR.Oculus;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;



/**
 * @author 이성수
 * @brief 플레이어 캐릭터를 조종하는 클래스입니다.
 * @since 2023-03-16
 */
public class PlayerController : MonoBehaviour, IDamageable
{
	private PlayerCharacterMovement CharacterMovementComponent;

	private IMoveable MovementComponent;

	[SerializeField]
	private GameObject MovementBox;

	[Header("디바이스 세팅")]
	[SerializeField]
	private XRNode LeftHandInputDeviceType = XRNode.LeftHand;

	[SerializeField]
	private XRNode RightHandInputDeviceType = XRNode.RightHand;

	private Vector2 InputAxisValue;

	//private CustomSnapTurnProvider SnapTurnProviderComp;

	//private bool bIndexTouching;

	//[SerializeField]
	//private float HeadsetMovementInputThreshold = 0.2f;

	private InputDevice LeftHandDevice;

	private InputDevice RightHandDevice;

	[SerializeField]
	private XROrigin Origin;

	public bool bRightHandPrimaryButtonPressed;

	private StatBase StatComp;

	private Animator AnimComp;

	[SerializeField]
	private UnityEngine.UI.Image HealthBarImage;

	private bool bLockedOnCamera = false;

	[SerializeField, ReadOnlyProperty]
	private Transform LockOnTarget;

	private EnemyCharacterBase LockOnEnemy;

	[SerializeField]
	private float LockOnRotationSpeed = 3.5f;

	[SerializeField]
	private float LockOnDistance = 15.0f;

	[SerializeField]
	private float LockOnRadius = 5.0f;

	private bool bLeftAxisClicked;

	[SerializeField]
	private GameObject LockOnImageInstance;



	private void Awake()
	{
		StatComp = GetComponent<StatBase>();
    }



	private void Start()
	{
		float[] StatInfo = ActorStatCache.instance.CharacterStatDictionary["Player"];
		StatComp.InitializeInitStats(StatInfo[0], 0.0f, StatInfo[1], StatInfo[2], StatInfo[3]);
		StatComp.InitializeStats();

		CharacterMovementComponent = GetComponent<PlayerCharacterMovement>();
		ChangeMovementMethod(CharacterMovementComponent);

		// 컨트롤러는 상당히 지연된 후 인식되므로 이런 방식을 사용
		StartCoroutine(TryGetXrDevices());
		Origin = GetComponent<XROrigin>();
		//SnapTurnProviderComp = GetComponent<CustomSnapTurnProvider>();
		AnimComp = GetComponent<Animator>();


        OnMoveSpeedChanged(StatComp.GetMoveSpeed());

		StatComp.HealthChangedEvent.AddListener(OnHealthChanged);
		StatComp.MoveSpeedChangedEvent.AddListener(OnMoveSpeedChanged);
	}



	private IEnumerator TryGetXrDevices()
	{
		while (!LeftHandDevice.isValid)
		{
			LeftHandDevice = InputDevices.GetDeviceAtXRNode(LeftHandInputDeviceType);

			yield return new WaitForSeconds(0.1f);
		}

		while (!RightHandDevice.isValid)
		{
			RightHandDevice = InputDevices.GetDeviceAtXRNode(RightHandInputDeviceType);

			yield return new WaitForSeconds(0.1f);
		}
	}



	private void Update()
	{
		LeftHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out InputAxisValue);
		RightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bRightHandPrimaryButtonPressed);

		// 중복 실행 방지
		bool bOldLeftAxisClicked = bLeftAxisClicked;

		LeftHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bLeftAxisClicked);

		if (bLeftAxisClicked && bLeftAxisClicked != bOldLeftAxisClicked)
		{
			ToggleLockOn();
		}

		AnimComp.SetFloat("Forward", InputAxisValue.x);
		AnimComp.SetFloat("Right", InputAxisValue.y);

		if (bLockedOnCamera && LockOnTarget != null && LockOnTarget.gameObject.activeSelf)
		{
			Quaternion LookAtRot = Quaternion.Euler(
				0,
				Quaternion.LookRotation(
					LockOnTarget.transform.position - transform.position, Vector3.up
					).eulerAngles.y,
				0
				);

			transform.rotation = Quaternion.Slerp(transform.rotation,
				LookAtRot, Time.deltaTime * LockOnRotationSpeed);
		}
    }



	private void FixedUpdate()
	{
		MoveCharacter();
	}



	private void MoveCharacter()
	{
		if (InputAxisValue == Vector2.zero) return;

		Vector3 CameraEuler = Origin.Camera.transform.eulerAngles;
		Vector3 MovementInput = new Vector3(InputAxisValue.x, 0.0f, InputAxisValue.y);

		// 이동은 카메라 Yaw 값이 기준이 되도록 한다.
		Quaternion CameraYaw = Quaternion.Euler(0.0f, CameraEuler.y, 0.0f);

		Vector3 Direction = CameraYaw * MovementInput;

		MovementComponent.Move(Direction);
		MovementBox.transform.rotation = CameraYaw;
    }



	private void ToggleLockOn()
	{
		Debug.Log("록온 시도!");
		bLockedOnCamera = !bLockedOnCamera;

		if (bLockedOnCamera)
		{
			RaycastHit[] HitInfos = Physics.SphereCastAll(
				Origin.Camera.transform.position,
				LockOnRadius,
				Origin.Camera.transform.forward,
				LockOnDistance,
				1 << LayerMask.NameToLayer("Hostile")
				);

			int Length = HitInfos.Length;
			if (Length > 0)
			{
				Transform SelectedTarget = null;
				float MinDistSqr = 10000000.0f;

				foreach (var HitInfo in HitInfos)
				{
					float DistSqr = (HitInfo.transform.position - transform.position).sqrMagnitude;

					if (MinDistSqr > DistSqr)
					{
						SelectedTarget = HitInfo.transform;
						MinDistSqr = DistSqr;
					}
				}

				LockOnTarget = SelectedTarget;

				LockOnEnemy = LockOnTarget.GetComponent<EnemyCharacterBase>();

				if (LockOnEnemy != null)
				{
					LockOnEnemy.OnEnemyDiedEvent.AddListener(TryLockOnWhenEnemyDied);
				}

				LockOnImageInstance.transform.parent = SelectedTarget.transform;
				LockOnImageInstance.transform.localPosition = Vector3.zero;
				LockOnImageInstance.SetActive(true);

				Debug.Log("록온 성공!");
			}
			else
			{
				Debug.Log("록온 실패! 걸린 수: " + Length);
			}

			if (LockOnTarget == null)
			{
				bLockedOnCamera = false;

				LockOnImageInstance.transform.parent = transform;
				LockOnImageInstance.SetActive(false);
			}
		}
		else
		{
			LockOnTarget = null;

			LockOnImageInstance.transform.parent = transform;
			LockOnImageInstance.SetActive(false);
		}
	}



	private void TryLockOnWhenEnemyDied()
	{
		LockOnEnemy?.OnEnemyDiedEvent.RemoveListener(TryLockOnWhenEnemyDied);

		LockOnEnemy = null;
		LockOnTarget = null;

		bLockedOnCamera = false;

		LockOnImageInstance.transform.parent = transform;
		LockOnImageInstance.SetActive(false);

		StartCoroutine(TryLockOnCoroutine());
	}



	private IEnumerator TryLockOnCoroutine()
	{
		yield return new WaitForSeconds(0.25f);

		ToggleLockOn();
	}



	public float GetCapsuleHalfHeight()
	{
		return CharacterMovementComponent.GetCapsuleHalfHeight();
	}



	#region 이동수단 관련
	public void ChangeMovementMethod(IMoveable NewMovement)
	{
		if (MovementComponent != null) MovementComponent.OnMovementMethodReleased(this);

		MovementComponent = NewMovement;
		MovementComponent.OnMovementMethodSet(this);
	}



	public void ChangeMovementToCharacterMovement()
	{
		ChangeMovementMethod(CharacterMovementComponent);
	}



	public PlayerCharacterMovement GetPlayerCharacterMovement()
	{
		return CharacterMovementComponent;
	}
	#endregion



	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(Origin.Camera.transform.position, LockOnRadius);
		Gizmos.DrawWireSphere(Origin.Camera.transform.position + Origin.Camera.transform.forward * LockOnDistance, LockOnRadius);
	}



	#region 스탯 관련
	private void OnHealthChanged(float NewHealth)
	{
		if (NewHealth <= 0.0f)
		{
			Debug.Log("<color=red>플레이어 사망!!!!</color>");

			GameManager.instance.Defeat();
		}

		HealthBarImage.fillAmount = NewHealth / StatComp.GetMaxHealth();
	}



	private void OnMoveSpeedChanged(float NewMoveSpeed)
	{
		CharacterMovementComponent.MaxWalkSpeed = NewMoveSpeed;
	}



	public float GetStrength()
	{
		return StatComp.GetStrength();
	}



	public float GetAttackSpeed()
	{
		return StatComp.GetAttackSpeed();
	}
	#endregion



	#region 햅틱 피드백
	public void SendHapticToHand(bool bIsLeftHand, float Amplitude, float Duration)
	{ 
		if (bIsLeftHand)
		{
			LeftHandDevice.SendHapticImpulse(0, Amplitude, Duration);
		}
		else
		{
			RightHandDevice.SendHapticImpulse(0, Amplitude, Duration);
		}
	}
	#endregion



	public Camera GetCamera()
	{
		return Origin.Camera;
	}



	public Quaternion GetCameraRotation()
	{
		return Origin.Camera.transform.rotation;
	}



	public void TakeDamage(float Damage, GameObject Instigator, GameObject DamageCauser)
	{
		float CalculatedDamage = GameplayHelperLibrary.CalculateDefense(Damage, StatComp);
		StatComp.SetHealth(StatComp.GetHealth() - CalculatedDamage);
	}
}
