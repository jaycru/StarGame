using UnityEngine;

public class ThrowableItem : MonoBehaviour
{
    public float damage = 50f;      // 伤害值
    public float explosionRadius = 5f; // 爆炸范围半径
    public LayerMask enemyLayer;    // 敌人的层

    private bool hasExploded = false;

    // 碰撞检测
    private void OnCollisionEnter(Collision collision)
    {
        // 如果还没爆炸，就执行爆炸逻辑
        if (!hasExploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;

        // 1. 视觉反馈：让球瞬间变大，代表爆炸范围 (持续0.2秒)
        GetComponent<MeshRenderer>().enabled = true; // 确保可见
        transform.localScale = Vector3.one * explosionRadius * 2; // 变大
        GetComponent<MeshRenderer>().material.color = Color.white; // 闪白

        // 2. 伤害逻辑：检测范围内的敌人
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);
        foreach (var hit in hitColliders)
        {
            // 假设敌人有个脚本叫 EnemyHealth
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)damage);
            }
        }

        // 3. 销毁：0.2秒后销毁这个球
        Destroy(gameObject, 0.2f);
    }
}