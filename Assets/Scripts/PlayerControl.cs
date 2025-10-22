using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    public int score = 0;
    private bool canJump;
    public float moveSpeed = 5f;       // Horizontal movement speed
    public float jumpForce = 3f;       // Jump power
    private bool isGrounded = true;           // Check if player is on ground
    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private float jumpCutMultiplier = 2f;
    [SerializeField] private List<GameObject> hearts;
    [SerializeField] private GameObject RestartButton;
    [SerializeField] private GameObject YouDiedText;
    [SerializeField] private GameObject YouWonText;

    [Header("Ground Check")]
    public Transform groundCheck;      // Empty object under player’s feet
    public float groundCheckRadius = 1.2f;
    public LayerMask groundLayer;      // What counts as ground

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        RestartButton.SetActive(false);
        YouDiedText.SetActive(false);
        YouWonText.SetActive(false);
    }

    void Update()
    {
        // Horizontal movement
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Jump (only if grounded)
        
        if (Input.GetButtonDown("Jump") && canJump)
        {
            
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        // Variable jump↓
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * (1f / jumpCutMultiplier));
            // cut velocity in half when releasing early
        }

        // Animator updates
        animator.SetFloat("moveX", Mathf.Abs(moveInput));
        animator.SetFloat("moveY", rb.velocity.y);
        animator.SetBool("isMoving", moveInput != 0);

        // Flip sprite
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void FixedUpdate()
    {
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        //canJump = isGrounded;
        //Debug.Log(isGrounded);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // I used a switch to eliminate if statements
        switch (collision.gameObject.tag)
        {
            case "Ground":
                canJump = true;
                break;
            case "Hazard":
                RemoveHeart();
                break;
            case "Finish":
                YouWonText.SetActive(true);
                RestartButton.SetActive(true);
                this.enabled = false;
                break;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Coin"))
            {
                score += 10;
                UpdateScoreUI();
                Destroy(other.gameObject);
            }
        }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            canJump = false;
        }
    }
    public void RemoveHeart()
    {
        if (hearts.Count > 0)
        {
            GameObject lastHeart = hearts[hearts.Count - 1];
            lastHeart.SetActive(false);   // hides the heart
            hearts.RemoveAt(hearts.Count - 1);

            Debug.Log("Hearts left: " + hearts.Count);

            if (hearts.Count == 0)
            {
                Debug.Log("Game Over!");

                this.enabled = false; // disables this script (no movement/damage)

                YouDiedText.SetActive(true);
                RestartButton.SetActive(true);
            }
        }
    }
    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

