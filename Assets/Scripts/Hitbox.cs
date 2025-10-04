using NUnit.Framework;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private Sprite disabledSprite;
    private Sprite originalSprite;
    private SpriteRenderer sr;
    private bool invincible;

    void Start()
    {
        invincible = false;
        sr = GetComponent<SpriteRenderer>();
        Assert.NotNull(sr);
        originalSprite = sr.sprite;
    }
 
    void OnTriggerStay2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        if (health == null || invincible) return;
        health.SoftKill();
    }

    void Update()
    {
        invincible = Invincibility.instance.GetState() == InvState.Invincible;
        sr.sprite = invincible ? disabledSprite : originalSprite;
    }
}
