using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



/**
 * @author 이성수
 * @brief XR Ray Interactor를 상속한 커스텀 레이 인터랙터 클래스입니다.
 * @details 주로 조작감의 향상을 위한 클래스입니다.
 * @since 2023-04-02
 */
public class CustomXRRayInteractor : XRRayInteractor
{
	private bool bCanSelectDirectly = false;

	// 0.5초까지만 선택 가능
	private float CanSelectTimeLimit = 0.5f;

	private float ElapsedTime = 0.0f;



	private void Update()
	{
		if (isSelectActive)
		{
			ElapsedTime += Time.deltaTime;

			bCanSelectDirectly = ElapsedTime <= CanSelectTimeLimit;
		}
		else
		{
			ElapsedTime = 0.0f;

			bCanSelectDirectly = false;
		}
	}



	public override bool CanSelect(IXRSelectInteractable interactable)
	{
		// 시간제한을 넘기지 않은 경우 선택 가능
		return bCanSelectDirectly && base.CanSelect(interactable);
	}
}
