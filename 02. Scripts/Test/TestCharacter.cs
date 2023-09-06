using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 기능 테스트용 캐릭터
 * @since 2023-04-18
 */
public class TestCharacter : MonoBehaviour
{
	private PlayerCharacterMovement CharacterMovementComponent;

	private IMoveable MovementComponent;

	private Vector2 InputAxisValue;
	
	[SerializeField]
	private Transform CameraRig;



	private void Start()
	{
		CharacterMovementComponent = GetComponent<PlayerCharacterMovement>();
		ChangeMovementMethod(CharacterMovementComponent);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}



	private void Update()
	{
		InputAxisValue.y = Input.GetAxis("Vertical");
		InputAxisValue.x = Input.GetAxis("Horizontal");
		float MouseX = Input.GetAxis("Mouse X");
		float MouseY = -Input.GetAxis("Mouse Y");

		if (Input.GetKeyDown(KeyCode.E)) TryInteract();

		Vector3 CamRot = CameraRig.transform.eulerAngles;
		CamRot.y += 1.6f * MouseX;
		CamRot.x += MouseY;
		CameraRig.transform.eulerAngles = CamRot;
	}



	private void FixedUpdate()
	{
		MoveCharacter();
	}



	public void ChangeMovementMethod(IMoveable NewMovement)
	{
		//if (MovementComponent != null) MovementComponent.OnMovementMethodReleased(this);

		MovementComponent = NewMovement;
		//MovementComponent.OnMovementMethodSet(this);
	}



	private void MoveCharacter()
	{
		if (InputAxisValue == Vector2.zero) return;

		Vector3 CameraEuler = CameraRig.eulerAngles;
		Vector3 MovementInput = new Vector3(InputAxisValue.x, 0.0f, InputAxisValue.y);

		// 이동은 카메라 Yaw 값이 기준이 되도록 한다.
		Quaternion CameraYaw = Quaternion.Euler(0.0f, CameraEuler.y, 0.0f);

		Vector3 Direction = CameraYaw * MovementInput;

		MovementComponent.Move(Direction);
	}



	private Vector3 from = Vector3.zero;
	private Vector3 to = Vector3.zero;
	private Color DrawColor = Color.red;
	private float InteractRadius = 0.15f;

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.color = DrawColor;
		Gizmos.DrawWireSphere(from, InteractRadius);
		Gizmos.DrawWireSphere(to, InteractRadius);
	}
#endif



	private void TryInteract()
	{
		Physics.SphereCast(CameraRig.position, InteractRadius, CameraRig.forward, out RaycastHit HitInfo, 3.0f);

		from = CameraRig.position;
		to = CameraRig.position + CameraRig.forward * 3.0f;
		DrawColor = HitInfo.collider != null ? Color.green : Color.red;

		if (HitInfo.collider == null) return;

		if (HitInfo.collider.CompareTag("Door"))
		{
			LevelDoorObject DoorObj = HitInfo.collider.gameObject.GetComponent<LevelDoorObject>();

			if (DoorObj != null)
			{
				DoorObj.Interact(this.gameObject);
			}
		}
	}
}
