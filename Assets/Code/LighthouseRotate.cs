using UnityEngine;

public class LighthouseRotate : MonoBehaviour
{
    public float rotateSpeed = 30f; // 旋转速度

    void Update()
    {
        // 让光柱绕着 Y 轴（垂直轴）旋转
        // 注意：如果你的模型轴向不同，可能需要改成 Vector3.up 或 Vector3.forward
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}
