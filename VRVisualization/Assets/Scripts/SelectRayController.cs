using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SelectRayController : MonoBehaviour
{
    public XRController leftSelectRay;
    public XRController rightSelectRay;

    public bool EnableLeftSelect { get; set; } = true;
    public bool EnableRightSelect { get; set; } = true;

    // Update is called once per frame
    void Update()
    {
        if (leftSelectRay)
        {
            leftSelectRay.gameObject.SetActive(EnableLeftSelect);
        }

        if (rightSelectRay)
        {
            rightSelectRay.gameObject.SetActive(EnableRightSelect);
        }
    }


}
