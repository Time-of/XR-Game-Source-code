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



// �� ���� ��Ʈ�÷���, DOWN�� �⺻ �����̸� Yaw ȸ���� 90���� �ϴ� �������� ����
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
	/// ���� ���� Ÿ���� üũ�մϴ�.
	/// </summary>
	/// <returns>���� ���� Ÿ��, ���� �������κ����� Yaw ȸ����</returns>
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
	/// �� ���� �÷��׿� ���� �����κ��� ���� ������ üũ�մϴ�.
	/// </summary>
	/// <param name="DoorDirectionFlags">��ȿ�� �� ���� �÷���</param>
	/// <param name="ValidDoorCount">��ȿ�� �� ����</param>
	/// <returns>���� ���� Ÿ��, ���� �������κ����� Yaw ȸ����</returns>
	private (ERoomPathType, int) CheckDirection(EDoorDirectionFlags DoorDirectionFlags, int ValidDoorCount)
	{
		int BitSize = sizeof(EDoorDirectionFlags) * 8;

		for (int i = 0; i < BitSize; ++i)
		{
			EDoorDirectionFlags CurrentFlag = (EDoorDirectionFlags)(1 << i);

			// �ش� �÷��װ� �������� �ʴٸ� ���� ���� ����
			if ((DoorDirectionFlags & CurrentFlag) == 0) continue;

			// ������ 1�� ��
			if (ValidDoorCount == 1)
			{
				// ���� �÷��׿� �����ϴٸ� i * 90�� ������ ��ȯ (DOWN�� �⺻ ����)
				return (ERoomPathType.ONE_PATH, i * 90);
			}
			// ������ 2�� ��
			else if (ValidDoorCount == 2)
			{
				// ���� �÷��� + 1�� ���⿡ �ִ� �÷��׵� �Բ� �����ִٸ� (���� �ڳ� ����)
				if ((DoorDirectionFlags & ((EDoorDirectionFlags)(1 << MathHelperLibrary.LoopInRange(i + 1, 4)))) != 0)
				{
					return (ERoomPathType.TWO_PATH_CORNER, i * 90);
				}
				// ���� �÷��� + 2�� ���⿡ �ִ� �÷��׵� �Բ� �����ִٸ� (���� ����)
				else if ((DoorDirectionFlags & ((EDoorDirectionFlags)(1 << MathHelperLibrary.LoopInRange(i + 2, 4)))) != 0)
				{
					return (ERoomPathType.TWO_PATH_STRAIGHT, i * 90);
				}
			}
			// ������ 3�� ��
			else if (ValidDoorCount == 3)
			{
				// ���� �÷��� + 1�� ���� �� �÷��� - 1�� ������ ��� ������ ��� (��, ���������� T�ڰ� �ϼ��� ���)
				if ((DoorDirectionFlags & ((EDoorDirectionFlags)(1 << MathHelperLibrary.LoopInRange(i + 1, 4)))) != 0
						&& (DoorDirectionFlags & ((EDoorDirectionFlags)(1 << MathHelperLibrary.LoopInRange(i - 1, 4)))) != 0)
				{
					return (ERoomPathType.THREE_PATH, i * 90);
				}
			}
			// ������ 4�϶��� ���� ��� ���� ��ȯ
			else if (ValidDoorCount == 4)
			{
				return (ERoomPathType.FOUR_PATH, 0);
			}
		}

		Debug.LogWarning("���� ���� ����: ��ȿ���� ���� ���� ����!!: " + DoorDirectionFlags + ", ���� ����: " + ValidDoorCount);
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



	// Ȯ���� ������ ����
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
 * @author �̼���
 * @brief ������ ��ġ�Ǵ� �� Ŭ�����Դϴ�.
 * @details LevelGenerator�� ���� �����Ǵ� ��
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



	// �濡 �湮 �� ȣ��Ǵ� ���
	public void OnVisitRoom()
	{
		EnemysCount = 0;

		if (!bIsCleared)
		{
			// �Ϲݹ��� ���!
			if (!bIsBossRoom)
			{
				CreateEnemies();
			}
			// �������� ���!
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



	// �� �渶�� �������̵�,,
	protected virtual void OpenDoors()
	{
		PivotDoor.bIsLocked = false;
	}



	protected virtual void InitializeDoors(RoomInfo ThisRoomInfo, int YawRotation)
	{
		// Yaw ȸ������ ���� ���� ������
		// (DoorList���� �� Ÿ�� ���ذ� ���������� Down, Left, Up, Right ������ �� ����)
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
	/// ���� ������ ���� ���� ���մϴ�. (��, ���� �� ���ϱ� ���� ����մϴ�)
	/// </summary>
	/// <param name="DoorDestinationRoom">���� ������ �� ��</param>
	/// <returns>�� �߿� �������� DoorDestinationRoom�� ��</returns>
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
