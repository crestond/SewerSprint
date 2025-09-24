using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    private void OnCollisionEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hazard"))
        {
            PlayerControl pc = collision.GetComponent<PlayerControl>();
            if (pc != null)
            {
                pc.RemoveHeart();
            }
        }
    }
}
