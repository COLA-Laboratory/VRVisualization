using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CubeTest : MonoBehaviour
{
    public XRRayInteractor leftInteractorRay;
    public XRRayInteractor rightInteractorRay;
    public XRSimpleInteractable cubeSimpleInteractable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cubeSimpleInteractable.isHovered && cubeSimpleInteractable.isSelected)
        {
            Vector3 pos = new Vector3();
            Vector3 norm = new Vector3();
            int index = 0;
            bool validTarget = false;
            rightInteractorRay.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget);
            Debug.Log("Right Controller:");
            Debug.Log("pos:" + pos + " norm:" + norm + " index:" + index + " validTarget:" + validTarget);
        }
        
    }

    public void OnSelect()
    {
        Vector3 pos = new Vector3();
        Vector3 norm = new Vector3();
        int index = 0;
        bool validTarget = false;

        Debug.Log("Cube: i'm selected");
        bool isLeftInteractorHovering = leftInteractorRay.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget);
        bool isRightInteractorHovering = rightInteractorRay.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget);

        if(isRightInteractorHovering)
        {
            Debug.Log("Right Controller:");
            Debug.Log("pos:" + pos + " norm:" + norm + " index:" + index+ " validTarget:" + validTarget);
        }

    }
}
