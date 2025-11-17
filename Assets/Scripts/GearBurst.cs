using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private ParticleSystem idleParticles;
    [SerializeField] private ParticleSystem burstParticles;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            // Stop the idle effect
            if (idleParticles != null)
            {
                idleParticles.Stop();
            }

            // Detach burst so it can play even if we destroy the coin
            if (burstParticles != null)
            {
                burstParticles.transform.parent = null;
                burstParticles.Play();
                Destroy(burstParticles.gameObject, 1f); // clean up after it finishes
            }

            // Destroy or disable the coin itself
            Destroy(gameObject);
        }
    }
}
