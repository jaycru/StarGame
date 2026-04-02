using UnityEngine;

public class CameraIdleMotion : MonoBehaviour
{
    public float range = 0.5f; // 晃动范围
    public float speed = 0.5f; // 晃动速度
    private Vector3 startPos;

    void Start() { startPos = transform.position; }

    void Update()
    {
        // 使用正弦波控制位置，产生漂浮感
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * range;
        float newX = startPos.x + Mathf.Cos(Time.time * speed * 0.8f) * range;
        transform.position = new Vector3(newX, newY, startPos.z);
    }
}