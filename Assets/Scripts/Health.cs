using UnityEngine;

public class Health : MonoBehaviour
{
    private Vector2 originalPos;
    [SerializeField] private AudioClip dieClip;
    public static Health instance;

    void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;
    }

    void Start()
    {
        originalPos = transform.position;
    }

    public void SoftKill()
    {
        if (Invincibility.instance.GetState() == InvState.Invincible) return;
        Kill();
    }

    public void HardKill() {
        Kill();
    }

    void Kill()
    {
        AudioSource.PlayClipAtPoint(dieClip, Camera.main.transform.position, 0.4f);
        gameObject.transform.position = originalPos;
        Invincibility.instance.Reset();
    }
}
