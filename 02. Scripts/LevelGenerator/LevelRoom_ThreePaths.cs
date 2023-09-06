using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



/**
 * @author 이성수
 * @brief 길이 세 개인 레벨 방 클래스입니다.
 * @since 2023-04-17
 */
public class LevelRoom_ThreePaths : LevelRoom
{
	public LevelDoorObject LeftDoor;

	public LevelDoorObject RightDoor;



	protected override void OpenDoors()
	{
		base.OpenDoors();

		LeftDoor.bIsLocked = false;
		RightDoor.bIsLocked = false;
	}



	protected override void InitializeDoors(RoomInfo ThisRoomInfo, int YawRotation)
	{
		base.InitializeDoors(ThisRoomInfo, YawRotation);

		// 왼쪽 문
		LeftDoor.DoorRef = ThisRoomInfo.DoorList[MathHelperLibrary.LoopInRange((YawRotation / 90) + 1, 4)];

		// 오른쪽 문
		RightDoor.DoorRef = ThisRoomInfo.DoorList[MathHelperLibrary.LoopInRange((YawRotation / 90) - 1, 4)];

		LeftDoor.UpdateDestination(this);
		RightDoor.UpdateDestination(this);
	}



	public override void SetActiveNearbyRooms(bool NewActive)
	{
		base.SetActiveNearbyRooms(NewActive);

		LeftDoor.DoorRef.ConnectedRoom.Room.gameObject.SetActive(NewActive);
		RightDoor.DoorRef.ConnectedRoom.Room.gameObject.SetActive(NewActive);
	}



	public override LevelDoorObject GetDoorByDestination(LevelRoom DoorDestinationRoom)
	{
		LevelDoorObject[] Doors = new LevelDoorObject[3];

		Doors[0] = PivotDoor;
		Doors[1] = LeftDoor;
		Doors[2] = RightDoor;

		foreach (LevelDoorObject Door in Doors)
		{
			if (Door.DoorRef.ConnectedRoom.Room == DoorDestinationRoom)
			{
				return Door;
			}
		}

		return null;
	}
}
