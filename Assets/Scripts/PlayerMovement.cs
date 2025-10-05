using System;
using UnityEngine.Assertions;
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private SpriteRenderer wallStraddleSprite;

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
        if (instance != null && instance == this)
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        // if it's a blackbox return, we'll never land on one
        if (collision.gameObject.CompareTag("BlackBox")) return;
        // if we're around a tile above what we're colliding with it's probably a downward collision
        if (Math.Abs(collision.transform.position.y - (transform.position.y - TILE_SIZE)) < 0.05f) { airborne = false; }
    }

    public bool GetGrounded()
    {
        return !airborne;
    }

    IEnumerator WallStraddleAnimation(bool left)
    {
        Debug.Log("starting");
        Vector2 startingPos = rb.position;
        yield return new WaitForSeconds(0.4f);

        if (Vector2.Distance(startingPos, rb.position) >= 0.05f)
        {
            Debug.Log("breaking");
            wallAnim = null;
            yield break;
        }

        wallStraddleSprite.enabled = true;
        if (left) wallStraddleSprite.flipX = true;

        try
        {
            while (Vector2.Distance(startingPos, rb.position) < 0.05f)
            {
                //AI CODE STARTS HERE
                float colorValue = (Mathf.Sin(Time.time * 5f) + 1) / 2f;
                wallStraddleSprite.color = new Color(colorValue, colorValue, colorValue);
                yield return null;
                //AI CODE ENDS HERE
            }
        }
        finally
        {
            Debug.Log("ending");
            wallStraddleSprite.enabled = false;
            wallStraddleSprite.flipX = false;
            wallStraddleSprite.color = Color.white;
            wallAnim = null;
        }
    }
}
