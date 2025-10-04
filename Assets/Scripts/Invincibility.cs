using System.Collections;
using UnityEngine;

public enum InvState
{
    Invincible,
    NotReady,
    Ready
}

public class Invincibility : MonoBehaviour
{
    public static Invincibility instance;
    [SerializeField] private float invTimer = 0.75f;
    private InvState state = InvState.Ready;
    private Coroutine durationCoroutine;

    private bool readyQueued = false;

    void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;
    }

    void Update()
    {
        if (PlayerMovement.instance.GetGrounded())
        {
            if (state == InvState.NotReady) state = InvState.Ready;
            else if (state == InvState.Invincible) readyQueued = true;
            else readyQueued = false;
        }
        
        if (Input.GetKeyDown(KeyCode.Z) && state == InvState.Ready && durationCoroutine == null)
        {
            durationCoroutine = StartCoroutine(InvincibilityDuration());
        }
    }

    public void Reset()
    {
        if(durationCoroutine != null) StopCoroutine(durationCoroutine);
        durationCoroutine = null;
        state = InvState.Ready;
    }

    public InvState GetState() { return state; }

    IEnumerator InvincibilityDuration()
    {
        try
        {
            state = InvState.Invincible;
            yield return new WaitForSeconds(invTimer);
            // if the player is in the air by the end go on cooldown otherwise ready
            state = PlayerMovement.instance.GetGrounded() || readyQueued ? InvState.Ready : InvState.NotReady;
            readyQueued = false;
        }
        finally
        {
            durationCoroutine = null;
        }
    }
}
