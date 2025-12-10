using System;
using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseRange = 6f;
    [SerializeField] private float attackRange = 1.2f;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilSpeed = 8f;
    [SerializeField] private float recoilDuration = 0.25f;
    private Boolean recoilSwitch = true;

    public bool isRecoiling = false;
    public float recoilTimer = 0f;
    public int recoilDirection = 1;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        PlayerMove2 p = FindObjectOfType<PlayerMove2>();
        if (p != null) player = p.transform;
    }

    private void FixedUpdate()
    {
        // Recoil override
        if (isRecoiling)
        {
            rb.velocity = new Vector2(recoilDirection * recoilSpeed, rb.velocity.y);
            recoilTimer -= Time.fixedDeltaTime;

            if (recoilTimer <= 0f)
                isRecoiling = false;

            return;
        }

        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else if (dist <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void ChasePlayer()
    {
        float dir = player.position.x > transform.position.x ? 1f : -1f;
        rb.velocity = new Vector2(dir * moveSpeed, rb.velocity.y);
        FaceDirection((int)dir);
    }

    public void FaceDirection(int direction)
    {
        Vector3 s = transform.localScale;

        if (direction > 0 && s.x < 0) s.x *= -1;
        else if (direction < 0 && s.x > 0) s.x *= -1;

        transform.localScale = s;
    }

    public void StartRecoil()
    {
    
        recoilDirection = recoilSwitch ? -1 : 1;
        FaceDirection(recoilDirection);

        isRecoiling = true;
        recoilTimer = recoilDuration;
        rb.velocity = new Vector2(recoilDirection * recoilSpeed, rb.velocity.y);
        recoilSwitch = !recoilSwitch;
    }
    

}
