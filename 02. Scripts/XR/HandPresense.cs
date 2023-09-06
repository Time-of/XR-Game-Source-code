using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;



/**
 * @author 이성수
 * @brief XR 기기에서 손을 인식하게 해 주는 클래스
 * @since 2023-03-14
 */
public class HandPresense : MonoBehaviour
{
	[SerializeField]
	private InputDeviceCharacteristics ControllerType;

	[SerializeField]
	private List<GameObject> ControllerModelPrefabList = new List<GameObject>();

	protected InputDevice TargetDevice;

	private GameObject SpawnedControllerModel;

	[SerializeField]
	private GameObject HandModelPrefab;

	protected bool bUseHand = false;

	private GameObject SpawnedHandModel;

	private Animator SpawnedHandModelAnimComp;

	[SerializeField]
	private bool bDefaultUseHand = false;



	public void SetUseHand(bool NewUseHand)
	{
		bUseHand = NewUseHand;
		//SpawnedControllerModel?.SetActive(!bUseHand);
		SpawnedHandModel?.SetActive(bUseHand);
	}



	protected virtual void Start()
	{
		// 컨트롤러는 상당히 지연된 후 인식되므로 이런 방식을 사용
		StartCoroutine(TryGetXrDevices());
    }



    protected virtual void Update()
    {
        if (bUseHand)
		{
            if (TargetDevice.TryGetFeatureValue(CommonUsages.grip, out float GripValue))
			{
                SpawnedHandModelAnimComp.SetFloat("Grip", GripValue);
            }
			else
			{
                SpawnedHandModelAnimComp.SetFloat("Grip", 0);
            }

            if (TargetDevice.TryGetFeatureValue(CommonUsages.trigger, out float TriggerValue))
            {
                SpawnedHandModelAnimComp.SetFloat("Trigger", TriggerValue);
            }
            else
            {
                SpawnedHandModelAnimComp.SetFloat("Trigger", 0);
            }
        }
    }



    private IEnumerator TryGetXrDevices()
	{
		while (!TargetDevice.isValid)
		{
			List<InputDevice> xr_Devices = new List<InputDevice>();

			// 비트 플래그 타입
			InputDevices.GetDevicesWithCharacteristics(ControllerType, xr_Devices);

			foreach (InputDevice device in xr_Devices)
			{
				Debug.Log(device.name + ", " + device.characteristics + "탐지됨");
			}

			if (xr_Devices.Count > 0)
			{
				TargetDevice = xr_Devices[0];
				GameObject FoundControllerModelPrefab = ControllerModelPrefabList.Find(ControllerPrefab => ControllerPrefab.name == TargetDevice.name);

				if (FoundControllerModelPrefab != null)
				{
					SpawnedControllerModel = Instantiate(FoundControllerModelPrefab, transform);
				}
				else
				{
					Debug.LogWarning("연결된 컨트롤러 기기와 동일한 모델을 찾지 못해 기본 모델로 대체함, 모델명: " + TargetDevice.name);
					SpawnedControllerModel = Instantiate(ControllerModelPrefabList[0], transform);
				}

				SpawnedHandModel = Instantiate(HandModelPrefab, transform);
				SpawnedHandModelAnimComp = SpawnedHandModel.GetComponent<Animator>();
                SpawnedControllerModel?.SetActive(false);

                SetUseHand(bDefaultUseHand);
			}


            yield return new WaitForSeconds(0.1f);
		}
	}
}
