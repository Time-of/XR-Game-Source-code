using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 무기 최상위 클래스입니다.
 * @since 2023-04-12
 */
[RequireComponent(typeof(WeaponStat))]
public class WeaponBase : MonoBehaviour
{
	protected WeaponStat StatComp;

	public string InternalName;

	[SerializeField]
	protected Transform AttachTransform;



	public WeaponBase() : base()
	{
		//InternalObjectType = EObjectType.WEAPON;
	}



	protected virtual void Awake()
	{
		StatComp = GetComponent<WeaponStat>();
	}



	protected virtual void Start()
	{
		float[] StatInfo = ActorStatCache.instance.WeaponStatDictionary[InternalName];
		StatComp.InitializeInitStats(StatInfo[0], StatInfo[1]);

		StatComp.InitializeStats();
	}



	public Transform GetAttachTransform()
	{
		return AttachTransform != null ? AttachTransform : transform;
	}



	public virtual void OnEquipped() { }

	public virtual void OnUnEquipped() { }

	// 트리거 버튼 누름
	public virtual void OnActivated() { }

	// 트리거 버튼 누르지 않음
	public virtual void OnDeactivated() { }
}
