using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;  // needed for List<T>

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D body;

    // 👇 Drag 3 heart GameObjects into this list in the Inspector
    [SerializeField] private List<GameObject> hearts;
    [SerializeField] private GameObject DiedMessage;
    [SerializeField] private GameObject RestartButton;
    [SerializeField] private float damageCooldown = 2f;
    private float lastDamageTime = -999f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        DiedMessage.SetActive(false);
        RestartButton.SetActive(false);
    }

    private void Update()
    {
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);

        if (Input.GetKey(KeyCode.Space))
        {
            body.velocity = new Vector2(body.velocity.x, speed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
    {
        // Check if enough time has passed since last damage
        if (Time.time - lastDamageTime >= damageCooldown)
        {
            Debug.Log("Player hit a spike!");
            RemoveHeart();
            lastDamageTime = Time.time; // reset cooldown
        }
        else
        {
            Debug.Log("Player is invulnerable right now.");
        }
    }
}

    private void RemoveHeart()
    {
        if (hearts.Count > 0)
        {
            // Get the LAST heart in the list
            GameObject lastHeart = hearts[hearts.Count - 1];

            // Disable it instead of destroying it
            lastHeart.SetActive(false);

            // Remove it from the list so the next hit targets the next heart
            hearts.RemoveAt(hearts.Count - 1);

            Debug.Log("Hearts left: " + hearts.Count);

            if (hearts.Count == 0)
            {
                Debug.Log("Game Over!");
                body.velocity = Vector2.zero;
                this.enabled = false; // disables this script (no movement/damage)

                DiedMessage.SetActive(true);
                RestartButton.SetActive(true);
            }

        }
    }
public void RestartGame()
{
    // Reloads the currently active scene
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}

}
