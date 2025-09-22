

/*
public class PlayerControl : MonoBehaviour
{

    public float moveSpeed;

    private bool isMoveing;

    private Animator animator;

    private Vector2 input;

    public LayerMask solidObjectsLayer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoveing)
        {
            input.x = Input.GetAxisRaw("Horizontal"); //input left/right keyboard or joystick
            input.y = Input.GetAxisRaw("Vertical");  // input up/down keyboard or joystick

            if (input != Vector2.zero) //if the player pushed a direction key
            {
                //setting the direction of the player character animation
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                //no diagnal movement
                if (input.x != 0) input.y = 0;

                // calculates new target position
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                //seeing if the target position can be stepped on (not a wall)
                if (isWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }
        }
        //updates animator to show moving animation
        animator.SetBool("isMoving", isMoveing);
    }
    IEnumerator Move(Vector3 targetPos)
    {
        isMoveing = true; //block new imputs durring movement

        //keeps moving until characher gets to new position
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            // moves character to new position
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoveing = false; // done moving
    }

    private bool isWalkable(Vector3 targetPos)
    {
        //checking if the tile in front of player has an object on the solid object layer. meaning there is a wall there.
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) != null)
        {
            return false;
        }
        return true;
    }
}
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private bool canJump;
    public float moveSpeed = 5f;       // Horizontal movement speed
    public float jumpForce = 3f;       // Jump power
    private bool isGrounded = true;           // Check if player is on ground
    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] private float jumpCutMultiplier = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;      // Empty object under player’s feet
    public float groundCheckRadius = 1.2f;
    public LayerMask groundLayer;      // What counts as ground

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Horizontal movement
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Jump (only if grounded)
        Debug.Log(canJump);
        if (Input.GetButtonDown("Jump") && canJump)
        {
            Debug.Log("Jumped");
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
        if (collision.gameObject.tag == ("Ground"))
        {
            Debug.Log("on ground");
            canJump = true;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("left ground");
            canJump = false;
        }
    }
}

