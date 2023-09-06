using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



/**
 * @author 이성수
 * @brief CSV 파일을 읽는 클래스입니다.
 * @since 2023-03-26
 */
public class CSVReader
{
	/// <summary>
	/// 데이터 행과 열을 읽어 딕셔너리로 반환합니다.
	/// </summary>
	/// <param name="FileName">애셋폴더 내 경로/파일명.csv (예: CSV/EnemyStats.csv)</param>
	/// <param name="bIgnoreFirstRow">true라면 첫번째 행은 무시 (제목 행?)</param>
	/// <returns><행, 열들> 딕셔너리</returns>
	public static Dictionary<string, float[]> GetAllRowColumns(string FileName, bool bIgnoreFirstRow = true)
	{
		Dictionary<string, float[]> RowColumnsDictionary = new Dictionary<string, float[]>();

		string FilePath = Path.Combine(Application.dataPath, FileName);

		if (File.Exists(FilePath))
		{
			string[] Rows = File.ReadAllLines(FilePath);
			int RowLength = Rows.Length;

			for (int i = (bIgnoreFirstRow ? 1 : 0); i < RowLength; i++)
			{
				string[] Columns = Rows[i].Split(',');
				int ColumnLength = Columns.Length;

				float[] FloatColumns = new float[ColumnLength - 1];

				for (int k = 1; k < ColumnLength; k++)
				{
					FloatColumns[k - 1] = float.Parse(Columns[k]);
				}
				
				RowColumnsDictionary[Columns[0]] = FloatColumns;
			}
		}
		else
		{
			Debug.LogError("파일 찾을 수 없음!!! " + FileName + ", 경로: " + FilePath);
		}

		return RowColumnsDictionary;
	}
}
