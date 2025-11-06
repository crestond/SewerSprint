using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private float speed = 10;
    [SerializeField] private float JumpForce = 7f;
    [SerializeField] private float jumpCutMultiplier = 2f;
    private bool canJump;
    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    
    private void Awake()
    {
        //grabs references
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
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
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Hazard")
        {
            canJump = true;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Hazard")
        {
            canJump = false;
        }
    }
}
