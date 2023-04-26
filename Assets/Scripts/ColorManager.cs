using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    public GameObject sideUIButtonPrefab;
    public GameObject movingColorPrefab;
    public GameObject sideButtonParent;
    public GameObject[] sideButtonArray = new GameObject[5];
    public GameObject[] movingColorArray = new GameObject[5];
    public static ColorManager Instance;
    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        
        for(int i = 0;i< GameDataManager.Instance.rawData.levelsArray[GameDataManager.Instance.currentLevel].colorsInLevel.Count; i++)
        {
            string colorName = GameDataManager.Instance.rawData.levelsArray[GameDataManager.Instance.currentLevel].colorsInLevel[i].colorName;
            int colorAmount = GameDataManager.Instance.rawData.levelsArray[GameDataManager.Instance.currentLevel].colorsInLevel[i].amount;
            GameObject colorButton =  Instantiate(sideUIButtonPrefab,sideButtonParent.transform);
            colorButton.name = colorName;
            GameObject movingColor = Instantiate(movingColorPrefab);
            movingColor.name = "moving" + colorName;
            int colorIndex = -1;
            switch (colorName)
            {
                case "red":
                    colorIndex = 0;
                    colorButton.GetComponent<Image>().color = new Color(256/256f, 131/256f,131/256f);
                    break;
                case "green":
                    colorIndex = 1;
                    colorButton.GetComponent<Image>().color = new Color(110/256f, 218/256f,140/256f);
                    break;
                case "blue":
                    colorIndex = 2;
                    colorButton.GetComponent<Image>().color = new Color(131/256f, 131/256f,256/256f);
                    break;
            }
            Debug.Log(colorIndex);
            colorButton.transform.GetComponent<ColorButtonData>().numberText.text = colorAmount.ToString();
            movingColorArray[colorIndex] = movingColor;
            colorButton.GetComponent<ColorButtonData>().colorIndex= colorIndex;
            colorButton.GetComponent<ColorButtonData>().colorName = colorName;
            colorButton.GetComponent<ColorButtonData>().colorAmount =colorAmount;
            sideButtonArray[colorIndex] = colorButton;
        }
        HexagonMover.Instance.StartFunctions();
    }
    
}
