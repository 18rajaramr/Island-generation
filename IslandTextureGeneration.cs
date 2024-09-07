using Unity.Mathematics;
using UnityEngine;

public class IslandTextureGeneration : MonoBehaviour
{
    public int TextureSize;
    public float NoiseScale, IslandSize;
    [Range(1, 20)] public int NoiseOctaves;
    [Range(0, 999999)] public int seed;

    // Privates
    private Color[] col;
    private Texture2D tex;

    public Gradient ColourGradient;

    private void Start()
    {
        tex = new Texture2D(TextureSize, TextureSize);
        col = new Color[tex.height * tex.width];

        Renderer rend = GetComponent<MeshRenderer>();
        rend.sharedMaterial.mainTexture = tex;

        // Set the filter mode to Point to avoid blurriness and achieve a pixelated effect
        tex.filterMode = FilterMode.Point;

        UnityEngine.Vector2 org = new UnityEngine.Vector2(Mathf.Sqrt(seed), Mathf.Sqrt(seed));

        for (int x = 0, i = 0; x < TextureSize; x++)
        {
            for (int y = 0; y < TextureSize; y++, i++)
            {
                float a = NoiseFunction(x, y, org);
                col[i] = ColourGradient.Evaluate(NoiseFunction((float)x, (float)y, org));
            }
        }

        tex.SetPixels(col);
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
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
}
