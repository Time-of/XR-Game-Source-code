using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/**
 * @author 이성수
 * @brief 두 손을 가지고 상호작용 가능한 그랩 인터랙터블 클래스입니다.
 * @since 2023-03-31
 */
public class TwoHandedGrabInteractable : XRGrabInteractable
{
	[SerializeField]
	private List<XRSimpleInteractable> OtherHandInteractPositions = new List<XRSimpleInteractable>();

	private IXRSelectInteractor OtherHandInteractor;

	private Quaternion InitialFirstHandLocalRotation;



	private void Start()
	{
		foreach (XRSimpleInteractable interactPosition in OtherHandInteractPositions)
		{
			interactPosition.selectEntered.AddListener(OnOtherHandGrabbed);
			interactPosition.selectExited.AddListener(OnOtherHandUngrabbed);
		}
	}



	public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
	{
		if (OtherHandInteractor != null && firstInteractorSelecting != null)
		{
			Transform FirstGrabAttachTransform = firstInteractorSelecting.GetAttachTransform(firstInteractorSelecting.firstInteractableSelected);
			FirstGrabAttachTransform.rotation =
				Quaternion.LookRotation(OtherHandInteractor.GetAttachTransform(OtherHandInteractor.firstInteractableSelected).position - FirstGrabAttachTransform.position);
		}

		base.ProcessInteractable(updatePhase);
	}



	public void OnOtherHandGrabbed(SelectEnterEventArgs args)
	{
		OtherHandInteractor = args.interactorObject;
	}



	public void OnOtherHandUngrabbed(SelectExitEventArgs args)
	{
		OtherHandInteractor = null;
	}



	protected override void OnSelectEntered(SelectEnterEventArgs args)
	{
		InitialFirstHandLocalRotation = args.interactorObject.GetAttachTransform(args.interactableObject).localRotation;

		base.OnSelectEntered(args);
	}



	protected override void OnSelectExited(SelectExitEventArgs args)
	{
		args.interactorObject.GetAttachTransform(args.interactableObject).localRotation = InitialFirstHandLocalRotation;

		OtherHandInteractor = null;

		base.OnSelectExited(args);
	}



	public override bool IsSelectableBy(IXRSelectInteractor interactor)
	{
		bool bIsAlreadyGrabbed = isSelected && !interactor.Equals(firstInteractorSelecting);

		return !bIsAlreadyGrabbed && base.IsSelectableBy(interactor);
	}
}
