using UnityEngine;

public class TPSCameraControl : MonoBehaviour
{
    [Header("目标引用")]
    public Transform player;         // 拖入玩家物体
    
    [Header("视角偏移")]
    public Vector3 offset = new Vector3(0, 1.5f, -3f); // 初始偏移（x左右, y上下, z前后）
    
    [Header("旋转设置")]
    public float sensitivityMultiplier = 1.0f; // 灵敏度倍率
    public float baseSensitivity = 0.2f;       // 基础感光度
    public float minY = -20f;                  // 俯视角度限制
    public float maxY = 80f;                   // 仰视角度限制

    private float currentY = 0f;               // 上下累计旋转

    void Start()
    {
        // 1. 初始化灵敏度（读取你之前在设置面板存的数值）
        UpdateSensitivity();

        // 2. 锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    // 使用 LateUpdate 确保在玩家移动逻辑执行完后再更新相机位置，彻底消除抖动
    void LateUpdate()
    {
        if (player == null || Time.timeScale == 0 || Cursor.lockState != CursorLockMode.Locked || Cursor.visible) return;

        // 1. 左右旋转：不再自己加减 Mouse X！
        // 直接读取玩家当前的 Y 轴旋转角度（确保永远同步）
        float currentYRotation = player.eulerAngles.y;

        // 2. 上下旋转：相机依然自己负责（因为身体不需要抬头）
        float mouseY = Input.GetAxis("Mouse Y") * baseSensitivity * sensitivityMultiplier;
        currentY -= mouseY;
        currentY = Mathf.Clamp(currentY, minY, maxY);

        // 3. 计算旋转：左右使用玩家的角度，上下使用自己的计算结果
        Quaternion rotation = Quaternion.Euler(currentY, currentYRotation, 0);

        // 4. 更新位置（绕着玩家转）
        transform.position = player.position + rotation * offset;

        // 5. 盯着玩家看
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }

    // 供 PauseManager 调用，当玩家改完灵敏度返回游戏时同步
    public void UpdateSensitivity()
    {
        // 读取键名为 "Sensitivity" 的值，如果没有存过，默认为 1.0
        sensitivityMultiplier = PlayerPrefs.GetFloat("Sensitivity", 1.0f);
        Debug.Log(gameObject.name + " 已同步灵敏度: " + sensitivityMultiplier);
    }
}
