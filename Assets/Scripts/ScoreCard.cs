using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScoreCard : MonoBehaviour
{

    public static ScoreCard Instance;
    private int score = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("score: " + score);
    }

    public int getScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0;
    }
}

public class Collectible : MonoBehaviour
{
    public int points = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ScoreCard.Instance.AddScore(points);
            Destroy(gameObject);
        }
    }
}
