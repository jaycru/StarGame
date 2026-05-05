using UnityEngine;
using System.Collections;

public class Grenade : Throwable
{
    [Header("手雷设置")]
    public float fuseTime = 4f;
    public float explosionRadius = 5f;
    public float explosionForce = 800f;
    public int damage = 100;
    public GameObject explosionEffectPrefab;
    public AudioClip explosionSound;

    protected override void OnThrown()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    public override void Explode()
    {
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            if (hit.CompareTag("Enemy"))
            {
                // hit.GetComponent<Enemy>().TakeDamage(damage);
                Debug.Log($"手雷击中 {hit.name} 造成 {damage} 伤害!");
            }
        }
        Destroy(gameObject);
    }
}