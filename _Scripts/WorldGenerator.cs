using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    public WorldRenderer worldRenderer;
    public BlockData blockData;

    public NoiseDataSO heightMapNoiseData, stoneNoiseData, perlin2DData;

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

    public void GenerateMapBetter()
    {
        worldRenderer.ClearGroundTilemap();
        for (int x = -1* mapLength; x < mapLength; x++)
        {
            var noise = SumNoise(heightMapNoiseData.offset.x + x, 1, heightMapNoiseData);
            var noiseInRange = RangeMap(noise, 0, 1, heightMapNoiseData.noiseRangeMin, heightMapNoiseData.noiseRangeMax);
            var noiseEndValue = Mathf.FloorToInt(noiseInRange);

            var noiseStone = SumNoise(stoneNoiseData.offset.x + x, 1, stoneNoiseData);
            var noiseStoneInRange = RangeMap(noiseStone, 0, 1, stoneNoiseData.noiseRangeMin, stoneNoiseData.noiseRangeMax);
            var noiseStoneInt = Mathf.FloorToInt(noiseStoneInRange);

            for (int y = 0; y <= noiseEndValue; y++)
            {
                worldRenderer.SetGroundTile(x, y, SelectTile(y, noiseEndValue, noiseStoneInt));
            }
        }
    }

    private void CreateNoiseValueText(int x, float y, float noise)
    {
        var t = Instantiate(textPrefab, new Vector3(x + 0.2f, y + 0.5f, 0), Quaternion.identity);
        t.text = noise.ToString("0.000");
        t.color = new Color(Mathf.Clamp01(noise), Mathf.Clamp01(noise), Mathf.Clamp01(noise), 1);
    }

    public float SumNoise(int x, int y, NoiseDataSO noiseSettings)
    {
        float amplitude = 1;
        float frequency = noiseSettings.startFrequency;
        float noiseSum = 0;
        float amplitudeSum = 0;
        for (int i = 0; i < noiseSettings.octaves; i++)
        {
            noiseSum += amplitude * Mathf.PerlinNoise(x * frequency, y * frequency);
            amplitudeSum += amplitude;
            amplitude *= noiseSettings.persistance;
            frequency *= noiseSettings.frequencyModifier;

        }
        return noiseSum / amplitudeSum; //normalize [0-1]

    }

    private float RangeMap(float inputValue, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + (inputValue - inMin) * (outMax - outMin) / (inMax - inMin);
    }

    private TileBase SelectTile(int y, int noiseValue, int stoneHeight)
    {

        if (y >= stoneHeight)
        {
            if (y == noiseValue)
                return blockData.stoneGrass;
            return blockData.stoneTile;

        }
        else if (y == noiseValue)
        {
            return blockData.dirtGrass;
        }
        return blockData.dirtTile;
    }
}
