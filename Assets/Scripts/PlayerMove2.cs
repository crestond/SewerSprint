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
    private Rigidbody2D body;
    private Animator anim;
    //private bool grounded;

    [Header("Invulnerability")]
    [SerializeField] private float invulnerabilityDuration = 1f;
    private float invulnerabilityTimer = 0f;
    public bool IsInvulnerable => invulnerabilityTimer > 0f;

    private PlayerControl playerControl;

    
    private void Awake()
    {
        //grabs references
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerControl = GetComponent<PlayerControl>();
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
        if (collision.gameObject.tag == "Hazard" && !IsInvulnerable && !playerControl.IsDead)
        {
            canJump = true;
            playerControl.RemoveHeart();
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
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Hazard")
        {
            canJump = false;
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
}
