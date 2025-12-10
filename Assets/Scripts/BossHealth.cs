using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RatBossHealth : MonoBehaviour
{
[Header("Health Settings")]
    [SerializeField] private int maxHearts = 5;
    [SerializeField] private float invulnerabilityDuration = 1f;

    [Header("Boss Recoil Settings")]
    [SerializeField] private float recoilDuration = 0.25f;

    [Header("Player Knockback Settings")]
    [SerializeField] private float playerKnockbackX = 14f;
    [SerializeField] private float playerKnockbackY = 8f;

    [Header("Boss Hearts UI")]
    [SerializeField] private Transform heartsParent; 
    [SerializeField] private List<GameObject> bossHearts = new List<GameObject>();

    private int currentHearts;
    private bool isInvulnerable = false;
    private bool isDead = false;

    private PlayerMove2 player;
    private SpriteRenderer sr;
    private BossAI ai;

    public bool IsInvulnerable => isInvulnerable;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ai = GetComponent<BossAI>();
        player = FindObjectOfType<PlayerMove2>();

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

        // Remove a heart UI
        if (bossHearts.Count > 0)
        {
            GameObject last = bossHearts[bossHearts.Count - 1];
            last.SetActive(false);
            bossHearts.RemoveAt(bossHearts.Count - 1);
        }

        TriggerInvulnerability();
        HandleRecoil(attacker);
        StartCoroutine(DamageFlash());

        if (currentHearts <= 0)
            KillBoss();
    }

    private void HandleRecoil(Transform attacker)
    {

        if (ai != null)
            ai.StartRecoil();
    }

    public void KnockbackPlayer()
    {
        if (player == null) return;

        PlayerControl pc = player.GetComponent<PlayerControl>();
        if (pc == null || pc.IsDead || player.IsInvulnerable) return;

        bool playerIsRight = player.transform.position.x > transform.position.x;
        Vector2 dir = playerIsRight ? Vector2.right : Vector2.left;

        pc.RemoveHeart();
        player.KnockBack(dir, playerKnockbackX, playerKnockbackY); // tweakable values, serialized
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
}
