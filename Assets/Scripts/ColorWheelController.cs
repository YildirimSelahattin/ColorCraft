using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ColorWheelController : MonoBehaviour
{
    public static ColorWheelController Instance;
    public GameObject hexagonPrefab;
    public int numberOfRings = 0;
    public float hexagonSize = 1f;
    public Image targetImage;
    public List<GameObject> hexagonList;
    public GameObject textPrefab;
    public bool isFinished = false;
    public bool isGettingTouch;
    public static bool loadDeckDirectly = false;
    
    private void Start()
    {
     
        if(Instance== null)
        {
            Instance = this;    
        }
        if (loadDeckDirectly)
        {
            StartCreatingEnvironment(GameDataManager.Instance.currentLevel);
            isGettingTouch = true;
            loadDeckDirectly = false;
        }

        CreateColorWheel();
    }
    

    public void StartCreatingEnvironment(int levelNumber)
       
    {
        if (GameDataManager.Instance.rawData.levelsArray[0].level.levelType == "hexagon")
        {
            numberOfRings = 17;
            CreateColorWheel();
        }
    }
    
    private void Update()
    {
        if (isFinished)
        {
            StartCoroutine(AnimateHexagons());
        }
    }

    private void CreateColorWheel()
    {
        int index = 0;
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
                hexagon.GetComponent<Hexagon>().index = index;
                //FOR LEVEL PURPOSES
                //GameObject temp = Instantiate(textPrefab,hexagon.transform);
                //temp.GetComponent<TextMeshPro>().text = index.ToString();
                hexagonList.Add(hexagon);
                index++;
            }
        }
        targetImage.color = hexagonList[GameDataManager.Instance.rawData.levelsArray[GameDataManager.Instance.currentLevel].winIndex].GetComponent<SpriteRenderer>().color;
        UIManager.Instance.winTargetImage.color = hexagonList[GameDataManager.Instance.rawData.levelsArray[GameDataManager.Instance.currentLevel].winIndex].GetComponent<SpriteRenderer>().color;
        Debug.Log("qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq: " + hexagonList[GameDataManager.Instance.rawData.levelsArray[GameDataManager.Instance.currentLevel].winIndex].GetComponent<SpriteRenderer>().color);
        hexagonList[GameDataManager.Instance.rawData.levelsArray[GameDataManager.Instance.currentLevel].winIndex].GetComponent<SpriteRenderer>().color = Color.black;
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
    
    private IEnumerator ScaleHexagon(GameObject hexagon, float targetScale, float duration, float posZ)
    {
        Vector3 initialScale = hexagon.transform.localScale;
        Vector3 targetLocalScale = Vector3.one * targetScale;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            hexagon.transform.localScale = Vector3.Lerp(initialScale, targetLocalScale, t);
            hexagon.transform.position = new Vector3(hexagon.transform.position.x, hexagon.transform.position.y, posZ);
            yield return null;
        }

        hexagon.transform.localScale = targetLocalScale;
    }

    private IEnumerator AnimateHexagons()
    {
        float delayBetweenHexagons = 0.02f;
        float scaleFactor = .3f;
        float animationDuration = 0.02f;
        float posZ = 0;
        
        foreach (GameObject hexagon in hexagonList)
        {
            posZ -= 5f;
            StartCoroutine(ScaleHexagon(hexagon, scaleFactor, animationDuration,posZ));
            yield return new WaitForSeconds(delayBetweenHexagons);
            StartCoroutine(ScaleHexagon(hexagon, hexagonSize, animationDuration,posZ));
        }
    }
}