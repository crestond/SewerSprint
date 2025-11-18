
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAI_TagCheck : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;              // Patrol speed
    [SerializeField] private float chaseSpeed = 3.5f;           // Chase speed
    [SerializeField] private bool startMovingRight = true;

    [Header("Edge / Wall Checks (child empties)")]
    [SerializeField] private Transform groundCheck;             // Slightly ahead of the feet
    [SerializeField] private float groundCheckDistance = 0.5f;  // Down ray distance
    [SerializeField] private Transform wallCheck;               // Slightly ahead of the nose (optional)
    [SerializeField] private float wallCheckDistance = 0.2f;    // Forward ray distance

    [Header("Player Detection")]
    [SerializeField] private bool autoFindPlayer = true;        // Auto-locate PlayerMove2 on scene load
    private Transform player;                                   // Player transform (resolved automatically)
    [SerializeField] private float chaseRange = 2f;             // Start chasing
    [SerializeField] private float stopChaseRange = 3f;         // Stop chasing

    [Header("Tags (matching PlayerMove2)")]
    [SerializeField] private string groundTag = "Ground";
    [SerializeField] private string hazardTag = "Hazard";       // spikes, lava, etc.

    [Header("Stability")]
    [SerializeField] private float flipCooldownTime = 0.15f;    // prevent rapid flip at edges

    private Rigidbody2D rb;
    private bool movingRight;
    private SpriteRenderer sr;
    public bool isDying = false;
    private bool isFlashing = false;
    private bool isChasing = false;
    private bool grounded = false;                              // mirrors your PlayerMove2 idea
    private float flipCooldownTimer = 0f;

    // --- Resolve player reference by script ---
    private void ResolvePlayerReference()
    {
        if (player != null) return;

        var playerScript = FindObjectOfType<PlayerMove2>();
        if (playerScript != null)
        {
            player = playerScript.transform;
        }
        else
        {
            StartCoroutine(TryFindPlayerSoon());
        }
    }

    private IEnumerator TryFindPlayerSoon()
    {
        yield return null; // wait one frame
        var playerScript = FindObjectOfType<PlayerMove2>();
        if (playerScript != null)
            player = playerScript.transform;
    }
    // --- END ---

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        movingRight = startMovingRight;
        if (rb) rb.freezeRotation = true;

        if (autoFindPlayer)
            ResolvePlayerReference();
    }

    private void OnEnable()
    {
        if (autoFindPlayer && player == null)
            ResolvePlayerReference();
    }

    private void Update()
    {
        // If we still don't have a player, just patrol
        if (player == null)
        {
            Patrol();
            if (flipCooldownTimer > 0f) flipCooldownTimer -= Time.deltaTime;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // State switch (chase vs patrol)
        if (!isChasing && distanceToPlayer < chaseRange) isChasing = true;
        else if (isChasing && distanceToPlayer > stopChaseRange) isChasing = false;

        // Move according to state
        if (isChasing) ChasePlayer();
        else Patrol();

        // Cooldown timer
        if (flipCooldownTimer > 0f) flipCooldownTimer -= Time.deltaTime;
    }

    private void Patrol()
    {
        // Horizontal move
        rb.velocity = new Vector2((movingRight ? 1f : -1f) * moveSpeed, rb.velocity.y);

        // Only decide to flip if actually grounded
        if (!grounded || flipCooldownTimer > 0f) return;

        // Edge check (downward ray ahead of feet) — ONLY Ground is safe now
        bool hasGroundAhead = HasGroundAheadTag();

        // Wall/hazard check (forward ray)
        bool wallOrHazardAhead = IsWallOrHazardAhead();

        // Flip if edge or wall/hazard
        if (!hasGroundAhead || wallOrHazardAhead)
        {
            Flip();
        }

        FaceMoveDirection();
    }

    private void ChasePlayer()
    {
        // Move toward player on X
        bool playerIsRight = player.position.x > transform.position.x;
        movingRight = playerIsRight;

        rb.velocity = new Vector2((movingRight ? 1f : -1f) * chaseSpeed, rb.velocity.y);

        // Don't chase off edges or into hazards
        if (grounded && flipCooldownTimer <= 0f && (!HasGroundAheadTag() || IsWallOrHazardAhead()))
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        FaceMoveDirection();
    }

    // ===== Ground & forward detection using tags =====

    // NOTE: hazards are NOT considered safe floor anymore
    private bool HasGroundAheadTag()
    {
        if (!groundCheck) return true; // fail-safe

        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance);
        if (!hit.collider) return false;

        // Only pure ground counts as safe floor
        return hit.collider.CompareTag(groundTag);
    }

    private bool IsWallOrHazardAhead()
    {
        if (!wallCheck) return false; // optional

        Vector2 fwd = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, fwd, wallCheckDistance);
        if (!hit.collider) return false;

        // If it's a hazard, we want to flip away from it
        if (hit.collider.CompareTag(hazardTag)) return true;

        // Treat anything NOT floor-like as a wall/stopper
        bool isFloorLike = hit.collider.CompareTag(groundTag);
        return !isFloorLike;
    }

    private void Flip()
    {
        movingRight = !movingRight;
        flipCooldownTimer = flipCooldownTime;

        // tiny nudge so the next frame’s raycast isn’t still on the corner
        rb.position += new Vector2((movingRight ? 1f : -1f) * 0.02f, 0f);
    }

    private void FaceMoveDirection()
    {
        Vector3 s = transform.localScale;
        if (movingRight && s.x < 0f) s.x *= -1f;
        else if (!movingRight && s.x > 0f) s.x *= -1f;
        transform.localScale = s;
    }

    // ===== Grounded state like PlayerMove2 + hazard turn-around =====
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
            grounded = true;

        // If we collide with a hazard, flip away
        if (collision.gameObject.CompareTag(hazardTag) && flipCooldownTimer <= 0f)
        {
            Flip();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
            grounded = false;
    }

    // If your hazards use triggers instead of solid colliders
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(hazardTag) && flipCooldownTimer <= 0f)
        {
            Flip();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Ranges
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopChaseRange);

        // Ground ray
        if (groundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }

        // Wall ray
        if (wallCheck)
        {
            Gizmos.color = Color.magenta;
            Vector3 fwd = (movingRight ? Vector3.right : Vector3.left);
            if (!Application.isPlaying) fwd = (startMovingRight ? Vector3.right : Vector3.left);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + fwd * wallCheckDistance);
        }
    }

    public IEnumerator DamageFlash()
    {
        float flashTime = 0.1f;
        int flashCount = 4;

        for (int i = 0; i < flashCount; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(flashTime);

            sr.color = Color.white;
            yield return new WaitForSeconds(flashTime);
        }

        Destroy(gameObject);
    }
    public void EnterRatDeathFlash()
    {
        if (isDying) return;
        isDying = true;

        rb.velocity = Vector2.zero; // Stop Movement when he dies
        rb.simulated = false; // Disable physics interactions
        this.enabled = false; // Disable this script to stop further movement

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false; // Disable collider to prevent further interactions


    }

}

