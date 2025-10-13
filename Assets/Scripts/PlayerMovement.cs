using UnityEngine.Assertions;
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private SpriteRenderer wallStraddleSprite;

    //START OF AI CODE
    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck; // An empty object at the player's feet
    [SerializeField] private float groundCheckRadius = 0.2f; // The size of the detection circle
    [SerializeField] private LayerMask groundLayer; // Which layers are considered 'ground'
    //END OF AI CODE

    private bool airborne;
    private bool canWallJump;
    private Rigidbody2D rb;
    private Vector2 spawnPos;
    private float jumpTimer;
    private float jumpCooldown = 0.15f;
    private Coroutine wallAnim = null;

    public static PlayerMovement instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);
        airborne = false;
        canWallJump = false;
        jumpTimer = jumpCooldown;
    }

    public void SpawnAtNewPos(Vector2 pos)
    {
        rb.position = pos;
        rb.linearVelocity = Vector2.zero;
        spawnPos = pos;
        //reset everything we need in HardKill and respawn after setting the new spawn
        //position
        Health.instance.HardKill(true);
    }

    public void Respawn()
    {
        rb.position = spawnPos;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAirborneStatus();
        
        //gross? yes. fixes the weird rotation bug? yes.
        transform.rotation = Quaternion.identity;
        if (Input.GetAxisRaw("Vertical") > 0f
            && (!airborne || canWallJump)
            && jumpTimer > jumpCooldown)
        {
            jumpTimer = 0;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            AudioSource.PlayClipAtPoint(jumpClip, Camera.main.transform.position, 0.1f);
            airborne = true;
            canWallJump = false;
        }

        float horiz = Input.GetAxisRaw("Horizontal");

        if (horiz != 0 && wallAnim == null && airborne &&
            Mathf.Abs(rb.linearVelocityY) < 0.02f)
        {
            wallAnim = StartCoroutine(WallStraddleAnimation(horiz < 0));
        }

        Vector2 old = rb.linearVelocity;
        old.x = horiz * movementSpeed;
        rb.linearVelocity = old;
        jumpTimer += Time.deltaTime;
    }

    public bool GetGrounded()
    {
        return !airborne;
    }

    IEnumerator WallStraddleAnimation(bool left)
    {
        Vector2 startingPos = rb.position;
        yield return new WaitForSeconds(0.4f);

        if (Vector2.Distance(startingPos, rb.position) >= 0.05f)
        {
            wallAnim = null;
            yield break;
        }

        wallStraddleSprite.enabled = true;
        if (left) wallStraddleSprite.flipX = true;
        canWallJump = true;

        try
        {
            while (Vector2.Distance(startingPos, rb.position) < 0.05f)
            {

                //START OF AI CODE
                float colorValue = (Mathf.Sin(Time.time * 5f) + 1) / 2f;
                wallStraddleSprite.color = new Color(colorValue, colorValue, colorValue);
                yield return null;
                //END OF AI CODE
            }
        }
        finally
        {
            wallStraddleSprite.enabled = false;
            wallStraddleSprite.flipX = false;
            wallStraddleSprite.color = Color.white;
            wallAnim = null;
            canWallJump = false;
        }
    }
    //START OF AI CODE
    void UpdateAirborneStatus()
    {
        // Get all colliders overlapping the ground check circle.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);

        // Assume we are airborne until we find a valid ground surface.
        bool isGrounded = false;

        // Loop through every collider we found.
        foreach (Collider2D hit in colliders)
        {
            // First, we can't stand on triggers, so ignore any collider that is one.
            // This automatically handles your two trigger-only tile types.
            if (hit.isTrigger)
            {
                continue;
            }

            // Next, check if the solid surface we've found is dangerous lava.
            // This requires your lava tile prefab/GameObject to have the tag "Lava".
            if (hit.CompareTag("Lava") && Invincibility.instance.GetState() != InvState.Invincible)
            {
                // If it's lava AND we're not invincible, this surface is not "ground".
                // We continue the loop to see if we're also touching a safe tile.
                continue;
            }

            // If we've passed all the checks, we've found a solid, safe surface.
            // This could be a normal walkable tile, or a lava tile while we're invincible.
            isGrounded = true;
            break; // We found ground, so we can stop checking.
        }

        // Update the airborne status based on whether we found any valid ground.
        airborne = !isGrounded;
    }
    //END OF AI CODE
}

