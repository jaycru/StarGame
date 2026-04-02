using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    [Header("准星线条引用")]
    public RectTransform top;
    public RectTransform bottom;
    public RectTransform left;
    public RectTransform right;

    [Header("间距设置")]
    public float idleMargin = 15f;    // 静止时的初始间距
    public float walkMargin = 40f;    // 走路/移动时的扩散间距
    public float smoothSpeed = 10f;   // 扩散和收缩的平滑速度

    private float currentMargin;

    void Start()
    {
        // 初始状态设为静止间距
        currentMargin = idleMargin;
    }

    void Update()
    {
        // 1. 检测玩家是否正在移动 (基于 WASD 或 方向键输入)
        // 只要水平或垂直轴有输入，就判定为正在移动
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        // 2. 确定目标间距
        float targetMargin = isMoving ? walkMargin : idleMargin;

        // 3. 使用 Lerp（线性插值）让间距变化变得丝滑
        currentMargin = Mathf.Lerp(currentMargin, targetMargin, Time.deltaTime * smoothSpeed);

        // 4. 将计算出的间距应用到四个方向的坐标上
        ApplyMargin(currentMargin);
    }

    // 辅助函数：更新四个线条的坐标
    void ApplyMargin(float margin)
    {
        // Y 轴正方向
        if (top != null) top.anchoredPosition = new Vector2(0, margin);
        // Y 轴负方向
        if (bottom != null) bottom.anchoredPosition = new Vector2(0, -margin);
        // X 轴负方向
        if (left != null) left.anchoredPosition = new Vector2(-margin, 0);
        // X 轴正方向
        if (right != null) right.anchoredPosition = new Vector2(margin, 0);
    }
}