using System.IO;
using UnityEngine;

public class PerlinNoiseTest : MonoBehaviour
{
    public int width = 256; // Width of the texture
    public int height = 256; // Height of the texture

    public float xOrigin;
    public float yOrigin;
    public float x_tOrigin;
    public float y_tOrigin;
    public float x_mOrigin;
    public float y_mOrigin;

    public float scale = 1.0f;
    public float scale_t = 1.0f;
    public float scale_m = 1.0f;

    private Texture2D randomTexture;

    [ContextMenu("Generate Random Texture")]
    public void GenerateRandomTexture()
    {
        // Create a new Texture2D with specified width and height
        randomTexture = new Texture2D(width, height);
        var tectonicMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = x_tOrigin + x / (float)width * scale_t;
                float yCoord = y_tOrigin + y / (float)height * scale_t;
                var noise = Mathf.PerlinNoise(xCoord, yCoord);
                tectonicMap[x, y] = noise > 0.5f;
                //randomTexture.SetPixel(x, y, tectonicMap[x, y]? Color.red : Color.black);
            }
        }

        var moistureMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = x_mOrigin + x / (float)width * scale_m;
                float yCoord = y_mOrigin + y / (float)height * scale_m;
                var noise = Mathf.PerlinNoise(xCoord, yCoord);
                moistureMap[x, y] = noise > 0.5f;
                //randomTexture.SetPixel(x, y, moistureMap[x, y] ? Color.blue : Color.black);
            }
        }

        var climateMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                climateMap[x, y] = y > 0.8f * height || y < 0.2f * height;
                //randomTexture.SetPixel(x, y, climateMap[x, y] ? Color.magenta : Color.black);
            }
        }

        for (int x = 0; x < randomTexture.width; x++)
        {
            for (int y = 0; y < randomTexture.height; y++)
            {
                float xCoord = xOrigin + x / (float)width * scale;
                float yCoord = yOrigin + y / (float)height * scale;
                var noise = Mathf.PerlinNoise(xCoord, yCoord);
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
                
                randomTexture.SetPixel(x, y, color);
            }
        }

        randomTexture.Apply();
        var sprite = Sprite.Create(randomTexture, new Rect(0.0f, 0.0f, width, height), new Vector2(0.5f, 0.5f));
        sprite.texture.filterMode = FilterMode.Point;

        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void SaveTextureAsPNG()
    {
        var texture = GetComponent<SpriteRenderer>().sprite.texture;
        byte[] bytes = texture.EncodeToPNG();
        var path = Path.Combine(Application.dataPath, $"Texture/texture-{Random.Range(0, 10000)}.png");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllBytes(path, bytes);
        Debug.Log("Texture saved as PNG at " + path);
    }
}
