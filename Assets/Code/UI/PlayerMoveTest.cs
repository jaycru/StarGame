using UnityEngine;

public class PlayerMoveTest : MonoBehaviour
{
    public float moveSpeed = 10f; // 移动速度

    void Update()
    {
        // 1. 获取键盘输入（A/D对应Horizontal，W/S对应Vertical）
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 2. 计算位移向量
        Vector3 moveVector = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;

        // 3. 应用位移
        transform.Translate(moveVector, Space.World);
    }
}
