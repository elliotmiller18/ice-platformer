using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Victory : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    private bool triggered = false;

    // janky but works
    IEnumerator AllowGeneration()
    {
        yield return new WaitForSeconds(2);
        triggered = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered || !collision.CompareTag("Player")) return;
        triggered = true;
        StartCoroutine(AllowGeneration());
        LevelManager.instance.GenerateNextLevel(true);
    }
}
