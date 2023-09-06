using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 길이 두 개인 레벨 방 클래스입니다.
 * @since 2023-04-17
 */
public class LevelRoom_TwoPaths : LevelRoom
{
	public LevelDoorObject OtherDoor;

	// 코너라면 true, 직선이라면 false
	[SerializeField]
	private bool bIsCorner = false;



	protected override void OpenDoors()
	{
		base.OpenDoors();

		OtherDoor.bIsLocked = false;
	}



	protected override void InitializeDoors(RoomInfo ThisRoomInfo, int YawRotation)
	{
		base.InitializeDoors(ThisRoomInfo, YawRotation);

		// 코너 맵이라면
		if (bIsCorner)
		{
			// 다른 문은 한 칸 왼쪽 문을 선택 (피벗이 DownDoor라면 OtherDoor은 LeftDoor가 된다)
			OtherDoor.DoorRef = ThisRoomInfo.DoorList[MathHelperLibrary.LoopInRange((YawRotation / 90) + 1, 4)];
		}
		// 직선 맵이라면
		else
		{	
			// 다른 문은 2칸 오른쪽(즉, 반대편) 문을 선택
			OtherDoor.DoorRef = ThisRoomInfo.DoorList[MathHelperLibrary.LoopInRange((YawRotation / 90) + 2, 4)];
		}

		OtherDoor.UpdateDestination(this);
	}



	public override void SetActiveNearbyRooms(bool NewActive)
	{
		base.SetActiveNearbyRooms(NewActive);

		OtherDoor.DoorRef.ConnectedRoom.Room.gameObject.SetActive(NewActive);
	}



	public override LevelDoorObject GetDoorByDestination(LevelRoom DoorDestinationRoom)
	{
		if (PivotDoor.DoorRef.ConnectedRoom.Room == DoorDestinationRoom)
		{
			return PivotDoor;
		}
		else return OtherDoor;
	}
}
