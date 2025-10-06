using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{

    public int levelBonus = 100;
    public GameObject levelCompleteUI;
    public bool loadNextLevel = false;

    public string nextLevelName;

    private bool levelEnded = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ScoreCard.Instance.AddScore(levelBonus);

            Debug.Log("Level Complete! " + levelBonus);
        }
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (levelEnded) return;

        if (collision.CompareTag("Player"))
        {
            levelEnded = true;

            ScoreCard.Instance.AddScore(levelBonus);
            Debug.Log("Level Complete! +" + levelBonus);

            if (levelCompleteUI != null)
            {
                levelCompleteUI.SetActive(true);
            }

            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;

            if (loadNextLevel && !string.IsNullOrEmpty(nextLevelName))
            {
                Invoke(nameof(loadNextLevel), 2f);
            }
        }
    }*/

    /*private void LoadNextLevel()
    {
        SceneManager.LoadScene(level2);
    }*/

}
