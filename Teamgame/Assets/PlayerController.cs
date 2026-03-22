using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayController : MonoBehaviour
{

    [Header("移动")]
    public float walkSpeed = 5f;
    public float gravity = -9.81f * 2f;
    public float jumpHeight = 1.5f;

    [Header("视角")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    [Header("高级跳跃")]
    public float groundedBufferTime = 0.2f; // 离开地面后多久内还能跳
    private float groundedBufferTimer = 0f; // 计时器

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) cameraTransform = cam.transform;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
        ApplyGravityAndJump();
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (x == 0 && z == 0) return;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * z + right * x).normalized;
        controller.Move(moveDirection * walkSpeed * Time.deltaTime);
    }

    void ApplyGravityAndJump()
    {
        // 1. 获取原始的地面检测结果
        bool rawIsGrounded = controller.isGrounded;

        // 2. 处理地面缓冲逻辑
        if (rawIsGrounded)
        {
            // 如果当前在地面，重置计时器
            groundedBufferTimer = groundedBufferTime;
            isGrounded = true;
        }
        else
        {
            // 如果不在地面，计时器开始倒数
            groundedBufferTimer -= Time.deltaTime;

            // 如果计时器大于0，说明还在缓冲期内，逻辑上视为“在地面”
            // 但要注意：如果已经跳起来了（速度向上），就不应该再视为在地面，防止二段跳
            if (groundedBufferTimer > 0 && velocity.y <= 0)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }

        // 3. 重力应用优化
        // 如果逻辑上在地面，且速度向下，强制将垂直速度设为一个小负值，确保贴地
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            // groundedBufferTimer = 0f; 
        }

        // 4. 跳跃逻辑
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            // 跳跃成功后，立即清空缓冲，防止空中再次触发
            groundedBufferTimer = 0f;
            isGrounded = false;
        }

        // 5. 应用重力和移动
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}