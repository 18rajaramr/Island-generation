using UnityEngine;
using Unity.Mathematics;
using System.IO;
using System.Collections;

public class IslandTextureGeneration : MonoBehaviour
{
    public ProgressBar ProgressBar_script;
    public Canvas Canvas_script;
    public int TextureSize;
    public float NoiseScale, IslandSize;
    [Range(1, 20)] public int NoiseOctaves;
    [Range(0, 999999)] public int seed;
    public bool Generate;

    // Privates
    private Color[] col;
    private Texture2D tex;
    public Gradient ColourGradient;
    private string filePath;

    private void Start()
    {
        // Set the file path where the texture will be saved
        filePath = Path.Combine(Application.persistentDataPath, "GeneratedMap.png");

        if (Generate)
        {
            Create();
        }
        else
        {
            Load();
        }
    }

    public void Create()
    {
        // Start the asynchronous texture generation process
        StartCoroutine(GenerateTextureCoroutine());
    }

    private IEnumerator GenerateTextureCoroutine()
    {
        tex = new Texture2D(TextureSize, TextureSize);
        col = new Color[tex.height * tex.width];

        Renderer rend = GetComponent<MeshRenderer>();
        rend.sharedMaterial.mainTexture = tex;

        // Set the filter mode to Point to avoid blurriness and achieve a pixelated effect
        tex.filterMode = FilterMode.Point;

        UnityEngine.Vector2 org = new UnityEngine.Vector2(Mathf.Sqrt(seed), Mathf.Sqrt(seed));

        // Generate the island texture asynchronously to prevent freezing
        for (int x = 0, i = 0; x < TextureSize; x++)
        {
            for (int y = 0; y < TextureSize; y++, i++)
            {
                float noiseValue = NoiseFunction(x, y, org);
                col[i] = ColourGradient.Evaluate(noiseValue); // Use the value once here
            }

            // Update the progress bar based on how much of the texture is done
            ProgressBar_script.IncrementProgress((float)x / TextureSize);

            // Yield every few rows to let Unity update the frame
            if (x % 10 == 0) 
            {
                yield return null;
            }
        }

        tex.SetPixels(col);
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;

        // Save the texture as a file after generation
        SaveTextureToFile(tex, "GeneratedMap");

        // Update the progress bar to show completion
        ProgressBar_script.IncrementProgress(1.0f);
        
        Canvas_script.GetComponent<Canvas> ().enabled = false;
    }

    public void Load()
    {
        // Check if the saved map exists
        if (File.Exists(filePath))
        {
            // Load the texture from the saved file
            byte[] fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(TextureSize, TextureSize);
            tex.filterMode = FilterMode.Point;
            tex.LoadImage(fileData);

            // Assign the texture to the material
            Renderer rend = GetComponent<MeshRenderer>();
            rend.sharedMaterial.mainTexture = tex;

            Debug.Log("Loaded texture from file: " + filePath);
        }
        else
        {
            Debug.LogError("Saved map file not found!");
        }

        Canvas_script.GetComponent<Canvas> ().enabled = false;
    }

    private float NoiseFunction(float x, float y, UnityEngine.Vector2 Origin)
    {
        float a = 0, NoiseSize = NoiseScale, opacity = 1;

        for (int octaves = 0; octaves < NoiseOctaves; octaves++)
        {
            float xVal = (x / (NoiseSize * TextureSize)) + Origin.x;
            float yVal = (y / (NoiseSize * TextureSize)) - Origin.y;
            float z = noise.snoise(new float2(xVal, yVal));
            a += Mathf.InverseLerp(0, 1, z) / opacity;

            NoiseSize /= 2f;
            opacity *= 2f;
        }
        return a -= FallOffMap(x, y, TextureSize, IslandSize);
    }

    private float FallOffMap(float x, float y, int size, float islandSize)
    {
        float gradient = 1;

        gradient /= (x * y) / (size * size) * (1 - (x / size)) * (1 - (y / size));
        gradient -= 16;
        gradient /= islandSize;

        return gradient;
    }

    private void SaveTextureToFile(Texture2D texture, string fileName)
    {
        // Encode texture into PNG format
        byte[] bytes = texture.EncodeToPNG();

        // Write the bytes to the file
        File.WriteAllBytes(filePath, bytes);

        // Log the file save path for debugging
        Debug.Log("Texture saved at: " + filePath);
    }
}
