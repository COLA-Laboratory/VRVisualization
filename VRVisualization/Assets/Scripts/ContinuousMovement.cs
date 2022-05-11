using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ContinuousMovement : MonoBehaviour
{
    public float speed = 1;
    public XRNode inputSource;
    public float gravity = -9.81f;
    public LayerMask groundLayer;   // 设置哪些layer是ground，来做是否到groud的射线检测
    public float addtionalHeight = 0.2f;

    private float fallingSpeed;
    private XRRig rig;
    private Vector2 inputAxis;
    private CharacterController character;
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XRRig>();
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    // 让character controller跟随camera
    void CapsuleFollowHeadset()
    { 
        character.height = rig.cameraInRigSpaceHeight + addtionalHeight;

        //Debug.Log("world space:" + rig.cameraGameObject.transform.position);
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position); //局部坐标
        //Debug.Log("local space:" + capsuleCenter);

        character.center = new Vector3(capsuleCenter.x,character.height/2 + character.skinWidth, capsuleCenter.z);

    }

    private void FixedUpdate()
    {
        // 让character controller跟着头
        CapsuleFollowHeadset();

        Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y); // 根据视角校正一下行进方向

        character.Move(direction * Time.fixedDeltaTime * speed);

        //gravity
        bool isGrounded = CheckIfGrounded();
        if (isGrounded)
            fallingSpeed = 0;
        else
            fallingSpeed += gravity * Time.fixedDeltaTime;

        character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
    }

    bool CheckIfGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.1f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }
}
