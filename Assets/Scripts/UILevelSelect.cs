using UnityEngine;

public class UILevelSelect : MonoBehaviour
{
    public void OnLastLevelClick()
    {
        LevelManager.instance.GenerateLastLevel();
    }
    public void OnNextLevelClick()
    {
        LevelManager.instance.GenerateNextLevel();
    }
}
