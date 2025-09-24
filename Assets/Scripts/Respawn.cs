using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Transform startPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ScoreCard.Instance.ResetScore();

            collision.transform.position = startPoint.position;
            Debug.Log("Player reset to start point");
        }
    }
}
