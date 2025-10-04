using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private AudioClip jumpClip;
    private bool airborne;
    private Rigidbody2D rb;
    public static PlayerMovement instance;
    private float jumpCooldown = 0.15f;
    private float jumpTimer;

    private const float tileSize = 0.16f;

    void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        airborne = false;
        jumpTimer = jumpCooldown;
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
        if (Math.Abs(collision.transform.position.y - (transform.position.y - tileSize)) < 0.05f) { airborne = false; }
    }

    public bool GetGrounded()
    {
        return !airborne;
    }
}
