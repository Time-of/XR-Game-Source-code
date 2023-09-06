using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 레벨의 방에 배치되는 문 오브젝트 클래스입니다.
 * @details 무조건 방 안쪽을 바라보게 배치할 것 !!!
 * @since 2023-04-17
 */
public class LevelDoorObject : MonoBehaviour
{
	[ReadOnlyProperty]
	public LevelDoor DoorRef;

	// 문의 목적지 위치
	[SerializeField, ReadOnlyProperty]
	private Vector3 DoorDestinationPosition;

	[SerializeField, ReadOnlyProperty]
	private Quaternion DoorDestinationRotation;

	[ReadOnlyProperty]
	public bool bIsLocked = true;

	[SerializeField, ReadOnlyProperty]
	private LevelRoom CurrentRoom;



	public void UpdateDestination(LevelRoom ThisLevelRoom)
	{
		CurrentRoom = ThisLevelRoom;

		StartCoroutine(UpdateDestinationCoroutine());
	}



	private IEnumerator UpdateDestinationCoroutine()
	{
		yield return null;

		Transform DestinationTransform = DoorRef.ConnectedRoom.Room.GetDoorByDestination(CurrentRoom).transform;
		DoorDestinationPosition = DestinationTransform.position + DestinationTransform.forward;
		DoorDestinationRotation = DestinationTransform.rotation;
	}



	public void Interact()
	{
		Interact(GameplayHelperLibrary.GetPlayer().gameObject);
	}



	public void Interact(GameObject InteractingObject)
	{
		if (DoorRef == null || DoorRef.ConnectedRoom.Room == null)
		{
			Debug.LogError("문 오브젝트 " + name + "의 LevelDoor가 유효하지 않습니다!!");
			return;
		}

		if (bIsLocked) return;

		CurrentRoom.SetActiveNearbyRooms(false);
		DoorRef.ConnectedRoom.Room.SetActiveNearbyRooms(true);

		CharacterController CC = InteractingObject.GetComponent<CharacterController>();

		if (CC != null) { CC.enabled = false; }
		InteractingObject.transform.position = DoorDestinationPosition;
		if (CC != null) { CC.enabled = true; }

		// 방 방문 시 기능을 호출
		DoorRef.ConnectedRoom.Room.OnVisitRoom();
	}
}
