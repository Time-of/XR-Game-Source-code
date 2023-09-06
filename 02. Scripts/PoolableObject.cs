using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



[System.Serializable]
public enum EObjectType
{
	UNDEFINED, CHARACTER, PARTICLE, PROJECTILE, WEAPON, VEHICLE, SOUND, BUFF
}



/**
 * @author 이성수
 * @brief 오브젝트 풀링이 가능한 오브젝트 클래스입니다.
 * @since 2023-03-12
 * @see ObjectPoolingSystem 참고.
 */
public class PoolableObject : MonoBehaviour
{
	[SerializeField]
	protected string InternalObjectName = "DEFAULT";

	public string InternalName { get { return InternalObjectName; } }

	[SerializeField]
	protected EObjectType InternalObjectType = EObjectType.UNDEFINED;

	public EObjectType InternalType { get { return InternalObjectType; } }

	//public UInt16 PoolQuantity;

	protected GameObject Instigator;

	public UnityEvent<PoolableObject> OnReturnedToPoolEvent = new UnityEvent<PoolableObject>();



	/// <summary>
	/// 인스티게이터(피해를 준 주체)를 설정
	/// </summary>
	/// <param name="NewInstigator">새로 설정할 인스티게이터</param>
	public void SetInstigator(GameObject NewInstigator)
	{
		Instigator = NewInstigator;
	}



	/// <summary>
	/// 스폰이 완료되기 전에 호출
	/// 의도: 주요 변수 초기화 용도로 사용
	/// 실제 호출: GameplayHelperLibrary에 의해 호출 메서드가 실행되면, FinishSpawnObject()가 호출되기 전에 호출
	/// </summary>
	public virtual void OnPreSpawnedFromPool()
	{
		Instigator = null;
	}

	/// <summary>
	/// 스폰이 완료된 후 호출
	/// 실제 호출: GameplayHelperLibrary에 의해 FinishSpawnObject()가 실행될 때 호출 (즉, 일반적으로 스폰을 끝냈을 때)
	/// </summary>
	public virtual void OnPostSpawnedFromPool() { }

	/// <summary>
	/// 오브젝트 풀로 돌아갈 때 호출
	/// </summary>
	public virtual void OnReturnedToPool() { }



	public void ReturnToPool()
	{
		OnReturnedToPoolEvent.Invoke(this);
		OnReturnedToPoolEvent.RemoveAllListeners();
		ObjectPoolingSystem.instance.ReturnObjectToPool(this);
	}



	public IEnumerator ReturnToPool(float Time)
	{
		yield return new WaitForSeconds(Time);

		ReturnToPool();
	}



	public void SetObjectRotation(float Pitch, float Yaw, float Roll)
    {
		transform.rotation = Quaternion.Euler(Pitch, Yaw, Roll);
	}
	
	public Vector3 GetObjectEulerRotation()
    {
		return transform.rotation.eulerAngles;
    }
}
