using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LomotionController : MonoBehaviour
{
    public XRController leftTeleportRay;
    public XRController rightTeleportRay;

    public XRRayInteractor leftInteractorRay;
    public XRRayInteractor rightInteractorRay;


    public InputHelpers.Button teleportActivationButton;
    public float activationThreshould = 0.1f;

    public bool EnableLeftTeleport { get; set; } = true; // 为了在手上拿东西的时候不允许teleport
    public bool EnableRightTeleport { get; set; } = true;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3();
        Vector3 norm = new Vector3();
        int index = 0;
        bool validTarget = false;

        if(leftTeleportRay)
        {
            // 如果选择射线检测到了可交互物体，则不允许传送
            bool isLeftInteractorHovering = leftInteractorRay.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget);
           
            bool res = EnableLeftTeleport && CheckIfActivated(leftTeleportRay) && !isLeftInteractorHovering;
            leftTeleportRay.gameObject.SetActive(res);
            leftInteractorRay.gameObject.SetActive(!res);

        }

        if (rightTeleportRay)
        {
            bool isRightInteractorHovering = rightInteractorRay.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget);
            bool res = EnableRightTeleport && CheckIfActivated(rightTeleportRay) && !isRightInteractorHovering;
            rightTeleportRay.gameObject.SetActive(res);
            rightInteractorRay.gameObject.SetActive(!res);
        }
    }

    public bool CheckIfActivated(XRController controller)
    {
        // if up to some threshould
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshould);
        return isActivated;
    }
}
