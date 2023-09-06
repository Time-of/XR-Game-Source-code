using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

/**
 * @author 이성수
 * @brief 레이캐스트로 위치에 홀로그램을 비춰주는 역할의 클래스입니다.
 * @since 2023-03-26
 */
public class RayPreviewHologram : MonoBehaviour
{
	[SerializeField, ReadOnlyProperty]
	private PlacePreviewHologram HologramObject;

	[SerializeField]
	private float RayDistance = 5.0f;

	[SerializeField]
	private LayerMask RayLayerMask;

	private InputDevice TargetDevice;

	[SerializeField]
	private XRNode InputDeviceType = XRNode.RightHand;



	private void Start()
	{
		// 컨트롤러는 상당히 지연된 후 인식되므로 이런 방식을 사용
		StartCoroutine(TryGetXrDevices());
	}



	private IEnumerator TryGetXrDevices()
	{
		while (!TargetDevice.isValid)
		{
			TargetDevice = InputDevices.GetDeviceAtXRNode(InputDeviceType);

			yield return new WaitForSeconds(0.1f);
		}

		Debug.Log("RayPreviewHologram: " + TargetDevice + " 찾음!!");
	}



	private void Update()
	{
		if (HologramObject == null) return;

		if (Physics.Raycast(transform.position, transform.forward, out RaycastHit HitInfo, RayDistance, RayLayerMask, QueryTriggerInteraction.Ignore))
		{
			HologramObject.transform.position = HitInfo.point;
		}

		//TargetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 InputAxisValue);
		//HologramObject.transform.rotation *= Quaternion.Euler(0.0f, InputAxisValue.x * Time.deltaTime, 0.0f);

		TargetDevice.TryGetFeatureValue(CommonUsages.trigger, out float TriggerValue);

		if (TriggerValue >= 0.4f && HologramObject.bIsPlaceable)
		{
			GameplayHelperLibrary.SpawnObject(HologramObject.ObjectPrefabToPlace.InternalName, HologramObject.transform.position, HologramObject.transform.rotation);

			HologramObject.ReturnToPool();
			HologramObject = null;
		}
	}



	/// <summary>
	/// BuyTurretUI의 OnBoughtTurretEvent에 등록해 사용
	/// </summary>
	/// <param name="PreviewHologramPrefab">미리보기 홀로그램 프리팹</param>
	public void SpawnHologram(PlacePreviewHologram PreviewHologramPrefab)
	{
		if (HologramObject != null)
		{
			Debug.LogWarning("이미 홀로그램 " + HologramObject + "이 있음에도 홀로그램을 스폰하려 했습니다!!");
			return;
		}

		Vector3 InitialPosition = GameplayHelperLibrary.GetPlayer().transform.position + GameplayHelperLibrary.GetPlayer().transform.forward * 1.5f;
		HologramObject = (PlacePreviewHologram)GameplayHelperLibrary.SpawnObject(PreviewHologramPrefab.InternalName, InitialPosition, Quaternion.identity);
	}
}
