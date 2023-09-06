using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class LevelDoor
{
	public RoomInfo ConnectedRoom;
}



public enum ERoomPathType
{
	ONE_PATH, TWO_PATH_CORNER, TWO_PATH_STRAIGHT, THREE_PATH, FOUR_PATH, ERROR
}



// 문 방향 비트플래그, DOWN이 기본 상태이며 Yaw 회전을 90도씩 하는 방향으로 구성
[Flags]
public enum EDoorDirectionFlags
{
	NONE = 0,
	DOWN = 1 << 0,
	LEFT = 1 << 1,
	UP = 1 << 2,
	RIGHT = 1 << 3
}



public class RoomInfo
{
	public RoomInfo()
	{
		bIsSecuredRoom = false;
		Room = null;

		InitializeDoorList();
	}



	public void SetSecuredRoom(bool NewActive)
	{
		bIsSecuredRoom = NewActive;
	}



	/// <summary>
	/// 방의 방향 타입을 체크합니다.
	/// </summary>
	/// <returns>방의 방향 타입, 기준 방향으로부터의 Yaw 회전값</returns>
	public (ERoomPathType, int) CheckRoomPathType()
	{
		EDoorDirectionFlags DoorDirectionFlags = EDoorDirectionFlags.NONE;

		int ValidDoorCount = 0;

		if (LeftDoor.ConnectedRoom != null)
		{
			DoorDirectionFlags |= EDoorDirectionFlags.LEFT;
			++ValidDoorCount;
		}

		if (RightDoor.ConnectedRoom != null)
		{
			DoorDirectionFlags |= EDoorDirectionFlags.RIGHT;
			++ValidDoorCount;
		}

		if (UpDoor.ConnectedRoom != null)
		{
			DoorDirectionFlags |= EDoorDirectionFlags.UP;
			++ValidDoorCount;
		}

		if (DownDoor.ConnectedRoom != null)
		{
			DoorDirectionFlags |= EDoorDirectionFlags.DOWN;
			++ValidDoorCount;
		}

		return CheckDirection(DoorDirectionFlags, ValidDoorCount);
	}



	/// <summary>
	/// 문 방향 플래그와 문의 개수로부터 방의 방향을 체크합니다.
	/// </summary>
	/// <param name="DoorDirectionFlags">유효한 문 방향 플래그</param>
	/// <param name="ValidDoorCount">유효한 문 개수</param>
	/// <returns>방의 방향 타입, 기준 방향으로부터의 Yaw 회전값</returns>
	private (ERoomPathType, int) CheckDirection(EDoorDirectionFlags DoorDirectionFlags, int ValidDoorCount)
	{
		int BitSize = sizeof(EDoorDirectionFlags) * 8;

		for (int i = 0; i < BitSize; ++i)
		{
			EDoorDirectionFlags CurrentFlag = (EDoorDirectionFlags)(1 << i);

			// 해당 플래그가 켜져있지 않다면 다음 루프 진행
			if ((DoorDirectionFlags & CurrentFlag) == 0) continue;

			// 개수가 1일 때
			if (ValidDoorCount == 1)
			{
				// 현재 플래그와 동일하다면 i * 90도 각도를 반환 (DOWN이 기본 상태)
				return (ERoomPathType.ONE_PATH, i * 90);
			}
			// 개수가 2일 때
			else if (ValidDoorCount == 2)
			{
				// 현재 플래그 + 1의 방향에 있는 플래그도 함께 켜져있다면 (ㄱ자 코너 검출)
				if ((DoorDirectionFlags & ((EDoorDirectionFlags)(1 << MathHelperLibrary.LoopInRange(i + 1, 4)))) != 0)
				{
					return (ERoomPathType.TWO_PATH_CORNER, i * 90);
				}
				// 현재 플래그 + 2의 방향에 있는 플래그도 함께 켜져있다면 (직선 검출)
				else if ((DoorDirectionFlags & ((EDoorDirectionFlags)(1 << MathHelperLibrary.LoopInRange(i + 2, 4)))) != 0)
				{
					return (ERoomPathType.TWO_PATH_STRAIGHT, i * 90);
				}
			}
			// 개수가 3일 때
			else if (ValidDoorCount == 3)
			{
				// 현재 플래그 + 1의 방향 및 플래그 - 1의 방향이 모두 충족된 경우 (즉, 기준점으로 T자가 완성된 경우)
				if ((DoorDirectionFlags & ((EDoorDirectionFlags)(1 << MathHelperLibrary.LoopInRange(i + 1, 4)))) != 0
						&& (DoorDirectionFlags & ((EDoorDirectionFlags)(1 << MathHelperLibrary.LoopInRange(i - 1, 4)))) != 0)
				{
					return (ERoomPathType.THREE_PATH, i * 90);
				}
			}
			// 개수가 4일때는 방향 상관 없이 반환
			else if (ValidDoorCount == 4)
			{
				return (ERoomPathType.FOUR_PATH, 0);
			}
		}

		Debug.LogWarning("레벨 생성 오류: 유효하지 않은 방의 방향!!: " + DoorDirectionFlags + ", 방의 개수: " + ValidDoorCount);
		return (ERoomPathType.ERROR, 0);
	}



	private void InitializeDoorList()
	{
		DoorList.Clear();
		DoorList.Add(DownDoor);
		DoorList.Add(LeftDoor);
		DoorList.Add(UpDoor);
		DoorList.Add(RightDoor);
	}



	// 확보된 방인지 여부
	public bool bIsSecuredRoom;
	public LevelRoom Room;
	public bool bVisited = false;
	public int YawRotation = 0;

	public List<LevelDoor> DoorList = new List<LevelDoor>();

	public LevelDoor LeftDoor = new LevelDoor();
	public LevelDoor RightDoor = new LevelDoor();
	public LevelDoor UpDoor = new LevelDoor();
	public LevelDoor DownDoor = new LevelDoor();
}



/**
 * @author 이성수
 * @brief 레벨에 배치되는 룸 클래스입니다.
 * @details LevelGenerator에 의해 생성되는 룸
 * @since 2023-04-16
 */
public class LevelRoom : MonoBehaviour
{
	public LevelDoorObject PivotDoor;

	public NavMeshSurface GroundSurface;

	public bool bIsCleared = false;

	[SerializeField]
	private List<Transform> EnemySpawnPositions = new();

	[SerializeField, ReadOnlyProperty]
	private int EnemysCount = 0;

	static WaitForSeconds WaitSeconds = new(1.0f);

	public bool bIsStartRoom = false;

	public bool bIsBossRoom = false;



	public void InitializeRoom(RoomInfo ThisRoomInfo)
	{
		InitializeDoors(ThisRoomInfo, ThisRoomInfo.YawRotation);

		GenerateNavMesh();

		StartCoroutine(WaitAndSetActiveToFalseCoroutine());
	}



	private IEnumerator WaitAndSetActiveToFalseCoroutine()
	{
		yield return WaitSeconds;

		if (!bIsStartRoom) gameObject.SetActive(false);
		else
		{
			StartCoroutine(WaitAndSetActiveToTrueOnStartRoom());
		}
	}



	private IEnumerator WaitAndSetActiveToTrueOnStartRoom()
	{
		yield return WaitSeconds;

		SetActiveNearbyRooms(true);
	}



	// 방에 방문 시 호출되는 기능
	public void OnVisitRoom()
	{
		EnemysCount = 0;

		if (!bIsCleared)
		{
			// 일반방인 경우!
			if (!bIsBossRoom)
			{
				CreateEnemies();
			}
			// 보스방인 경우!
			else
			{
				CreateBoss();
			}
		}
		else
		{
			OpenDoors();
		}
	}



	private void CreateEnemies()
	{
		foreach (Transform SpawnTransform in EnemySpawnPositions)
		{
			++EnemysCount;

			EnemyCharacterBase SpawnedEnemy = GameManager.instance.SpawnerComp.SpawnEnemyAtTransform(SpawnTransform);

			if (SpawnedEnemy != null)
			{
				SpawnedEnemy.OnEnemyDiedEvent.AddListener(OnEnemyDiedListner);
			}
		}

		OpenDoorsOnEnemyCountIsZero();
	}



	private void CreateBoss()
	{
		int RandomPosition = UnityEngine.Random.Range(0, EnemySpawnPositions.Count - 1);

		++EnemysCount;

		ABossBase SpawnedEnemy = GameManager.instance.SpawnerComp.SpawnBossAtTransform(EnemySpawnPositions[RandomPosition]);

		if (SpawnedEnemy != null)
		{
			SpawnedEnemy.OnBossDiedEvent.AddListener(OnEnemyDiedListner);
		}
	}



	// 각 방마다 오버라이드,,
	protected virtual void OpenDoors()
	{
		PivotDoor.bIsLocked = false;
	}



	protected virtual void InitializeDoors(RoomInfo ThisRoomInfo, int YawRotation)
	{
		// Yaw 회전값에 따라 문을 선택함
		// (DoorList에도 방 타입 기준과 마찬가지로 Down, Left, Up, Right 순으로 들어가 있음)
		PivotDoor.DoorRef = ThisRoomInfo.DoorList[YawRotation / 90];
		PivotDoor.UpdateDestination(this);
	}



	public virtual void SetActiveNearbyRooms(bool NewActive)
	{
		gameObject.SetActive(NewActive);

		PivotDoor.DoorRef.ConnectedRoom.Room.gameObject.SetActive(NewActive);
	}



	private void GenerateNavMesh()
	{
		GroundSurface.BuildNavMesh();
	}



	/// <summary>
	/// 문의 목적지 룸인 문을 구합니다. (즉, 문의 페어를 구하기 위해 사용합니다)
	/// </summary>
	/// <param name="DoorDestinationRoom">문의 목적지 룸 명</param>
	/// <returns>문 중에 목적지가 DoorDestinationRoom인 문</returns>
	public virtual LevelDoorObject GetDoorByDestination(LevelRoom DoorDestinationRoom)
	{
		return PivotDoor;
	}



	private void OnEnemyDiedListner()
	{
		--EnemysCount;

		OpenDoorsOnEnemyCountIsZero();
	}



	private void OpenDoorsOnEnemyCountIsZero()
	{
		if (EnemysCount <= 0)
		{
			bIsCleared = true;
			OpenDoors();
		}
	}
}
