using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class Color32ToPrefabMapping
{
    public Color32 colorKey;
    public GameObject prefab;
}

public class GenerateLevel : MonoBehaviour
{
    public static GenerateLevel instance;
    [SerializeField] private List<Color32ToPrefabMapping> mappings;
    private Dictionary<Color32, GameObject> prefabMappings;

    private const int LEVEL_WIDTH = 32;
    private const int LEVEL_HEIGHT = 32;
    private const float TILE_SIZE = 0.16f;

    void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;
        prefabMappings = new Dictionary<Color32, GameObject>();
        foreach (Color32ToPrefabMapping mapping in mappings)
        {
            prefabMappings[mapping.colorKey] = mapping.prefab;
        }
    }

    public void Generate(Sprite level_sprite)
    {
        Texture2D level_texture = level_sprite.texture;

        Assert.AreEqual(level_sprite.rect.width, LEVEL_WIDTH);
        Assert.AreEqual(level_sprite.rect.height, LEVEL_HEIGHT);
        // AI CODE STARTS
        Color[] pixelBlock = level_texture.GetPixels(
            (int)level_sprite.rect.x,
            (int)level_sprite.rect.y,
            LEVEL_WIDTH,
            LEVEL_HEIGHT
        );

        for (int y = 0; y < LEVEL_HEIGHT; y++)
        {
            for (int x = 0; x < LEVEL_WIDTH; x++)
            {
                Color32 pixelColor = pixelBlock[y * LEVEL_WIDTH + x];
                if (pixelColor.a > 0)
                {
                    // AI CODE ENDS
                    Assert.IsNotNull(prefabMappings[pixelColor]);
                    Vector2 newPos = new Vector2(x * TILE_SIZE, y * TILE_SIZE);
                    GameObject tile = Instantiate(prefabMappings[pixelColor], transform);
                    tile.transform.position = newPos;
                }
            }
        }
    }
}
