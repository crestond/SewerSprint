using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private ParticleSystem idleParticles;
    [SerializeField] private ParticleSystem burstParticles;
    [SerializeField] private AudioClip collectSFX;

    // 👉 new field
    [SerializeField] private AudioSource sfxSource;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            if (idleParticles != null)
                idleParticles.Stop();

            // 🔊 Play through the shared SFX AudioSource (which is on the mixer SFX group)
            if (sfxSource != null && collectSFX != null)
            {
                sfxSource.PlayOneShot(collectSFX);
            }

            if (burstParticles != null)
            {
                burstParticles.transform.parent = null;
                burstParticles.Play();
                Destroy(burstParticles.gameObject, 1f);
            }

            Destroy(gameObject);
        }
    }
}
