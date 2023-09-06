using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 수학과 관련한 기능을 모아 놓은 라이브러리 클래스입니다.
 * @since 2023-03-16
 */
public class MathHelperLibrary : MonoBehaviour
{
	public static float MapRangeClamped(float Value, float Min, float Max, float TargetMin, float TargetMax)
	{
		return Mathf.Clamp(Value / (Max - Min) * (TargetMax - TargetMin), TargetMin, TargetMax);
	}



	public static Vector3 VLerp(Vector3 From, Vector3 To, float Alpha)
	{
		return new Vector3(Mathf.Lerp(From.x, To.x, Alpha), Mathf.Lerp(From.y, To.y, Alpha), Mathf.Lerp(From.z, To.z, Alpha));
	}



	public static Vector3 LimitVector(Vector3 Vec, float Limit)
	{
		return (Vec.sqrMagnitude > Limit * Limit) ? Vec.normalized * Limit : Vec;
	}



	// [0 ~ ExclusiveRangeFromZero - 1] 안에서 Num이 루프하도록 하는 기능.
	public static int LoopInRange(int Num, int ExclusiveRangeFromZero)
	{
		Num %= ExclusiveRangeFromZero;
		return (Num < 0) ? Num + ExclusiveRangeFromZero : Num;
	}



	public static bool AlmostEquals(Quaternion target, Quaternion second, float maxAngle)
	{
		return Quaternion.Angle(target, second) < maxAngle;
	}
}
