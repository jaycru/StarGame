using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    public LineRenderer lineRenderer; // 用来画线的组件
    public int segments = 20; // 线的平滑度
    public float checkInterval = 0.1f; // 计算间隔

    void Start()
    {
        // 如果没有手动赋值，尝试获取组件
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        // 初始化线的样式
        lineRenderer.positionCount = segments;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.red;

        // 默认隐藏
        lineRenderer.enabled = false;
    }

    // 计算并显示轨迹
    public void ShowTrajectory(Vector3 startPos, Vector3 velocity)
    {
        lineRenderer.enabled = true;
        Vector3 pos = startPos;
        Vector3 vel = velocity;

        for (int i = 0; i < segments; i++)
        {
            lineRenderer.SetPosition(i, pos);
            // 简单的物理模拟：位置 += 速度 * 时间
            pos += vel * checkInterval;
            // 速度 += 重力 * 时间
            vel += Physics.gravity * checkInterval;

            // 简单的地面检测，如果碰到地面就停止画线
            RaycastHit hit;
            if (Physics.SphereCast(pos, 0.2f, vel.normalized, out hit, vel.magnitude * checkInterval))
            {
                lineRenderer.SetPosition(i, hit.point);
                break;
            }
        }
    }

    // 隐藏轨迹
    public void HideTrajectory()
    {
        lineRenderer.enabled = false;
    }
}