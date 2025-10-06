using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Victory : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private float cycleTime = 0.2f;
    private int currentSpriteIdx = 0;
    private SpriteRenderer sr;
    private bool triggered = false;
    public static bool victoryIsWin = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(sr);
        Assert.IsTrue(sprites.Count > 1);
        StartCoroutine(CycleSprite());
    }

    IEnumerator CycleSprite()
    {
        while (true)
        {
            yield return new WaitForSeconds(cycleTime);
            sr.sprite = sprites[currentSpriteIdx];
            currentSpriteIdx = (currentSpriteIdx + 1) % sprites.Count;
        }
    }

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
        if (victoryIsWin)
        {
            victoryIsWin = false;
            TextManager.instance.IncrementWins();
        }
        LevelManager.instance.GenerateNextLevel(true);
    }
}
