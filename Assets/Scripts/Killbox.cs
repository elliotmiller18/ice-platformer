using UnityEngine;

public class Killbox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        if (health == null) return;
        health.HardKill();
    }
}
