using UnityEngine;
using UnityEngine.UI;

public class CoolmodeController : MonoBehaviour
{
    [SerializeField] SpriteRenderer coolText;

    [SerializeField] Image win;
    [SerializeField] Sprite coolWinSprite;

    [SerializeField] Image deaths;
    [SerializeField] Sprite coolDeathSprite;

    public static CoolmodeController instance;

    void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;
    }

    void Start()
    {
        coolText.enabled = false;
    }

    public void ActivateCoolMode()
    {
        coolText.enabled = true;
        win.sprite = coolWinSprite;
        deaths.sprite = coolDeathSprite;
    }

    public void DeactivateCoolMode()
    {   
        coolText.enabled = false;
    }
}
