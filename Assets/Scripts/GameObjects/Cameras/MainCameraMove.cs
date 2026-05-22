using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：控制第三人称下摄像机的移动和旋转
//*************************

public class MainCameraMove : MonoBehaviour
{
    //目标
    public Transform target;
    //摄像机基础设置
    public float height = 1.5f;//摄像机与物体竖直距离
    public float distance = 2.5f;//摄像机与物体水平距离
    public float heightDamping = 2.0f;//高度阻尼
    public float rotateDamping = 5f;//旋转阻尼
    //public float offset = 3f;//摄像头指向的位置
    //摄像机跟随
    public float mouseSensitivityX = 2.0f;//鼠标水平灵敏度
    public float mouseSensitivityY = 1.0f;//鼠标竖直灵敏度
    //private float currentHeight;//当前高度
    private Vector3 rotationOffset;//旋转偏移
    private bool isFixed;//是否在重置视角
    void Start()
    {
        //初始化旋转偏移
        rotationOffset = target.forward;
        //初始化摄像机高度
        transform.position = target.transform.position + new Vector3(0, 1.5f, -2.5f);
        //currentHeight = 0;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

    }

    void LateUpdate()
    {
        if (!target)
        {
            return;
        }
        //实现中键重置向前视角
        if (Input.GetMouseButtonDown(2))
        {
            isFixed = true;
            FixMove();
        }
        //采集鼠标水平位移
        if (!isFixed)
        {
            NormalMove();
        }
        else
        {
            OnlyAfter();
        }
    }
    /// <summary>
    /// 常规条件下的移动
    /// </summary>
    public void NormalMove()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
        float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivityY;
        //创建一个绕target旋转的水平位移应用于rotationOffset
        rotationOffset = Quaternion.AngleAxis(mouseX, Vector3.up) * rotationOffset;
        //创建一个绕自身x轴旋转的竖直位移应用于rotationOffset
        rotationOffset = Quaternion.AngleAxis(mouseY, transform.right) * rotationOffset;
        //计算所需高度
        //float wantHeight = height;
        //currentHeight = Mathf.Lerp(currentHeight, wantHeight, heightDamping * Time.deltaTime);
        //设置摄像机位置
        Vector3 newPos = target.position;
        newPos -= rotationOffset * distance;
        newPos.y += height;
        transform.position = newPos;

        transform.LookAt(target);
    }
    /// <summary>
    /// 仅仅跟随目标（用作处理旋转过程中的主角移动）
    /// </summary>
    public void OnlyAfter()
    {

    }
    /// <summary>
    /// 旋转
    /// </summary>
    public void FixMove()
    {
        Vector3 forward = target.forward;
        rotationOffset = forward;
        isFixed = false;
    }
}
