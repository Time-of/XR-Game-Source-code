using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 길이 네 개인 레벨 방 클래스입니다.
 * @since 2023-04-17
 */
public class LevelRoom_FourPaths : LevelRoom
{
	public LevelDoorObject LeftDoor;

	public LevelDoorObject RightDoor;

	public LevelDoorObject TopDoor;



	protected override void OpenDoors()
	{
		base.OpenDoors();

		LeftDoor.bIsLocked = false;
		RightDoor.bIsLocked = false;
		TopDoor.bIsLocked = false;
	}



	// 4방향 문은 회전값이 0이므로 직접 초기화
	protected override void InitializeDoors(RoomInfo ThisRoomInfo, int YawRotation)
	{
		PivotDoor.DoorRef = ThisRoomInfo.DoorList[0];

		// 왼쪽 문
		LeftDoor.DoorRef = ThisRoomInfo.DoorList[1];

		// 위쪽 문
		TopDoor.DoorRef = ThisRoomInfo.DoorList[2];

		// 오른쪽 문
		RightDoor.DoorRef = ThisRoomInfo.DoorList[3];

		PivotDoor.UpdateDestination(this);
		LeftDoor.UpdateDestination(this);
		TopDoor.UpdateDestination(this);
		RightDoor.UpdateDestination(this);
	}



	public override void SetActiveNearbyRooms(bool NewActive)
	{
		base.SetActiveNearbyRooms(NewActive);

		LeftDoor.DoorRef.ConnectedRoom.Room.gameObject.SetActive(NewActive);
		RightDoor.DoorRef.ConnectedRoom.Room.gameObject.SetActive(NewActive);
		TopDoor.DoorRef.ConnectedRoom.Room.gameObject.SetActive(NewActive);
	}



	public override LevelDoorObject GetDoorByDestination(LevelRoom DoorDestinationRoom)
	{
		LevelDoorObject[] Doors = new LevelDoorObject[4];

		Doors[0] = PivotDoor;
		Doors[1] = LeftDoor;
		Doors[2] = RightDoor;
		Doors[3] = TopDoor;

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
