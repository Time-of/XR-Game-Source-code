using Oculus.Platform.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public enum ERoomDirection
{
	LEFT, RIGHT, UP, DOWN
}



/**
 * @author 이성수
 * @brief 레벨 생성기 클래스입니다.
 * @since 2023-04-16
 */
public class LevelGenerator : MonoBehaviour
{
	private Vector2Int StartRoomCoord = new Vector2Int();

	public LevelRoom StartRoom { get; private set; }

	public LevelRoom BossRoom { get; private set; }

	List<(int, int)> ValidCoords = new List<(int, int)>();

	[SerializeField]
	private GameObject TestPlayer;

	[Header("레벨 생성 설정")]

	[SerializeField]
	private int RoomCount = 10;

	private int RemainRoomCount = 0;

	private int GeneratedRoomCount = 0;

	// 방과 방 사이 간격 (방 프리팹 생성 시 사용)
	[SerializeField]
	private float DistanceBetweenRooms = 75.0f;

	// 자동으로 RoomCount에 따라 레벨 크기 계산
	[SerializeField]
	private bool bAutoCalcLevelWidthHeight = true;

	[SerializeField, EditCondition("!bAutoCalcLevelWidthHeight")]
	private int DefaultLevelWidth = 6;

	[SerializeField, EditCondition("!bAutoCalcLevelWidthHeight")]
	private int DefaultLevelHeight = 6;

	private int LevelWidth = 6;

	private int LevelHeight = 6;

	private List<List<RoomInfo>> LevelMap;

	private int[] deltaX = { 1, 0, -1, 0 };

	private int[] deltaY = { 0, 1, 0, -1 };

	// 가장 깊은 깊이
	private int DeepestDepth = 0;

	// 가장 깊은 곳의 좌표값 (보스방)
	private Vector2Int DeepestCoord = new Vector2Int();

	WaitForSeconds WaitForSecondsUntilRoomInstantiated = new WaitForSeconds(0.3f);

	[Header("레벨 방 프리팹")]

	[SerializeField]
	private List<LevelRoom> OnePathRoomPrefabs = new List<LevelRoom>();

	[SerializeField]
	private List<LevelRoom_TwoPaths> TwoPathCornerRoomPrefabs = new List<LevelRoom_TwoPaths>();

	[SerializeField]
	private List<LevelRoom_TwoPaths> TwoPathStraightRoomPrefabs = new List<LevelRoom_TwoPaths>();

	[SerializeField]
	private List<LevelRoom_ThreePaths> ThreePathRoomPrefabs = new List<LevelRoom_ThreePaths>();

	[SerializeField]
	private List<LevelRoom_FourPaths> FourPathRoomPrefabs = new List<LevelRoom_FourPaths>();



	private void Start()
	{
		InitLevelMap();

		CreateFirstRoom();

		SecureRoomSpaceWalker();

		ConnectSecuredRoomsByDFS(StartRoomCoord.y, StartRoomCoord.x, 0);

		CreateRooms();

		SetStartRoom();

		PlaceBossRoom();

		// PlaceSpecialRooms();

		FinishCreateRooms();
	}



	public void InitLevelMap()
	{
		if (bAutoCalcLevelWidthHeight)
		{
			LevelHeight = LevelWidth = Mathf.FloorToInt(Mathf.Sqrt(RoomCount * 4));

			Debug.Log("레벨 생성: 자동 계산된 레벨 크기: " + LevelHeight + " x " + LevelWidth);
		}
		else
		{
			LevelHeight = DefaultLevelHeight;
			LevelWidth = DefaultLevelWidth;
		}

		LevelMap = new List<List<RoomInfo>>();
		LevelMap.Capacity = LevelHeight;

		for (int i = 0; i < LevelHeight; ++i)
		{
			List<RoomInfo> Row = new List<RoomInfo>();
			Row.Capacity = LevelWidth;

			for (int j = 0; j < LevelWidth; ++j)
			{
				Row.Add(new RoomInfo());
			}

			LevelMap.Add(Row);
		}
	}



	private void CreateFirstRoom()
	{
		RemainRoomCount = RoomCount;

		--RemainRoomCount;

		StartRoomCoord.y = Random.Range(0, LevelHeight);
		StartRoomCoord.x = Random.Range(0, LevelWidth);

		LevelMap[StartRoomCoord.y][StartRoomCoord.x].SetSecuredRoom(true);

		Debug.Log("레벨 생성: 시작 방 생성 좌표: " + StartRoomCoord.y + ", " + StartRoomCoord.x);
	}



	// 랜덤 워커로 공간을 먼저 확보
	private void SecureRoomSpaceWalker()
	{
		int y = StartRoomCoord.y;
		int x = StartRoomCoord.x;

		while (RemainRoomCount > 0)
		{
			switch (Random.Range(0, 4))
			{
				case 0:
					y = Mathf.Clamp(y + 1, 0, LevelHeight - 1);
					break;
				case 1:
					y = Mathf.Clamp(y - 1, 0, LevelHeight - 1);
					break;
				case 2:
					x = Mathf.Clamp(x + 1, 0, LevelWidth - 1);
					break;
				case 3:
					x = Mathf.Clamp(x - 1, 0, LevelWidth - 1);
					break;
				default:
					break;
			}

			if (!LevelMap[y][x].bIsSecuredRoom)
			{
				LevelMap[y][x].SetSecuredRoom(true);
				Debug.Log("레벨 생성 디버그: 공간 확보: " + y + ", " + x);
				--RemainRoomCount;
			}
		}

		Debug.Log("레벨 생성: 미리 공간 확보 끝!");
	}



	// 확보된 방들을 연결하기 (DFS 알고리즘 사용)
	private bool ConnectSecuredRoomsByDFS(int y, int x, int Depth)
	{
		// 범위 초과 또는 방문했던 곳이나 확보되지 않은 공간인 경우 반환
		if (y < 0 || y >= LevelHeight || x < 0 || x >= LevelWidth || LevelMap[y][x].bVisited || !LevelMap[y][x].bIsSecuredRoom)
			return false;

		LevelMap[y][x].bVisited = true;

		if (Depth > DeepestDepth)
		{
			DeepestDepth = Depth;
			DeepestCoord = new Vector2Int(x, y);
		}

		for (int i = 0; i < 4; ++i)
		{
			if (ConnectSecuredRoomsByDFS(y + deltaY[i], x + deltaX[i], Depth + 1))
			{
				ConnectRoomToRoom(LevelMap[y][x], LevelMap[y + deltaY[i]][x + deltaX[i]],
					GetRoomDirection(y, x, y + deltaY[i], x + deltaX[i]));

				Debug.Log("<color=yellow>레벨 문 연결: " + y + ", " + x + "와 " + (y + deltaY[i]) + ", " + (x + deltaX[i]) + " 연결됨!!</color> 방향: " + GetRoomDirection(y, x, y + deltaY[i], x + deltaX[i]));
			}
		}

		return true;
	}



	private void CreateRooms()
	{
		ValidCoords.Capacity = RoomCount;

		for (int i = 0; i < LevelHeight; ++i)
		{
			for (int j = 0; j < LevelWidth; ++j)
			{
				if (LevelMap[i][j].bIsSecuredRoom)
				{
					var PathYawPair = LevelMap[i][j].CheckRoomPathType();

					LevelRoom RoomPrefabToCreate = GetRandomLevelRoomPrefab(PathYawPair.Item1);

					if (RoomPrefabToCreate == null) Debug.LogWarning("레벨의 방 생성 중 오류 발생: 좌표: (" + i + ", " + j + ")");

					LevelMap[i][j].YawRotation = PathYawPair.Item2;

					LevelMap[i][j].Room = Instantiate(RoomPrefabToCreate,
						new Vector3(j * DistanceBetweenRooms, 0.0f, (LevelHeight - i) * DistanceBetweenRooms),
						Quaternion.Euler(0.0f, PathYawPair.Item2, 0.0f),
						transform);

					ValidCoords.Add((i, j));
				}
			}
		}
	}



	private void SetStartRoom()
	{
		StartRoom = LevelMap[StartRoomCoord.y][StartRoomCoord.x].Room;
	}



	private void PlaceBossRoom()
	{
		BossRoom = LevelMap[DeepestCoord.y][DeepestCoord.x].Room;
	}



	private void FinishCreateRooms()
	{
		foreach (var Coord in ValidCoords)
		{
			StartCoroutine(CheckSetRoomCompletedCoroutine(Coord.Item1, Coord.Item2));
		}
	}



	#region 함수 내부적으로 들어간 기능들
	// 방과 방 사이의 방향 구하기, 서로 붙어있어야 한다는 전제조건 있음, 서로 같은 좌표 금지
	private ERoomDirection GetRoomDirection(int FirstY, int FirstX, int SecondY, int SecondX)
	{
		if (FirstX < SecondX) return ERoomDirection.RIGHT;
		else if (FirstX > SecondX) return ERoomDirection.LEFT;
		else if (FirstY < SecondY) return ERoomDirection.DOWN;
		else return ERoomDirection.UP;
	}



	// 방과 방을 연결
	private void ConnectRoomToRoom(RoomInfo StartRoom, RoomInfo EndRoom, ERoomDirection Direction)
	{
		switch (Direction)
		{
			case ERoomDirection.LEFT:
				StartRoom.LeftDoor.ConnectedRoom = EndRoom;
				EndRoom.RightDoor.ConnectedRoom = StartRoom;
				break;
			case ERoomDirection.RIGHT:
				StartRoom.RightDoor.ConnectedRoom = EndRoom;
				EndRoom.LeftDoor.ConnectedRoom = StartRoom;
				break;
			case ERoomDirection.UP:
				StartRoom.UpDoor.ConnectedRoom = EndRoom;
				EndRoom.DownDoor.ConnectedRoom = StartRoom;
				break;
			case ERoomDirection.DOWN:
				StartRoom.DownDoor.ConnectedRoom = EndRoom;
				EndRoom.UpDoor.ConnectedRoom = StartRoom;
				break;
			default:
				break;
		}
	}



	private LevelRoom GetRandomLevelRoomPrefab(ERoomPathType RoomType)
	{
		LevelRoom RoomPrefab;

		switch (RoomType)
		{
			case ERoomPathType.ONE_PATH:
				RoomPrefab = OnePathRoomPrefabs[Random.Range(0, OnePathRoomPrefabs.Count)];
				break;
			case ERoomPathType.TWO_PATH_CORNER:
				RoomPrefab = TwoPathCornerRoomPrefabs[Random.Range(0, TwoPathCornerRoomPrefabs.Count)];
				break;
			case ERoomPathType.TWO_PATH_STRAIGHT:
				RoomPrefab = TwoPathStraightRoomPrefabs[Random.Range(0, TwoPathStraightRoomPrefabs.Count)];
				break;
			case ERoomPathType.THREE_PATH:
				RoomPrefab = ThreePathRoomPrefabs[Random.Range(0, ThreePathRoomPrefabs.Count)];
				break;
			case ERoomPathType.FOUR_PATH:
				RoomPrefab = FourPathRoomPrefabs[Random.Range(0, FourPathRoomPrefabs.Count)];
				break;
			case ERoomPathType.ERROR:
			default:
				return null;
		}

		return RoomPrefab;
	}



	private IEnumerator CheckSetRoomCompletedCoroutine(int y, int x)
	{
		while (LevelMap[y][x].Room == null)
		{
			yield return WaitForSecondsUntilRoomInstantiated;
		}

		++GeneratedRoomCount;

		if (GeneratedRoomCount == RoomCount)
			OnFullyGenerated();
	}



	private void OnFullyGenerated()
	{
		StartRoom.bIsStartRoom = true;

		foreach (var Coord in ValidCoords)
		{
			LevelMap[Coord.Item1][Coord.Item2].Room.InitializeRoom(LevelMap[Coord.Item1][Coord.Item2]);
		}

		Instantiate(TestPlayer, StartRoom.transform.position, Quaternion.identity);

		StartRoom.bIsCleared = true;

		StartRoom.OnVisitRoom();

		BossRoom.bIsBossRoom = true;
	}
	#endregion
}
