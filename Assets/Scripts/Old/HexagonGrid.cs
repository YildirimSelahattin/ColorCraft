using UnityEngine;

public class HexagonGrid : MonoBehaviour
{
    public GameObject hexagonPrefab;
    public HexagonColorPalette colorPalette;

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        float hexWidth = hexagonPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        float hexHeight = hexagonPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

        int totalRings = colorPalette.colorRings;

        for (int ring = 0; ring < totalRings; ring++)
        {
            int hexagonsInRing = colorPalette.segmentsPerRing * ring;

            for (int i = 0; i < hexagonsInRing; i++)
            {
                float angle = (i * 360.0f) / hexagonsInRing;

                Vector3 position = new Vector3(
                    ring * hexWidth * Mathf.Cos(angle * Mathf.Deg2Rad),
                    ring * hexHeight * 0.75f * Mathf.Sin(angle * Mathf.Deg2Rad),
                    0
                );

                GameObject hex = Instantiate(hexagonPrefab, position, Quaternion.identity);
                hex.transform.SetParent(transform);

                int colorIndex = ring * colorPalette.segmentsPerRing + (i / ring);
                hex.GetComponent<SpriteRenderer>().color = colorPalette.colors[colorIndex];
            }
        }

        // Merkezde beyaz hexagon ekleme
        GameObject centerHex = Instantiate(hexagonPrefab, Vector3.zero, Quaternion.identity);
        centerHex.transform.SetParent(transform);
        centerHex.GetComponent<SpriteRenderer>().color = Color.white;
    }
}