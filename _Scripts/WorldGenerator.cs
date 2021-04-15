using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public WorldRenderer worldRenderer;
    public BlockData blockData;

    public int mapLength = 50;
    public float amplitude = 1;
    public float frequency = 0.01f;

    public TextMesh textPrefab;

    private float GetNoiseValue(int x, int y)
    {
        return amplitude * Mathf.PerlinNoise(x * frequency, y * frequency);
    }

    public void GenerateMap1DNoise()
    {
        worldRenderer.ClearGroundTilemap();
        for (int x = 0; x < mapLength; x++)
        {
            var noise = GetNoiseValue(x, 1);
            var yCoordinate = Mathf.FloorToInt(noise);
            for (int y = 0; y <= yCoordinate; y++)
            {
                worldRenderer.SetGroundTile(x, y, blockData.dirtTile);
            }
        }
    }

    private void CreateNoiseValueText(int x, float y, float noise)
    {
        var t = Instantiate(textPrefab, new Vector3(x + 0.2f, y + 0.5f, 0), Quaternion.identity);
        t.text = noise.ToString("0.000");
        t.color = new Color(Mathf.Clamp01(noise), Mathf.Clamp01(noise), Mathf.Clamp01(noise), 1);
    }
}
