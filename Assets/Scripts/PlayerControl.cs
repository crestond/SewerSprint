using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private List<GameObject> hearts;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI flowButtonText;
    [SerializeField] private GameObject FlowButton;
    [SerializeField] private GameObject YouDiedText;
    [SerializeField] private GameObject YouWonText;

    private Rigidbody2D rb;
    private bool wonLevel = false;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        FlowButton.SetActive(false);
        YouDiedText.SetActive(false);
        YouWonText.SetActive(false);
        UpdateScoreUI();
    }

    public void RemoveHeart()
    {
        if (IsDead) return;

        if (hearts.Count > 0)
        {
            GetComponent<PlayerMove2>()?.TriggerInvulnerability();
            
            GameObject lastHeart = hearts[hearts.Count - 1];
            lastHeart.SetActive(false);
            hearts.RemoveAt(hearts.Count - 1);

            Debug.Log($"Heart removed. Remaining: {hearts.Count}");

            if (hearts.Count == 0)
            {
                KillPlayer();
            }
        }
    }

    private void KillPlayer()
    {
        IsDead = true;
        GameData.score = 0;

        rb.velocity = Vector2.zero;
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;

        YouDiedText.SetActive(true);
        FlowButton.SetActive(true);
        flowButtonText.text = "Restart";

        Debug.Log("Player died!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            GameData.score += 10;
            UpdateScoreUI();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Finish"))
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
            
            wonLevel = true;
            YouWonText.SetActive(true);
            FlowButton.SetActive(true);
            flowButtonText.text = "Next Level";
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + GameData.score;
    }

    public void FlowGame()
    {
        if (IsDead)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else if (wonLevel)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public bool IsInvulnerable
    {
    get
    {
        PlayerMove2 move = GetComponent<PlayerMove2>();
        return move != null && move.IsInvulnerable;
    }
    }
}
