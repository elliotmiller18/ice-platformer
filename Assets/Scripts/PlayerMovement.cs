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
    private Rigidbody2D rb;
    private Vector2 spawnPos;
    private float jumpTimer;
    private float jumpCooldown = 0.15f;
    private Coroutine wallAnim = null;

    public static PlayerMovement instance;
    private const float TILE_SIZE = 0.16f;

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
        UpdateGroundedStatus();
        
        //gross? yes. fixes the weird rotation bug? yes.
        transform.rotation = Quaternion.identity;
        if (Input.GetAxisRaw("Vertical") > 0f && !airborne && jumpTimer > jumpCooldown)
        {
            jumpTimer = 0;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            AudioSource.PlayClipAtPoint(jumpClip, Camera.main.transform.position, 0.4f);
            airborne = true;
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
        }
    }
    //START OF AI CODE
    void UpdateGroundedStatus()
    {
        // Physics2D.OverlapCircle returns true if any collider on the specified layer
        // is within the circle defined by the position and radius.
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // The player is airborne if they are NOT grounded.
        airborne = !isGrounded;
    }
    //END OF AI CODE
}

