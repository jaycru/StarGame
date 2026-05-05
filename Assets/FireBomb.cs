using UnityEngine;
using System.Collections;

public class FireBomb : Throwable
{
    [Header("燃烧弹设置")]
    public GameObject fireEffectPrefab;
    public float burnDuration = 8f;
    public float damagePerSecond = 12f;
    public float burnRadius = 3f;
    public AudioClip igniteSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasBeenThrown)
        {
            Explode();
        }
    }

    public override void Explode()
    {
        if (igniteSound != null)
            AudioSource.PlayClipAtPoint(igniteSound, transform.position);

        GameObject fireZone = Instantiate(fireEffectPrefab, transform.position, Quaternion.identity);
        FireZone fireZoneScript = fireZone.GetComponent<FireZone>();
        if (fireZoneScript != null)
        {
            fireZoneScript.Setup(burnDuration, damagePerSecond, burnRadius);
        }
        Destroy(gameObject);
    }
}

// 辅助脚本：管理地面上的火焰区域
public class FireZone : MonoBehaviour
{
    private float duration;
    private float dps;
    private float radius;
    private float timer = 0f;

    public void Setup(float burnDuration, float damagePerSecond, float burnRadius)
    {
        duration = burnDuration;
        dps = damagePerSecond;
        radius = burnRadius;
    }

    private void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider hit in colliders)
            {
                if (hit.CompareTag("Enemy"))
                {
                    // hit.GetComponent<Enemy>().TakeDamage(dps * Time.deltaTime);
                    Debug.Log($"{hit.name} 正在燃烧!");
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}