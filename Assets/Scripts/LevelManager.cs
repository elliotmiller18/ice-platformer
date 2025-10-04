using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Sprite debugLevel;
    [SerializeField] private List<Sprite> levels;

    public static LevelManager instance;

    private int currentLevel = 0;

    void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;    
    }
    
    void Start()
    {
        GenerateNextLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Reset();
    }

    public void GenerateNextLevel()
    {
        if (debugLevel != null)
        {
            GenerateLevel.instance.Generate(debugLevel);
            return;
        }
        GenerateLevel.instance.Generate(levels[currentLevel]);
        currentLevel = (currentLevel + 1) % levels.Count;
        Health.instance.HardKill();
    }

    void Reset()
    {
        currentLevel = 0;
        GenerateNextLevel();
    }
}
