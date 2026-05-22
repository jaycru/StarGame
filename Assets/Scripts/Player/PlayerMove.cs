using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：
//*************************

public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera mainCamera;
    public float speed;//移速
    public float rotateSpeed;
    public float jumpForce;//跳跃力量
    private Vector3 forward;//当前摄像机朝向
    private Vector3 right;//当前摄像机右方
    private Vector3 moveDirection;//所需位移
    //public MainAnimationController animationcontroller;//动画控制器
    void Start()
    {
        speed = 4f;
        rotateSpeed = 200f;
        jumpForce = 10f;
        forward = mainCamera.transform.forward;
        right = mainCamera.transform.right;
        moveDirection = new Vector3(0, 0, 0);
        //animationcontroller = GetComponent<MainAnimationController>();
        FreezeRotation.FreezeXAndZ(gameObject);
    }

    void Update()
    {
        //实现物体平移转动
        HandleMove();
        //实现物体跳跃
        HandleJump();
    }
    /// <summary>
    /// 实现物体的移动和转动
    /// </summary>
    public void HandleMove()
    {
        moveDirection = Vector3.zero;//至关重要！如果没有就会运动累积
        forward = mainCamera.transform.forward;
        right = mainCamera.transform.right;
        //实现物体WASD的平动
        float testStright = Input.GetAxis("Vertical");
        float testLeftOrRight = Input.GetAxis("Horizontal");
        //if (testStright != 0 || testLeftOrRight != 0)
        //{
        //    animationcontroller.MovePlay();
        //}
        moveDirection = forward * testStright + right * testLeftOrRight;
        moveDirection.y = 0;
        transform.position += moveDirection * Time.deltaTime * speed;
        //实现物体朝向的转动（八方向锁定）
        if (moveDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(testStright, -testLeftOrRight) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, angle + mainCamera.transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }
    /// <summary>
    /// 实现物体跳跃
    /// </summary>
    public void HandleJump()
    {
        if (!CheckGrounded())
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rigidbody rb = transform.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    /// <summary>
    /// 检测是否位于地面
    /// </summary>
    public bool CheckGrounded()
    {
        float rayLength = 7f;
        RaycastHit hit;
        int groundLayer = 1 << LayerMask.NameToLayer("Ground");
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, groundLayer);
        //Debug.DrawRay(transform.position, Vector3.down * rayLength,
        //    isGrounded ? Color.green : Color.red);
        return isGrounded;
    }
    /// <summary>
    /// 主角转向屏幕正向
    /// </summary>
    public void RotateForward()
    {
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0f;
        transform.forward = forward;
    }
}
