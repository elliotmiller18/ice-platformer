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
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        prefabMappings = new Dictionary<Color32, GameObject>();
        foreach (Color32ToPrefabMapping mapping in mappings)
        {
            prefabMappings[mapping.colorKey] = mapping.prefab;
        }
    }

    public void Generate(Sprite level_sprite)
    {
        // nuke every tile generated already
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Texture2D level_texture = level_sprite.texture;

        bool spawned = false;

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
            GameObject currentCompositeParent = null;

            for (int x = 0; x < LEVEL_WIDTH; x++)
            {
                Vector2 posVector = new Vector2(x * TILE_SIZE, y * TILE_SIZE);
                Color32 pixelColor = pixelBlock[y * LEVEL_WIDTH + x];
                if (pixelColor == Color.white)
                {
                    Assert.IsFalse(spawned);
                    PlayerMovement.instance.SpawnAtNewPos(posVector);
                    spawned = true;
                    currentCompositeParent = null;
                }
                else if (pixelColor.a > 0)
                {

                    GameObject prefabToSpawn = prefabMappings[pixelColor];
                    Assert.IsNotNull(prefabToSpawn);

                    if (HasNonTriggerBoxCollider2D(prefabToSpawn))
                    {
                        // If this is the first tile in a new contiguous block, create the composite parent.
                        if (currentCompositeParent == null)
                        {
                            currentCompositeParent = new GameObject($"Composite Row {y} - Block");
                            currentCompositeParent.layer = LayerMask.NameToLayer("Ground");
                            currentCompositeParent.transform.SetParent(transform);
                            var rb = currentCompositeParent.AddComponent<Rigidbody2D>();
                            rb.bodyType = RigidbodyType2D.Static;
                            currentCompositeParent.AddComponent<CompositeCollider2D>();
                        }

                        // Instantiate the tile as a child of the composite parent.
                        GameObject tile = Instantiate(prefabToSpawn, currentCompositeParent.transform);
                        tile.transform.position = posVector;

                        foreach (var bc in tile.GetComponentsInChildren<BoxCollider2D>())
                        {
                            if(!bc.isTrigger) bc.compositeOperation = Collider2D.CompositeOperation.Merge;
                        }
                    }
                    else
                    {
                        // This tile does not have a non-trigger collider, so it breaks the contiguous block.
                        currentCompositeParent = null;
                        GameObject tile = Instantiate(prefabToSpawn, transform);
                        tile.transform.position = posVector;
                    }
                    // AI CODE ENDS
                }
                else // An empty pixel (alpha == 0) also breaks a contiguous block.
                {
                    currentCompositeParent = null;
                }
            }
        }
        Assert.IsTrue(spawned, "Player should be spawned during level generation");
    }

    private bool HasNonTriggerBoxCollider2D(GameObject prefab)
    {
        // Get all BoxCollider2D components from the prefab and its children.
        BoxCollider2D[] colliders = prefab.GetComponentsInChildren<BoxCollider2D>();
        foreach (var collider in colliders)
        {
            // If we find any collider that is not set to be a trigger, this prefab qualifies.
            if (!collider.isTrigger)
            {
                return true;
            }
        }
        // If we loop through all colliders and none are non-triggers, it does not qualify.
        return false;
    }
}


