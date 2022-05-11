using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class InteractiveController : MonoBehaviour
{
    public XRRayInteractor leftInteractorRay;
    public XRRayInteractor rightInteractorRay;
    public XRSimpleInteractable frameSimpleInteractable;
    public SnapTurnProviderBase SnapTurenProviderScript;

    // the list of hovering interactors
    private List<XRBaseInteractor> hoverInteractors;

    private bool isSendHaptic = true;
    private bool isFirstGrip = true;
    
    private Vector3 hitToFrame;

    private Vector3 startEuler;
    private Vector3 lastEuler;

    private Vector3 xAxis;
    private Vector3 yAxis;
    private Vector3 zAxis;
    private Vector3 hitPos;

    private float xdistance;
    private float ydistance;
    private float zdistance;
    private Vector3 cameraStartPosition;

    private Quaternion lastFrameQuaternion;
    private Quaternion lastAxisQuaternion;

    private float accumalteRotate;
    private float accumalteDistance;
    private Vector3 accumalteEuler;


    // for two controller event
    private Vector3 leftStartPos;
    private Vector3 leftHitPos;
    private  Vector3  rightStartPos;
    private Vector3 rightHitPos;
    private float startdiff;

    // Start is called beforze the first frame update
    void Start()
    {
        xAxis = new Vector3(1f, 0f, 0f);
        yAxis = new Vector3(0f, 1f, 0f);
        zAxis = new Vector3(0f, 0f, 1f);

        accumalteRotate = 0f;
        accumalteDistance = 0f; accumalteEuler = new Vector3(0f, 0f, 0f); ;
    }

    // Update is called once per frame
    void Update()
    {
        // check for enabling snap turn
        Vector3 pos_;
        Vector3 norm_;
        int index_;
        bool validTarget_;
        hoverInteractors = frameSimpleInteractable.hoveringInteractors;

        // 如果左右射线interactor都没有hover到东西，允许做snap turn
        bool isRightInteractorHovering = rightInteractorRay.TryGetHitInfo(out pos_, out norm_, out index_, out validTarget_);
        bool isLeftInteractorHovering = leftInteractorRay.TryGetHitInfo(out pos_, out norm_, out index_, out validTarget_);
        if (isRightInteractorHovering == false && isLeftInteractorHovering == false)
            SnapTurenProviderScript.enabled = true;

        // 如果该物体仅仅被一个interactor hover了
        if (hoverInteractors.Count == 1)
        {
            SingleControllerEvent();


        }
        else if(hoverInteractors.Count == 2)
        {
            // get the hover interactor
            XRBaseInteractor interactor1;
            XRBaseInteractor interactor2;
            if (hoverInteractors[0].name == "Right Ray Interactor")
            {
                interactor1 = hoverInteractors[1];
                interactor2 = hoverInteractors[0];
            }
            else
            {
                interactor1 = hoverInteractors[0];
                interactor2 = hoverInteractors[1];
            }
            
            // 获得controller
            XRController controller1 = interactor1.GetComponent<XRController>();
            XRController controller2 = interactor2.GetComponent<XRController>();
            // 获得device，为了具体的得到手柄输入
            InputDevice device1 = controller1.inputDevice;
            InputDevice device2 = controller2.inputDevice;

            if (device1.TryGetFeatureValue(CommonUsages.grip, out float gripValue1)
                && device2.TryGetFeatureValue(CommonUsages.grip, out float gripValue2) && gripValue1 >= 0.1 && gripValue2 >= 0.2)
            {

                // 如果第一次抓到，记录一些数据
                if (isFirstGrip)
                {
                    leftStartPos = controller1.transform.position;
                    rightStartPos = controller2.transform.position;

                    Vector3 framePosition = this.transform.position;



                    // 获得ray击中的坐标
                    leftHitPos = new Vector3();
                    rightHitPos = new Vector3();
                    Vector3 norm1 = new Vector3();
                    int index1 = 0;
                    bool validTarget1 = false;
                    rightInteractorRay.TryGetHitInfo(ref rightHitPos, ref norm1, ref index1, ref validTarget1);
                    leftInteractorRay.TryGetHitInfo(ref leftHitPos, ref norm1, ref index1, ref validTarget1);

                    startdiff = Vector3.Distance(leftHitPos, rightHitPos);

                    // 禁止snap turn
                    SnapTurenProviderScript.enabled = false;
                    isFirstGrip = false;
                }

                float distance1 = Vector3.Distance(leftHitPos, leftStartPos);
                float distance2 = Vector3.Distance(rightHitPos, rightStartPos);

                Vector3 newPos1 = controller1.transform.position + controller1.transform.forward * distance1;
                Vector3 newPos2 = controller2.transform.position + controller2.transform.forward * distance2;

                float diff = Vector3.Distance(newPos1, newPos2);

                float efficiency = diff / startdiff;
                this.transform.localScale = new Vector3(efficiency, efficiency, efficiency);


                if (gripValue1 <= 0.1 && gripValue2<=0.1)
                {
                    isSendHaptic = true;
                    isFirstGrip = true;
                    SnapTurenProviderScript.enabled = true;
                    accumalteDistance = 0f;
                    //accumalteRotate = 0f;
                    //accumalteEuler = new Vector3(0f, 0f, 0f);

                }
            }
        }


    }



    private void FixedUpdate()
    {

    }


    void SingleControllerEvent()
    {
        // get the hover interactor
        XRBaseInteractor interactor = hoverInteractors[0];
        // 获得controller
        XRController controller = interactor.GetComponent<XRController>();
        // 获得transform组件
        Transform controllerTransform = interactor.GetComponent<Transform>();
        // 获得device，为了具体的得到手柄输入
        InputDevice device = controller.inputDevice;

        // 查看grip输入
        if (device.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            if (gripValue > 0.1)
            {
                // 如果第一次按下，送震动
                if (isSendHaptic)
                {
                    device.SendHapticImpulse(0, 0.2f, 0.06f);
                    isSendHaptic = false;
                }

                // 如果第一次抓到，记录一些数据
                if (isFirstGrip)
                {
                    cameraStartPosition = controllerTransform.position;
                    Vector3 framePosition = this.transform.position;

                    // 获得ray击中的坐标
                    hitPos = new Vector3();
                    Vector3 norm = new Vector3();
                    int index = 0;
                    bool validTarget = false;
                    if (interactor.name == "Right Ray Interactor")
                        rightInteractorRay.TryGetHitInfo(ref hitPos, ref norm, ref index, ref validTarget);
                    else
                        leftInteractorRay.TryGetHitInfo(ref hitPos, ref norm, ref index, ref validTarget);


                    // 从碰撞点到坐标系的向量
                    hitToFrame = framePosition - hitPos;
                    lastEuler = controllerTransform.eulerAngles;

                    // calculate projection distance
                    Vector3 yRotateAxis_ = controller.transform.rotation * yAxis;
                    Vector3 xRotateAxis_ = controller.transform.rotation * xAxis;
                    Vector3 zRotateAxis_ = controller.transform.rotation * zAxis;

                    // hitToFrame在手柄的坐标系里面的投影坐标
                    xdistance = Vector3.Dot(hitToFrame, xRotateAxis_);
                    ydistance = Vector3.Dot(hitToFrame, yRotateAxis_);
                    zdistance = Vector3.Dot(hitToFrame, zRotateAxis_);


                    // 禁止snap turn
                    SnapTurenProviderScript.enabled = false;
                    isFirstGrip = false;
                }

                // 获得ray击中的新坐标
                Vector3 newHitPos = new Vector3();

                // 计算当前手柄坐标系
                Vector3 xRotateAxis = controller.transform.rotation * xAxis;
                Vector3 yRotateAxis = controller.transform.rotation * yAxis;
                Vector3 zRotateAxis = controller.transform.rotation * zAxis;

                newHitPos = hitPos + accumalteDistance * zRotateAxis;
                //Debug.Log(newHitPos);
                Vector3 forward = controller.transform.forward;
                Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
                Vector3 right = Vector3.Cross(up, forward);
                // calculate roration
                // ------------------
                // roration 1, 累计的手柄旋转欧拉角（Track 手柄旋转）
                Vector3 currentEuler = controllerTransform.eulerAngles;
                accumalteEuler += currentEuler - lastEuler; // 累计欧拉角不复原是为了第二次碰到也是上一次离开的状态
                transform.RotateAround(newHitPos, right, (currentEuler - lastEuler).x);
                transform.RotateAround(newHitPos, up, (currentEuler - lastEuler).y);
                transform.RotateAround(newHitPos, forward, (currentEuler - lastEuler).z);




                // roration 2 + primary2DAxis输入处理
                device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DValue);
                
                if (Mathf.Abs(primary2DValue[0]) > 0.7f)
                {
                    accumalteRotate += (primary2DValue[0] * 5f);
                    transform.Rotate(yAxis, primary2DValue[0] * 5f); //局部旋转
                }


                // calculate position and rotation 2
                // ------------------
                // 初始距离
                float distance = Vector3.Distance(hitPos, cameraStartPosition);

                // 现在的hitToFrame应该是多少
                Vector3 add = xdistance * xRotateAxis + ydistance * yRotateAxis + zRotateAxis * zdistance;

                // 计算primary2DAxis所带来的的累计距离（forward方向）
                if (Mathf.Abs(primary2DValue[1]) > 0.7f)
                    accumalteDistance += primary2DValue[1] / 15.0f;

                // 禁止太近
                if (accumalteDistance < -distance + 0.2f)
                    accumalteDistance = -distance + 0.2f;


                // 更新位置
                transform.position = controller.transform.position + zRotateAxis * (distance + accumalteDistance) + add;


                

                //Debug.Log("distance:" + distance.ToString("f4"));
                //Debug.Log("accumlateDistance:" + accumalteDistance.ToString("f4") + "primary2D:" + primary2DValue.ToString("f4"));
                lastEuler = controllerTransform.eulerAngles;
            }

            // 松开了grip， 设置一些东西
            if (gripValue <= 0.1)
            {
                isSendHaptic = true;
                isFirstGrip = true;
                SnapTurenProviderScript.enabled = true;
                accumalteDistance = 0f;
                //accumalteRotate = 0f;
                //accumalteEuler = new Vector3(0f, 0f, 0f);

            }
        }

    }



    // 这段代码没用
    private void OutputInpectorEulers(Transform trans, out float x, out float y, out float z)
    {
        Vector3 angle = trans.eulerAngles;
        x = angle.x;
        y = angle.y;
        z = angle.z;

        if (Vector3.Dot(trans.up, Vector3.up) >= 0f)
        {
            if (angle.x >= 0f && angle.x <= 90f)
            {
                x = angle.x;
            }
            if (angle.x >= 270f && angle.x <= 360f)
            {
                x = angle.x - 360f;
            }
        }
        if (Vector3.Dot(trans.up, Vector3.up) < 0f)
        {
            if (angle.x >= 0f && angle.x <= 90f)
            {
                x = 180 - angle.x;
            }
            if (angle.x >= 270f && angle.x <= 360f)
            {
                x = 180 - angle.x;
            }
        }

        if (angle.y > 180)
        {
            y = angle.y - 360f;
        }

        if (angle.z > 180)
        {
            z = angle.z - 360f;
        }




    }


    public float CheckAngle(float value)

    {

        float angle = value - 180;




        if (angle > 0)

            return angle - 180;




        return angle + 180;

    }
}
