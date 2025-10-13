using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DotsAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> rdots;
    [SerializeField] List<Sprite> bdots;
    private int idx = 0;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Assert.AreEqual(rdots.Count, bdots.Count);
        Assert.IsTrue(rdots.Count > 0);
        Assert.IsNotNull(sr);
        StartCoroutine(UpdateIdx());
    }

    void Update()
    {
        if (Invincibility.instance.GetState() == InvState.Invincible)
        {
            sr.sprite = bdots[idx];
        }
        else
        {
            sr.sprite = rdots[idx];
        }
    }

    IEnumerator UpdateIdx()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            idx = Random.Range(0, rdots.Count);
        }
    }
}
