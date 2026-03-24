using UnityEngine;

public class HUDTester : MonoBehaviour
{
    private float currentH = 100f;
    private int currentA = 30;
    private bool isHintActive = false;

    void Update()
    {
        // 按 H 模拟受伤
        if (Input.GetKeyDown(KeyCode.H))
        {
            currentH -= 15f;
            if (currentH < 0) currentH = 0;

            // 1. 更新血条数值
            HUDManager.Instance.UpdateHealth(currentH, 100f);
            // 2. 触发受击全屏闪红效果
            HUDManager.Instance.PlayDamageEffect();
        }

        // 按 J 模拟射击
        if (Input.GetKeyDown(KeyCode.J))
        {
            currentA -= 1;
            if (currentA < 0) currentA = 30; // 模拟换弹
            HUDManager.Instance.UpdateAmmo(currentA, 30);
        }

        // 按 E 模拟开关提示框
        if (Input.GetKeyDown(KeyCode.E))
        {
            isHintActive = !isHintActive;
            HUDManager.Instance.SetInteractionHint(isHintActive, "按 [F] 交互物品");
        }
    }
}