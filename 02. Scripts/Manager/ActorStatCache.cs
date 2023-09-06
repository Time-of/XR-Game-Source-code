using Amazon.DynamoDBv2.DataModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



/**
 * @author 이성수
 * @brief 적이 새로 생성될 때마다 CSV를 읽어오지 않도록, 가져온 적 있는 스탯 정보를 저장하는 클래스입니다.
 * @since 2023-03-26
 */
public class ActorStatCache : MonoBehaviour
{
	public static ActorStatCache instance;

	//public Dictionary<string, float[]> Deprecated_AutoTurretStatDictionary { get; private set; }

	public Dictionary<string, float[]> CharacterStatDictionary { get; private set; }

	public Dictionary<string, float[]> WeaponStatDictionary { get; private set; }



	public ActorStatCache() : base()
	{
		//Deprecated_AutoTurretStatDictionary = new Dictionary<string, float[]>();
		CharacterStatDictionary = new Dictionary<string, float[]>();
		WeaponStatDictionary = new Dictionary<string, float[]>();
	}



	private void Awake()
	{
		if (instance == null) instance = this;

		//Deprecated_AutoTurretStatDictionary = CSVReader.GetAllRowColumns("CSV/AutoTurretStats.csv");
		CharacterStatDictionary = CSVReader.GetAllRowColumns("CSV/CharacterStats.csv");
		WeaponStatDictionary = CSVReader.GetAllRowColumns("CSV/WeaponStats.csv");
	}
}
