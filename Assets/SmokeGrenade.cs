using UnityEngine;

public class SmokeGrenade : Throwable
{
    [Header("—ÃŒÌµØ…Ë÷√")]
    public GameObject smokeEffectPrefab;
    public AudioClip deploySound;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasBeenThrown)
        {
            Explode();
        }
    }

    public override void Explode()
    {
        if (smokeEffectPrefab != null)
            Instantiate(smokeEffectPrefab, transform.position, Quaternion.identity);
        if (deploySound != null)
            AudioSource.PlayClipAtPoint(deploySound, transform.position);

        Destroy(gameObject);
    }
}