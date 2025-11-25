using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RatBossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHearts = 5;
    [SerializeField] private float invulnerabilityDuration = 1f;
    [SerializeField] private float knockbackForceX = 4f;
    [SerializeField] private float knockbackForceY = 4f;
    [SerializeField] private Transform heartsParent; 
    [SerializeField] private List<GameObject> bossHearts;


    private int currentHearts;
    private bool isInvulnerable = false;
    public bool IsInvulnerable => isInvulnerable;
    private bool isDead = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private RatAI_TagCheck ai;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ai = GetComponent<RatAI_TagCheck>();

            // AUTO-POPULATE HEARTS
        if (heartsParent != null)
        {
            bossHearts.Clear();
            foreach (Transform child in heartsParent)
            {
                bossHearts.Add(child.gameObject);
            }
        }

        currentHearts = maxHearts;
    }

    public void TakeDamage(Transform attacker)
    {
        if (isDead || isInvulnerable) return;

        currentHearts--;
        GameObject lastHeart = bossHearts[bossHearts.Count - 1];
        lastHeart.SetActive(false);
        bossHearts.RemoveAt(bossHearts.Count - 1);

        rb.bodyType = RigidbodyType2D.Kinematic;
        StartCoroutine(RestoreDynamic());

        TriggerInvulnerability();
        Knockback(attacker);
        StartCoroutine(DamageFlash());

        if (currentHearts <= 0)
            KillBoss();
    }

    private void Knockback(Transform attacker)
    {
        rb.velocity = Vector2.zero;

        bool attackerIsRight = attacker.position.x > transform.position.x;
        Vector2 dir = attackerIsRight ? Vector2.left : Vector2.right;

        rb.AddForce(
            new Vector2(dir.x * knockbackForceX, knockbackForceY),
            ForceMode2D.Impulse
        );
    }

    private void TriggerInvulnerability()
    {
        isInvulnerable = true;
        StartCoroutine(InvulnerabilityTimer());
    }

    private IEnumerator InvulnerabilityTimer()
    {
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    private IEnumerator DamageFlash()
    {
        float t = 0.1f;
        for (int i = 0; i < 4; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(t);

            sr.color = Color.white;
            yield return new WaitForSeconds(t);
        }
    }

    private void KillBoss()
    {
        if (isDead) return;

        isDead = true;
        rb.velocity = Vector2.zero;
        rb.simulated = false;

        if (ai != null)
            ai.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        StartCoroutine(DeathCleanup());
    }

    private IEnumerator DeathCleanup()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    private IEnumerator RestoreDynamic()
    {
        yield return new WaitForSeconds(0.2f);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
