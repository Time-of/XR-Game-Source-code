using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/**
 * @author 이성수
 * @brief XRDirectInteractor를 오버라이드하여 사용하는 클래스입니다.
 * @details bIsGrabbed라면(이미 그랩중인 물체라면) Select할 수 없게 합니다.
 * @since 2023-03-30
 */
public class ControllerDirectInteractor : XRDirectInteractor
{
	/*
	public override bool CanSelect(IXRSelectInteractable interactable)
	{
		CustomGrabInteractable CustomGrab = (CustomGrabInteractable)interactable;

		if (CustomGrab != null && CustomGrab.bIsGrabbed)
		{
			return false;
		}

		return base.CanSelect(interactable);
	}
	*/
}
