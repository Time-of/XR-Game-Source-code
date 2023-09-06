using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * 작성자: 20181220 이성수
 * HUD가 항상 카메라 위치를 향하도록 조정합니다.
 */
public class HudBillboard : MonoBehaviour
{
	void LateUpdate()
	{
		transform.rotation = Camera.allCameras[0].transform.rotation;
	}
}
