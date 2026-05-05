using UnityEngine;

// 投掷物类型枚举，与 LootItem 对应
public enum ThrowableType
{
    Grenade,
    Smoke,
    Fire
}

public abstract class Throwable : MonoBehaviour
{
    [Header("投掷设置")]
    public float throwForce = 10f;
    public ThrowableType type;

    protected bool hasBeenThrown = false;

    // 由玩家控制器调用，施加物理力
    public void Throw(Vector3 direction)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(direction * throwForce, ForceMode.Impulse);
            hasBeenThrown = true;
            OnThrown();
        }
    }

    protected virtual void OnThrown() { }

    // 抽象方法，由子类实现爆炸逻辑
    public abstract void Explode();
}