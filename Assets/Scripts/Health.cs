using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private AudioClip dieClip;
    [SerializeField] private float volume = 0.08f;
    public static Health instance;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void SoftKill()
    {
        if (Invincibility.instance.GetState() == InvState.Invincible) return;
        AudioSource.PlayClipAtPoint(dieClip, Camera.main.transform.position, volume);
        Kill();
    }

    public void HardKill(bool silent = false) {
        if (!silent)
        {
            AudioSource.PlayClipAtPoint(dieClip, Camera.main.transform.position, volume);
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
