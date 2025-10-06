using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Sprite debugLevel;
    [SerializeField] private List<Sprite> levels;

    public static LevelManager instance;

    private int levelToGenerate = 0;

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
        GenerateNextLevel();
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.R)) Reset();
    }

    public void GenerateNextLevel(bool credit = false)
    {
        if (debugLevel != null)
        {
            GenerateLevel.instance.Generate(debugLevel);
            return;
        }
        GenerateLevel.instance.Generate(levels[levelToGenerate]);
        levelToGenerate = (levelToGenerate + 1) % levels.Count;
        if (levelToGenerate == 0) Victory.victoryIsWin = true;
    }

    public void GenerateLastLevel()
    {
        levelToGenerate -= 2;
        //TODO: add secret levels here
        if (levelToGenerate < 0) levelToGenerate = 0;
        GenerateNextLevel();
    }

    void Reset()
    {
        levelToGenerate = 0;
        GenerateNextLevel();
    }
}
