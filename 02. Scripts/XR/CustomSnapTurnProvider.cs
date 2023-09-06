using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;



/**
 * @author 이성수
 * @brief 플레이어를 DOTween을 이용해 부드럽게 회전이 가능하도록 만든 클래스입니다.
 * @details 주로 멀미를 제거하기 위해 만든 클래스입니다. (기존의 Snap Turn Provider는 딱딱한 움직임)
 * @since 2023-04-02
 */
public class CustomSnapTurnProvider : MonoBehaviour
{
    private XROrigin Origin;

    [SerializeField]
    private float TurnDuration = 0.2f;

    [SerializeField]
    private float TurnAngle = 30.0f;

    [SerializeField]
    private float InputDeadZone = 0.75f;

    private Tweener RotateTweener;

    // 회전 중 시야 좁히기, 포스트 프로세스 볼륨이 있는 경우만 가능
    [SerializeField]
    private bool bTryNarrowSight = true;

    private Volume PostProcessVolume;

    private Vignette PostProcessVignette;

    private bool bCanTurn = true;



    private void Start()
    {
        Origin = GetComponent<XROrigin>();
        PostProcessVolume = FindObjectOfType<Volume>();
    }



    public void GetInput(float AxisInputValue)
    {
        if (Mathf.Abs(AxisInputValue) >= InputDeadZone)
            HandleTurn(AxisInputValue < 0);
        else
            bCanTurn = true;
    }



    private void HandleTurn(bool bIsLeft)
    {
        // 이미 회전 진행중이거나 회전이 불가능한 경우 실행하지 않음
        if ((RotateTweener != null && RotateTweener.IsPlaying()) || !bCanTurn) return;

        bCanTurn = false;
        ActivateNarrowSight();
        Turn(bIsLeft ? -TurnAngle : TurnAngle);
    }



    private void Turn(float YawAmount)
    {
        float TargetYaw = Origin.transform.localEulerAngles.y + YawAmount;
        RotateTweener = Origin.transform.DOLocalRotate(new Vector3(0.0f, TargetYaw, 0.0f), TurnDuration)
            .SetEase(Ease.OutSine);
        RotateTweener.OnComplete(DeactivateNarrowSight);
    }



    private void ActivateNarrowSight()
    {
        if (bTryNarrowSight && PostProcessVolume != null && PostProcessVolume.profile.TryGet(out PostProcessVignette))
        {
            PostProcessVignette.intensity.Override(1.0f);
        }
    }



    private void DeactivateNarrowSight()
    {
        if (bTryNarrowSight && PostProcessVolume != null && PostProcessVolume.profile.TryGet(out PostProcessVignette))
        {
            PostProcessVignette.intensity.Override(0.35f);
        }
    }
}
