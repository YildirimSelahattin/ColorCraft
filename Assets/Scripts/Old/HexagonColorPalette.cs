using UnityEngine;

public class HexagonColorPalette : MonoBehaviour
{
    public int colorRings = 6;
    public int segmentsPerRing = 6;

    public Color[] colors;

    private void Awake()
    {
        GenerateColorPalette();
    }

    private void GenerateColorPalette()
    {
        colors = new Color[colorRings * segmentsPerRing];

        for (int ring = 0; ring < colorRings; ring++)
        {
            for (int segment = 0; segment < segmentsPerRing; segment++)
            {
                float hue = (float)segment / segmentsPerRing;
                float saturation = 1.0f;
                float luminance = (float)ring / (colorRings - 1);

                Color color = Color.HSVToRGB(hue, saturation, luminance);
                colors[ring * segmentsPerRing + segment] = color;
            }
        }
    }
}