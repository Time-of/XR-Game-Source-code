using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/**
 * @author 이성수
 * @brief 총의 탄알집 클래스입니다.
 * @since 2023-03-29
 */
[RequireComponent(typeof(BoxCollider))]
public class GunMagazine : MonoBehaviour
{
	[SerializeField]
	private int Capacity;

	public int CapacityCount { get { return Capacity; } }

	public int BulletCount { get; private set; }

	private bool bIsBulletCharging = false;

	/// <summary>
	/// 초당 총알 충전 속도
	/// </summary>
	[SerializeField]
	private float BulletChargeSpeed = 12.0f;

	private float ElapsedTime = 0.0f;

	/// <summary>
	/// 탄알집 장착되는 위치
	/// </summary>
	[SerializeField]
	private Transform MagazineEquippedPosition;

	[SerializeField, ReadOnlyProperty]
	private Quaternion MagazineEquippedLocalRotation;

	[SerializeField]
	private Transform MagazineRemovedPosition;

	private BoxCollider BoxComp;

	/// <summary>
	/// 총에서 설정 필요 !!!
	/// </summary>
	[ReadOnlyProperty]
	public Weapon_GunBase TargetGun;

	[SerializeField]
	private AudioClip MagOutSfx;

	[SerializeField]
	private AudioClip MagInSfx;

	[SerializeField]
	private MagazineInfoHUD InfoHUD;

	public MagazineInfoHUD GetInfoHUD() { return InfoHUD; }



	private void Awake()
	{
		BoxComp = GetComponent<BoxCollider>();
	}



	private void Start()
	{
		InfoHUD?.SetMagazineCapacity(Capacity);
		SetBulletCount(Capacity);
		MagazineEquippedLocalRotation = transform.localRotation;

		BoxComp.isTrigger = true;
		BoxComp.enabled = false;
	}



	private void Update()
	{
		if (!bIsBulletCharging) return;

		ElapsedTime += Time.deltaTime * BulletChargeSpeed;

		if (ElapsedTime >= 1.0f)
		{
			ElapsedTime -= 1.0f;
			SetBulletCount(Mathf.Min(BulletCount + 1, Capacity));
		}
	}



	public void SetBulletCount(int NewCount)
	{
		BulletCount = NewCount;
		InfoHUD?.UpdateBulletQuantity(BulletCount);
	}



	public void StartChargeBullet()
	{
		ElapsedTime = 0.0f;
		bIsBulletCharging = true;
	}



	public void StopChargeBullet()
	{
		ElapsedTime = 0.0f;
		bIsBulletCharging = false;
	}



	public void RemoveFromGun()
	{
		if (!TargetGun.bIsMagazineEquipped) return;

		TargetGun.SetMagazineEquipped(false);

		transform.DOLocalMove(MagazineRemovedPosition.localPosition, 0.7f)
			.OnComplete(OnFullyRemovedFromGun);

		SoundHelperLibrary.SpawnSoundAtLocation(transform.position, transform.rotation, MagOutSfx, false, 1.0f, 0.05f);
	}

	private void OnFullyRemovedFromGun()
	{
		BoxComp.enabled = true;
	}



	private void AttachToGun()
	{
		if (TargetGun.bIsMagazineEquipped) return;

		BoxComp.enabled = false;
		transform.position = MagazineRemovedPosition.position;
		transform.rotation = TargetGun.transform.rotation * MagazineEquippedLocalRotation;
		transform.DOLocalMove(MagazineEquippedPosition.localPosition, 0.7f)
			.OnComplete(OnFullyAttachedToGun);

		SoundHelperLibrary.SpawnSoundAtLocation(transform.position, transform.rotation, MagInSfx, false, 1.0f, 0.05f);
	}

	private void OnFullyAttachedToGun()
	{
		TargetGun.SetMagazineEquipped(true);
	}



	public void OnGrabEnded()
	{
		if (TargetGun.bIsMagazineEquipped) return;

		transform.position = MagazineRemovedPosition.position;
		transform.rotation = TargetGun.transform.rotation * MagazineEquippedLocalRotation;
	}



	public void ForceAttachToGun()
	{
        if (TargetGun.bIsMagazineEquipped) return;

        BoxComp.enabled = false;
        transform.position = MagazineEquippedPosition.position;
		OnFullyAttachedToGun();

        SoundHelperLibrary.SpawnSoundAtLocation(transform.position, transform.rotation, MagInSfx, false, 1.0f, 0.05f);
    }



	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == TargetGun.gameObject)
		{
			AttachToGun();
		}
	}
}
