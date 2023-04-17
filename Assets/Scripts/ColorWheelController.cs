using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColorWheelController : MonoBehaviour
{
    public GameObject hexagonPrefab;
    public int numberOfRings = 0;
    public float hexagonSize = 1f;
    public Image targetImage;
    public List<GameObject> hexagonList;

    private void Start()
    {
        if (GameDataManager.Instance.rawData.levelsArray[0].level.levelType == "hexagon")
        {
            numberOfRings = GameDataManager.Instance.rawData.levelsArray[0].level.levelParametres[0];
            CreateColorWheel();
        }
    }

    private void CreateColorWheel()
    {
        for (int ring = 0; ring < numberOfRings; ring++)
        {
            int hexagonsInRing = ring == 0 ? 1 : ring * 6;

            for (int i = 0; i < hexagonsInRing; i++)
            {
                GameObject hexagon = Instantiate(hexagonPrefab);
                hexagon.transform.SetParent(transform);
                hexagon.transform.localPosition = CalculateHexagonPosition(ring, i);
                hexagon.transform.localScale = Vector3.one * hexagonSize;
                Color color = CalculateHexagonColor(ring, i);
                hexagon.GetComponent<Hexagon>().SetColor(color);
                hexagon.GetComponent<Hexagon>().index = ring*hexagonsInRing+i;
                hexagonList.Add(hexagon);
            }
        }
        targetImage.color = hexagonList[GameDataManager.Instance.rawData.levelsArray[GameDataManager.Instance.currentLevel].winIndex].GetComponent<SpriteRenderer>().color;
    }

    private Vector3 CalculateHexagonPosition(int ring, int index)
    {
        if (ring == 0)
        {
            return Vector3.zero;
        }

        int hexagonsInRing = ring * 6;
        int segment = index / ring;
        int positionInSegment = index % ring;

        Vector3 cornerPosition = HexMetrics.corners[segment] * ring;
        Vector3 offset = (HexMetrics.corners[segment + 1] - HexMetrics.corners[segment]) * positionInSegment;

        return cornerPosition + offset;
    }

    private Color CalculateHexagonColor(int ring, int index)
    {
        if (ring == 0)
        {
            return Color.HSVToRGB(0f, 0f, 1f);
        }

        float hue = (float)(index % (ring * 6)) / (ring * 6);
        float saturation = (float)ring / numberOfRings;
        float value = 1f;

        return Color.HSVToRGB(hue, saturation, value);
    }
    
    //for Camera
    public Vector2 GetBoundary()
    {
        float horizontalBoundary = numberOfRings * hexagonSize * 0.87f;
        float verticalBoundary = numberOfRings * hexagonSize * 0.755f;
        return new Vector2(horizontalBoundary, verticalBoundary);
    }
    
    public float GetOuterRadius()
    {
        return numberOfRings * hexagonSize;
    }
}