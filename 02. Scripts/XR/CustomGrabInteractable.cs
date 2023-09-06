using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



/**
 * @author 이성수
 * @brief XRGrabInteractable을 오버라이드하여 사용하는 클래스입니다.
 * @since 2023-03-30
 */
public class CustomGrabInteractable : XRGrabInteractable
{
	public override bool IsSelectableBy(IXRSelectInteractor interactor)
	{
		bool bIsAlreadyGrabbed = isSelected && !interactor.Equals(firstInteractorSelecting);

		return !bIsAlreadyGrabbed && base.IsSelectableBy(interactor);
	}
}
