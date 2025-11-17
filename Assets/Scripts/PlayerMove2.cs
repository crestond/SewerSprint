using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove2 : MonoBehaviour
{
    /*
    public int playerSpeed = 10;
    public bool facingRight = true;
    public int playerJumpPower = 1250;
    private float movex;
    public bool isGrounded;


    // Update is called once per frame
    void Update()
    {

    }
    
    void playerMove()
    {
        movex = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("jump"))
        {
            
        }
    }*/
    [Header("Movement Settings")]
    [SerializeField] private float speed = 10;
    [SerializeField] private float JumpForce = 7f;
    [SerializeField] private float jumpCutMultiplier = 2f;
    private bool canJump;
    private bool isKnockedback;
    private float knockbackTimer = 0f;
    private Rigidbody2D body;
    private Animator anim;
    private SpriteRenderer sr;

    [Header("Invulnerability")]
    [SerializeField] private float invulnerabilityDuration = 1f;
    private float invulnerabilityTimer = 0f;
    public bool IsInvulnerable => invulnerabilityTimer > 0f;

    [Header("Enemies")]
    private RatAI_TagCheck ratUnderPlayer;

    private PlayerControl playerControl;

    
    private void Awake()
    {
        //grabs references
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerControl = GetComponent<PlayerControl>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Update invulnerability timer
        if (invulnerabilityTimer > 0f)
        {
            invulnerabilityTimer -= Time.deltaTime;
            // Clamp timer to 0 to prevent negative values
            if (invulnerabilityTimer < 0f)
            {
                invulnerabilityTimer = 0f;
            }
        }
        
        if (isKnockedback)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedback = false;
            }
            return; // Skip normal movement while in knockback
        }

        float HorizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(HorizontalInput * speed, body.velocity.y);

        //flipping the player left->right
        if (HorizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (HorizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        
        //sets animation*/
        if (Input.GetButtonDown("Jump") && canJump)
        {
            if (ratUnderPlayer != null)
            {
                Destroy(ratUnderPlayer.gameObject);
                ratUnderPlayer = null;
            }

            body.velocity = new Vector2(body.velocity.x, JumpForce);
            GetComponent<PlayerSound>()?.PlayJumpSound();
        }
        // Variable jump↓
        if (Input.GetButtonUp("Jump") && body.velocity.y > 0)
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * (1f / jumpCutMultiplier));
            // cut velocity in half when releasing early
        }

        anim.SetBool("walk", HorizontalInput != 0);
    }

    /*private void jump()
    {
        body.velocity = new Vector2(body.velocity.x, JumpForce);
        grounded = false;
    }*/

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            canJump = true;
        }

        if (collision.gameObject.tag == "Hazard"  && !IsInvulnerable && !playerControl.IsDead)
        {
            canJump = true;
            playerControl.RemoveHeart();
        }

        if (collision.gameObject.tag == "Rat")
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Check if the contact point is below the player's center
                if (contact.normal.y > 0.5f)
                {
                    StartCoroutine(RatDeathRoutine(collision.gameObject));
                    GameData.score += 5;
                    canJump = true; // Allow jumping again after killing a rat/enemy    
                    return;
                }
            }
            TryTakeDamage();
            return;
        }
    }

        void OnCollisionStay2D(Collision2D collision)
    {
        // Handle continuous contact with hazards
        // Only process if not invulnerable (to avoid unnecessary checks every frame)
        // RemoveHeart() will handle the actual damage and invulnerability reset
        if (collision.gameObject.tag == "Hazard")
        {
            TryTakeDamage();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            canJump = false;
        }

        if (collision.gameObject.tag == "Rat")
        {
            ratUnderPlayer = null;
        }
    }

    public void TriggerInvulnerability()
    {
    invulnerabilityTimer = invulnerabilityDuration;
    }

    private void TryTakeDamage()
    {
        if (IsInvulnerable || playerControl == null || playerControl.IsDead) return;

        playerControl.RemoveHeart();
        invulnerabilityTimer = invulnerabilityDuration;

    }

    public void KnockBack(Vector2 direction, float force, float upwardForce = 2f)
    {
        isKnockedback = true;
        knockbackTimer = 0.5f; // Duration of knockback effect

        //kill movement and apply knockback
        body.velocity = Vector2.zero;

        Vector2 knock = new Vector2(direction.x * force, upwardForce);

        body.velocity = knock;
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
    }

    private IEnumerator RatDeathRoutine(GameObject rat)
    {
    RatAI_TagCheck ai = rat.GetComponent<RatAI_TagCheck>();

    if (ai != null)
        yield return StartCoroutine(ai.DamageFlash());

    Destroy(rat);
    }

}
