using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackRange = 1.2f;

    [Header("Player Detection")]
    [SerializeField] private bool autoFindPlayer = true;
    private Transform player;

    private int currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isAttacking = false;
    private bool isInvulnerable = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (autoFindPlayer)
            ResolvePlayerReference();
    }

    private void ResolvePlayerReference()
    {
        var p = FindObjectOfType<PlayerMove2>();
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (!isAttacking)
                StartCoroutine(AttackRoutine());
        }
        else if (dist <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // idle
        }

        FacePlayer();
    }

    private void ChasePlayer()
    {
        float dir = player.position.x > transform.position.x ? 1f : -1f;
        rb.velocity = new Vector2(dir * moveSpeed, rb.velocity.y);
    }

    private void FacePlayer()
    {
        if (!player) return;
        float dir = player.position.x > transform.position.x ? 1f : -1f;

        Vector3 s = transform.localScale;
        if (dir > 0 && s.x < 0) s.x *= -1;
        if (dir < 0 && s.x > 0) s.x *= -1;
        transform.localScale = s;
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // TODO: Trigger your boss attack animation here
        yield return new WaitForSeconds(0.4f);

        // TODO: Damage the player here

        yield return new WaitForSeconds(0.6f);
        isAttacking = false;
    }

    // --------- Damage & Death ----------
    public void TakeDamage(int amount)
    {
        if (isInvulnerable) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        isInvulnerable = true;

        for (int i = 0; i < 3; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        isInvulnerable = false;
    }

    private void Die()
    {
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;

        // TODO: play death animation
        Destroy(gameObject, 0.3f);
    }
}
