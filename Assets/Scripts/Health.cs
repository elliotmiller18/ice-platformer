using UnityEngine;

public class Health : MonoBehaviour
{
    private Vector2 originalPos;
    [SerializeField] private AudioClip dieClip;
    public static Health instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        originalPos = transform.position;
    }

    public void SoftKill()
    {
        if (Invincibility.instance.GetState() == InvState.Invincible) return;
        AudioSource.PlayClipAtPoint(dieClip, Camera.main.transform.position, 0.4f);
        Kill();
    }

    public void HardKill(bool silent = false) {
        if (!silent)
        {
            AudioSource.PlayClipAtPoint(dieClip, Camera.main.transform.position, 0.4f);
        }
        Kill();
    }

    void Kill()
    {
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        PlayerMovement.instance.Respawn();
        Invincibility.instance.Reset();
    }
}
