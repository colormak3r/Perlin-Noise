using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct Tile
{
    public MinMaxFloat elevation;
    public Color color;
}

public struct MinMaxFloat
{
    public float min, max;
}

public class PerlinNoiseTest : MonoBehaviour
{
    [Header("General")]
    public int width;
    public int height;

    /*[Header("Color")]
    public Color grassColor;
    public Color grassColor;*/

    [Header("Elevation Map")]
    public SpriteRenderer elevationMapRenderer;
    public Vector2 elevationMapOrigin;
    public float elevationMapScale = 1.0f;
    public int octaves = 3;
    public float exp = 1f;
    public float persistence = 0.5f;
    public float frequencyBase = 2f;

    [Header("Tectonic Map")]
    public SpriteRenderer tectonicMapRenderer;
    public Vector2 tectonicMapOrigin;
    public float tectonicMapScale = 1.0f;

    [Header("Moisture Map")]
    public SpriteRenderer moistureMapRenderer;
    public Vector2 moistureMapOrigin;
    public float moistureMapScale = 1.0f;

    [Header("Climate Map")]
    public SpriteRenderer climateMapRenderer;
    public float climateZone = 0.2f;

    [Header("Combined Map")]
    public SpriteRenderer combinedRenderer;

    private Texture2D elevationMapTexture;
    private Texture2D tectonicMapTexture;
    private Texture2D moistureMapTexture;
    private Texture2D climateMapTexture;
    private Texture2D combinedTexture;

    [ContextMenu("Generate Random Texture")]
    public void GenerateRandomTexture()
    {
        elevationMapTexture = new Texture2D(width, height);

        var elevationMap = new float[width, height];
        

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = elevationMapOrigin.x + (float)x / width * elevationMapScale;
                float yCoord = elevationMapOrigin.y + (float)y / height * elevationMapScale;
                
                // Octaves
                var total = 0f;
                var frequency = 1f;
                var amplitude = 1f;
                var maxValue = 0f;
                for (int i = 0; i < octaves; i++)
                {
                    total += Mathf.PerlinNoise(xCoord * frequency, yCoord * frequency) * amplitude;

                    maxValue += amplitude;
                    amplitude *= persistence;
                    frequency *= frequencyBase;
                }
                float noise = Mathf.Pow(total / maxValue, exp);

                elevationMap[x, y] = noise;
                var color = Color.white;
                if (noise > 0.7f)
                {
                    color = Color.grey;
                }
                else if (noise > 0.5f)
                {
                    color = Color.green;
                }
                else if (noise > 0.4f)
                {
                    color = Color.yellow;
                }
                else
                {
                    color = Color.blue;
                }
                elevationMapTexture.SetPixel(x, y, color);
            }
        }

        elevationMapTexture.Apply();
        var elevationMapSprite = Sprite.Create(elevationMapTexture, new Rect(0.0f, 0.0f, width, height), new Vector2(0.5f, 0.5f));
        elevationMapSprite.texture.filterMode = FilterMode.Point;
        elevationMapRenderer.sprite = elevationMapSprite;

        var tectonicMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = tectonicMapOrigin.x + (float)x / width * tectonicMapScale;
                float yCoord = tectonicMapOrigin.y + (float)y / height * tectonicMapScale;
                var noise = Mathf.PerlinNoise(xCoord, yCoord);
                tectonicMap[x, y] = noise > 0.5f;
                tectonicMapTexture.SetPixel(x, y, tectonicMap[x, y] ? Color.red : Color.black);
            }
        }

        tectonicMapTexture.Apply();
        var tectonicMapSprite = Sprite.Create(tectonicMapTexture, new Rect(0.0f, 0.0f, width, height), new Vector2(0.5f, 0.5f));
        tectonicMapSprite.texture.filterMode = FilterMode.Point;
        tectonicMapRenderer.sprite = tectonicMapSprite;

        var climateMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                climateMap[x, y] = y > (1 - climateZone) * height || y < climateZone * height;
                climateMapTexture.SetPixel(x, y, climateMap[x, y] ? Color.magenta : Color.black);
            }
        }

        climateMapTexture.Apply();
        var climateMapSprite = Sprite.Create(climateMapTexture, new Rect(0.0f, 0.0f, width, height), new Vector2(0.5f, 0.5f));
        climateMapSprite.texture.filterMode = FilterMode.Point;
        climateMapRenderer.sprite = climateMapSprite;

        var moistureMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = moistureMapOrigin.x + (float)x / width * moistureMapScale;
                float yCoord = moistureMapOrigin.y + (float)y / height * moistureMapScale;
                var noise = Mathf.PerlinNoise(xCoord, yCoord);
                moistureMap[x, y] = noise > 0.5f;
                moistureMapTexture.SetPixel(x, y, moistureMap[x, y] ? Color.blue : Color.black);
            }
        }

        moistureMapTexture.Apply();
        var moistureMapSprite = Sprite.Create(moistureMapTexture, new Rect(0.0f, 0.0f, width, height), new Vector2(0.5f, 0.5f));
        moistureMapSprite.texture.filterMode = FilterMode.Point;
        moistureMapRenderer.sprite = moistureMapSprite;

        for (int x = 0; x < combinedTexture.width; x++)
        {
            for (int y = 0; y < combinedTexture.height; y++)
            {
                var noise = elevationMap[x, y];
                var color = Color.white;
                if (tectonicMap[x, y])
                {
                    if (noise > 0.8f)
                        color = Color.grey;
                    else if (noise > 0.5f)
                    {
                        if (climateMap[x, y])
                            color = Color.cyan;
                        else
                            color = Color.green;
                    }
                    else if (noise > 0.4f)
                    {
                        if (moistureMap[x, y])
                            color = Color.yellow;
                        else
                            color = Color.blue;
                    }
                    else
                        color = Color.blue;
                }
                else
                {
                    color = Color.blue;
                }

                combinedTexture.SetPixel(x, y, color);
            }
        }

        combinedTexture.Apply();
        var combinedSprite = Sprite.Create(combinedTexture, new Rect(0.0f, 0.0f, width, height), new Vector2(0.5f, 0.5f));
        combinedSprite.texture.filterMode = FilterMode.Point;
        combinedRenderer.sprite = combinedSprite;
    }

    public void ExportMaps()
    {
        SaveTextureAsPNG(combinedTexture, "combined");
        SaveTextureAsPNG(moistureMapTexture, "moisture");
        SaveTextureAsPNG(climateMapTexture, "climate");
        SaveTextureAsPNG(elevationMapTexture, "elevation");
        SaveTextureAsPNG(tectonicMapTexture, "tectonic");
    }

    public void SaveTextureAsPNG(Texture2D texture, string name)
    {
        byte[] bytes = texture.EncodeToPNG();
        var path = Path.Combine(Application.dataPath, $"Texture/{name}-{Random.Range(0, 10000)}.png");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllBytes(path, bytes);
        Debug.Log("Texture saved as PNG at " + path);
    }
}