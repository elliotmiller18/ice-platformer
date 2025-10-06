using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using System.Collections; // Required for TextMeshPro components

public class TextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI deathText;

    private int winCount = 0;
    private int deathCount = 0;
    private bool deathIncrementLock = false;

    public static TextManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        Assert.IsNotNull(winText);
        Assert.IsNotNull(deathText);

        // Initialize the text displays
        UpdateWinText();
        UpdateDeathText();
    }

    public void IncrementWins()
    {
        winCount++;
        UpdateWinText();
    }

    public void IncrementDeaths()
    {
        StartCoroutine(ImplIncrementDeaths());
    }

    private void UpdateWinText()
    {
        winText.text = "Wins: " + winCount;
    }

    private void UpdateDeathText()
    {
        deathText.text = "Deaths:\n" + deathCount;
    }

    IEnumerator ImplIncrementDeaths()
    {
        if (deathIncrementLock) yield break;
        deathIncrementLock = true;
        deathCount++;
        UpdateDeathText();
        yield return new WaitForSeconds(0.1f);
        deathIncrementLock = false;
    }
}