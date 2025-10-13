using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Sprite debugLevel;
    [SerializeField] private List<Sprite> levels;

    public static LevelManager instance;

    private int levelToGenerate = 3;

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

    public void GenerateNextLevel(bool credit = false)
    {
        if (debugLevel != null)
        {
            GenerateLevel.instance.Generate(debugLevel);
            return;
        }
        GenerateLevel.instance.Generate(levels[levelToGenerate]);
        levelToGenerate = (levelToGenerate + 1) % levels.Count;
        if (levelToGenerate == 1 && credit) TextManager.instance.IncrementWins();

        if (levelToGenerate == 4 && credit) CoolmodeController.instance.ActivateCoolMode();
        else CoolmodeController.instance.DeactivateCoolMode();
    }

    public void GenerateLastLevel()
    {
        if (levelToGenerate == 0) levelToGenerate = levels.Count - 2;
        else if (levelToGenerate == 1) levelToGenerate = levels.Count - 1;
        else levelToGenerate -= 2;
        //TODO: add secret levels here
        GenerateNextLevel();
    }
}
