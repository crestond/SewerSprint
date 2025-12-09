using System.Collections;
using UnityEngine;

public class Level1IntroCutscene : MonoBehaviour
{
    [Header("References")]
    public PlayerMove2 playerMove;
    public Rigidbody2D playerRb;
    public Animator playerAnim;
    public Transform entrancePoint;
    public Transform sewerStartPoint;
    public Animator roofAnimator;
    public Collider2D exitBlocker;

    [Header("Settings")]
    public float walkSpeed = 3f;
    public float arrivalTolerance = 0.05f;
    public float pauseAtEntrance = 0.5f;
    public float pauseAfterCollapse = 1f;
    
    [Header("Collapse Visuals")]
    [SerializeField] private SpriteRenderer collapseSprite;  // debris pile sprite

    private void Start()
    {
        if (GameData.startingFromSave || GameData.hasSeenLevel1Intro)
        {
            // Make sure world is in "post-collapse" state
            if (collapseSprite != null)
                collapseSprite.enabled = true;

            if (exitBlocker != null)
                exitBlocker.enabled = true;

            // Make sure player has control
            if (playerMove != null)
                playerMove.SetAllowInput(true);

            Destroy(gameObject); // don’t run the cutscene
            return;
        }
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // 1. Turn off player input so keyboard doesn’t interfere
        if (playerMove != null)
        {
            playerMove.SetAllowInput(false);
        }

        // Make sure player faces right (if right is "into" the sewer)
        if (playerRb != null && playerAnim != null)
        {
            // based on your existing flip logic: scale (1,1,1) = facing right
            playerRb.transform.localScale = Vector3.one;
        }

        // 2. Walk to entrance
        yield return MovePlayerToX(entrancePoint.position.x);

        yield return new WaitForSeconds(pauseAtEntrance);

        // 3. Walk down tunnel to sewer start
        yield return MovePlayerToX(sewerStartPoint.position.x);

        // Stop the player
        if (playerRb != null)
        {
            playerRb.velocity = new Vector2(0f, playerRb.velocity.y);
        }
        if (playerAnim != null)
        {
            playerAnim.SetBool("walk", false);
        }

        // 4. Play roof cave-in
        if (roofAnimator != null)
        {
            roofAnimator.SetTrigger("Collapse");  // Make a "Collapse" trigger in Animator
        }
        
        if (collapseSprite != null)
        {
            collapseSprite.enabled = true;   // sprite appears now
        }
        
        if (exitBlocker != null)
        {
            exitBlocker.enabled = true;           // Now the player is trapped
        }

        yield return new WaitForSeconds(pauseAfterCollapse);

        // 5. Give control back
        if (playerMove != null)
        {
            playerMove.SetAllowInput(true);
        }

        GameData.hasSeenLevel1Intro = true;

        // Optional: destroy this controller once done
        Destroy(gameObject);
    }

    private IEnumerator MovePlayerToX(float targetX)
    {
        if (playerRb == null) yield break;

        if (playerAnim != null)
        {
            playerAnim.SetBool("walk", true);
        }

        while (Mathf.Abs(playerRb.transform.position.x - targetX) > arrivalTolerance)
        {
            float direction = Mathf.Sign(targetX - playerRb.transform.position.x);

            // Move using velocity
            playerRb.velocity = new Vector2(direction * walkSpeed, playerRb.velocity.y);

            // Flip sprite based on direction, same style as your PlayerMove2
            if (direction > 0.01f)
            {
                playerRb.transform.localScale = Vector3.one;
            }
            else if (direction < -0.01f)
            {
                playerRb.transform.localScale = new Vector3(-1, 1, 1);
            }

            yield return null; // wait for next frame
        }

        // Snap position and stop
        playerRb.transform.position = new Vector3(
            targetX,
            playerRb.transform.position.y,
            playerRb.transform.position.z
        );

        playerRb.velocity = new Vector2(0f, playerRb.velocity.y);

        if (playerAnim != null)
        {
            playerAnim.SetBool("walk", false);
        }
    }
}
