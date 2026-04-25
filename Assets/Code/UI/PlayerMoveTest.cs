using UnityEngine;

public class PlayerMoveTest : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public CharacterController controller; // 建议改用这个组件处理碰撞

    [Header("旋转设置")]
    public float baseSensitivity = 0.2f; // 删掉deltaTime后，基础值要调小（比如2-5）
    private float sensitivityMultiplier = 1.0f;

    void Start()
    {
        UpdateSensitivity();
        // 确保获取到组件
        if (controller == null) controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Time.timeScale == 0 || Cursor.lockState != CursorLockMode.Locked || Cursor.visible) return;

        // 1. 左右旋转：由玩家身体独立完成
        float mouseX = Input.GetAxis("Mouse X") * baseSensitivity * sensitivityMultiplier;
        transform.Rotate(Vector3.up * mouseX); 

        // 2. 移动逻辑：始终沿着玩家自己的 forward 走
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = (transform.forward * v + transform.right * h).normalized;

        if (controller != null)
            controller.Move(move * moveSpeed * Time.deltaTime);
        else
            transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }

    public void UpdateSensitivity()
    {
        // 对应你之前在 UI 存的键名
        sensitivityMultiplier = PlayerPrefs.GetFloat("Sensitivity", 1.0f);
        Debug.Log(gameObject.name + " 已同步灵敏度: " + sensitivityMultiplier);
    }
}
