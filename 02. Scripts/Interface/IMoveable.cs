using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 이동을 다루는 인터페이스 클래스입니다.
 * @since 2023-03-16
 * @see PlayerController 참고.
 */
public interface IMoveable
{
	public abstract void Move(Vector3 InputAxisValue);

	/// <summary>
	/// 이동 방식이 해제되었을 때 호출
	/// </summary>
	public abstract void OnMovementMethodReleased(PlayerController Player);

	/// <summary>
	/// 이동 방식으로 설정했을 때 호출
	/// </summary>
	public abstract void OnMovementMethodSet(PlayerController Player);
}
