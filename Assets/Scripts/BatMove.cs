using System.Collections;
using UnityEngine;

public class BatAI_TagCheck : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 1.5f;              // Patrol speed
    [SerializeField] private float chaseSpeed = 2.5f;           // Chase speed
    [SerializeField] private bool startMovingRight = true;

    [Header("Patrol Settings")]
    [SerializeField] private Transform patrolPointA;           // Left patrol point
    [SerializeField] private Transform patrolPointB;           // Right patrol point
    private Vector2 worldPatrolA;
    private Vector2 worldPatrolB;

    private Transform currentTarget;

    [Header("Player Detection")]
    [SerializeField] private float chaseRange = 2f;             // Start chasing
    [SerializeField] private float stopChaseRange = 3f;         // Stop chasing

    [Header("Wall Check")]
    [SerializeField] private Transform wallCheck;               // Slightly ahead of the nose 
    [SerializeField] private float wallCheckDistance = 0.2f;    // Forward ray distance

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Transform player;     
    
    public bool ChaseEnabled = false;                              // Player transform (resolved automatically)

    private bool movingRight;
    private bool isChasing = false;
    public bool isDying = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0;
        ResolvePlayerReference();

        worldPatrolA = patrolPointA.position;
        worldPatrolB = patrolPointB.position;

        movingRight = startMovingRight;
        currentTarget = startMovingRight ? patrolPointB : patrolPointA;
    }


    private void Update()
    {
        if (isDying) return;

        float distanceToPlayer = (player != null)
        ? Vector2.Distance(transform.position, player.position)
        : Mathf.Infinity;   

        if (!isChasing && distanceToPlayer <chaseRange)
        {
            isChasing = true;
        }
        else if (isChasing && distanceToPlayer > stopChaseRange)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        FaceMoveDirection();
    }

    private void Patrol()
    {
        if (isDying) return;

        Vector2 targetPos = (currentTarget == patrolPointA) ? worldPatrolA : worldPatrolB;

        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        rb.velocity = direction * patrolSpeed;

        float dist = Vector2.Distance(transform.position, targetPos);
        if (dist < 0.1f)
        {
            if (currentTarget == patrolPointA)
            {
                currentTarget = patrolPointB;
            }
            else
            {
                currentTarget = patrolPointA;
            }
            // Determine direction using WORLD positions
            Vector2 nextTargetPos = (currentTarget == patrolPointA) ? worldPatrolA : worldPatrolB;
            movingRight = nextTargetPos.x > transform.position.x;
        }
        FaceMoveDirection();
    }


    private void ChasePlayer()
    {
        if (ChaseEnabled == false) return;
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = dir * chaseSpeed;
    }

    private bool IsWallAhead()
    {
        if (!wallCheck) return false;

        Vector2 fwd = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, fwd, wallCheckDistance);
        return hit.collider != null && !hit.collider.CompareTag("Player");

    }

    private void FaceMoveDirection()
    {
        Vector3 s = transform.localScale;
        if (movingRight && s.x <0f) s.x *= -1f;
        else if (!movingRight && s.x >0f) s.x *= -1f;
        transform.localScale = s;
    }

    private void ResolvePlayerReference()
    {
        var script = FindObjectOfType<PlayerMove2>();
        if (script != null) player = script.transform;
    }

    public void EnterDeathState()
    {
        if (isDying) return;
        isDying = true;

        rb.velocity = Vector2.zero;
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;
    }
    
    public IEnumerator DamageFlashAndFade()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float flashTime = 0.1f;
        int flashCount = 4;

        for (int i = 0; i < flashCount; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(flashTime);

            sr.color = Color.white;
            yield return new WaitForSeconds(flashTime);
        }

        float t = 0f;
        float duration = 1f;
        Color c = sr.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            sr.color = new Color(c.r, c.g, c.b, 1f - t / duration);
            yield return null;
        }
        Destroy(gameObject);
    }

}
