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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        LevelManager.instance.GenerateNextLevel();
    }
}
