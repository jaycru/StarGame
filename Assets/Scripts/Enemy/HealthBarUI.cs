using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("设置")]
    public Transform target; // 血条跟随的目标（敌人）
    public Vector3 offset = new Vector3(0, 1.5f, 0); // 血条相对于目标的偏移

    private Camera mainCamera;
    private Slider healthSlider;

    private void Start()
    {
        Canvas canvas = GetComponent<Canvas>();

        canvas.renderMode = RenderMode.WorldSpace;

        canvas.worldCamera = Camera.main;
        // 获取主摄像机
        mainCamera = Camera.main;

        // 获取自身的Slider组件
        healthSlider = GetComponent<Slider>();

        // 初始化血条为满血状态
        if (healthSlider != null)
        {
            healthSlider.value = 1f;
        }
    }

    private void LateUpdate()
    {
        // 确保目标和主摄像机都存在才执行更新
        if (target != null && mainCamera != null)
        {
            // 1. 血条跟随目标
            transform.position = target.TransformPoint(offset);
            transform.LookAt(Camera.main.transform);

            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            // 2. 血条始终面向主摄像机
            // 这是实现广告牌效果的核心
            //transform.forward = -mainCamera.transform.forward;
        }
    }

    /// <summary>
    /// 由外部（如EnemyHealth脚本）调用，用于更新血量显示
    /// </summary>
    /// <param name="current">当前血量</param>
    /// <param name="max">最大血量</param>
    public void SetHealth(float current, float max)
    {
        if (healthSlider != null && max > 0)
        {
            // 计算血量百分比并更新Slider的值
            healthSlider.value = current / max;
        }
    }
}