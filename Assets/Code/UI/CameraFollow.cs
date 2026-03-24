using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // 想要跟随的目标（把Player拖进来）
    public Vector3 offset;          // 摄像机与目标的距离偏移
    public float smoothSpeed = 5f;  // 跟随的平滑度（数值越大跟得越紧）

    void Start()
    {
        // 自动计算初始偏移量：当前摄像机位置 - 玩家位置
        // 这样你在编辑器里摆好位置后，脚本会自动记住这个距离
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    // 为什么要用 LateUpdate？
    // 摄像机跟随必须在所有物体移动（Update）完成后再执行
    // 这样可以彻底避免镜头抖动现象
    void LateUpdate()
    {
        if (target == null) return;

        // 计算目标位置
        Vector3 desiredPosition = target.position + offset;
        
        // 使用 Lerp（线性插值）实现平滑移动
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // 应用位置
        transform.position = smoothedPosition;
    }
}
