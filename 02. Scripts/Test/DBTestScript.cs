using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;



/**
 * @author 이성수
 * @brief AWS 처음 써 보기 앞서 사용해보는 테스트 코드
 * @since 2023-03-23
 */
public class DBTestScript : MonoBehaviour
{
	AmazonDynamoDBClient DBClient;
	DynamoDBContext Context;

	// 자격증명?
	CognitoAWSCredentials credentials;



	private void Awake()
	{
		UnityInitializer.AttachToGameObject(gameObject);

		// Cognito 연동을 위해 생성한 credentials를 이용해 DB 클라이언트 구성
		credentials = new CognitoAWSCredentials("ap-northeast-2:5e90bdac-5b04-4f08-86fa-7c73c7fdbb34",
			RegionEndpoint.APNortheast2);

		DBClient = new AmazonDynamoDBClient(credentials, RegionEndpoint.APNortheast2);
		Context = new DynamoDBContext(DBClient);
	}



	private void Start()
	{
		AddEnemyStatInfoToDB();
		FindItem();
	}



	[DynamoDBTable("EnemyCharacterStat")]
	public class TestEnemyStatInfo
	{
		[DynamoDBHashKey]
		public string InternalName { get; set; }

		[DynamoDBProperty]
		public float Strength { get; set; }

		[DynamoDBProperty]
		public float AttackSpeed { get; set; }

		[DynamoDBProperty]
		public float Defense { get; set; }

		[DynamoDBProperty]
		public float MaxHealth { get; set; }

		[DynamoDBProperty]
		public float MoveSpeed { get; set; }
	}



	// 적 스탯 정보를 DB에 업로드
	private void AddEnemyStatInfoToDB()
	{
		TestEnemyStatInfo testInfo = new TestEnemyStatInfo
		{
			InternalName = "TestEnemy",
			Strength = 1,
			AttackSpeed = 500,
			Defense = 1000,
			MaxHealth = 650,
			MoveSpeed = 3.5f
		};

		Context.SaveAsync<TestEnemyStatInfo>(testInfo, (result) =>
		{
			// testInfo 정보를 DB에 저장
			if (result.Exception == null) Debug.Log("DB 업로드 성공!!");
			else Debug.Log(result.Exception);
		});
	}



	// DB로부터 찾기
	private void FindItem()
	{
		TestEnemyStatInfo testInfo;

		Context.LoadAsync<TestEnemyStatInfo>("TestEnemy", (result) =>
		{
			// TestEnemy 프라이머리 키로부터 testInfo 정보 가져오기
			if (result.Exception != null)
			{
				Debug.LogException(result.Exception);
				return;
			}

			testInfo = result.Result;

			Debug.Log("DB에서 찾음!!: " + testInfo.InternalName);
		});
	}
}
