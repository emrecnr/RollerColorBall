using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Texture")]
    [SerializeField] private Texture2D levelTexture;
    [Header("Tiles Prefabs")]
    [SerializeField] private GameObject prefabWallTile;
    [SerializeField] private GameObject prefabRoadTile;

    private Color colorWall = Color.white;
    private Color colorRoad = Color.black;

    private float unitPerPixel;

    private void Awake()
    {
        // Level generation process is initiated within the Awake method.
        GenerateTile();
    }

    private void GenerateTile()
    {
        // Calculate the unit equivalent of a pixel.
        unitPerPixel = prefabWallTile.transform.lossyScale.x;
        float halfUnit = unitPerPixel / 2f;
        float width = levelTexture.width;
        float height = levelTexture.height;

        // Calculate an offset based on the level size.
        Vector3 offset = (new Vector3(width / 2f, 0f, height / 2f) * unitPerPixel) - new Vector3(halfUnit, 0f, halfUnit);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Get the color of the pixel.
                Color pixelColor = levelTexture.GetPixel(x, y);
                // Determine the position where the object will be spawned.
                Vector3 spawnPos = ((new Vector3(x, 0f, y) * unitPerPixel) - offset);

                // Create wall or road based on pixel color.
                if (pixelColor == colorWall)
                {
                    Spawn(prefabWallTile, spawnPos);
                }
                else if (pixelColor == colorRoad)
                {
                    Spawn(prefabRoadTile, spawnPos);
                }
            }
        }
    }

    private void Spawn(GameObject prefabTile, Vector3 position)
    {
        // Determine the position where the object will be created.
        position.y = prefabTile.transform.position.y;
        // Instantiate the object.
        GameObject obj = Instantiate(prefabTile, position, Quaternion.identity, transform);
    }
}
