using NUnit.Framework;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    [SerializeField] private Sprite invSprite;
    [SerializeField] private Sprite notReadySprite;
    private Sprite originalSprite;
    private SpriteRenderer sr;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        Assert.NotNull(sr);
        originalSprite = sr.sprite;
        Assert.NotNull(originalSprite);
    }

    void Update()
    {
        switch (Invincibility.instance.GetState())
        {
            case InvState.Invincible:
                sr.sprite = invSprite;
                break;
            case InvState.Ready:
                sr.sprite = originalSprite;
                break;
            case InvState.NotReady:
                sr.sprite = notReadySprite;
                break;
        }
    }
}
