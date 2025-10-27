using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class SpikeHazard : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Hazard")
        {
            PlayerControl pc = collision.gameObject.GetComponent<PlayerControl>();
            if (pc != null)
            {
                pc.RemoveHeart();
            }
        }
    }
}
