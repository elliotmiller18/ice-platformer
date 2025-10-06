using UnityEngine;

public class Killbox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        TextManager.instance.IncrementDeaths();
        Health.instance.HardKill();
    }
}
