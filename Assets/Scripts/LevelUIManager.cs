using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    public GameObject gridPrefab;
    public List<GameObject> gridList;
    public GameObject levelButtonPrefab;
    public GameObject gridParent;
    public float gridWidth;
    private Color32[] colors = {
        new Color32 (255, 131, 131, 255),
        new Color32 (164, 148, 255, 255),
        new Color32 (131, 180, 255, 255),
        new Color32 (110, 218, 140, 255),
        new Color32 (255, 183, 117, 255)};
    
    // Start is called before the first frame update
    void Start()
    {
        CreateLevelPanels();
    }
    
    public void CreateLevelPanels()
    {
        int gridCounter = 0;
        int howManyToAdd = 0;
        int temp = GameDataManager.Instance.totalLevelNumber;
        while (temp != 0)
        {
            if (temp >= 15)
            {
                temp -= 15;
                howManyToAdd = 15;
            }
            else
            {
                howManyToAdd = temp;
                temp = 0;
            }
            GameObject grid = Instantiate(gridPrefab, gridParent.transform);
            grid.transform.localPosition = new Vector3(0, -gridWidth * gridCounter, 0);
            gridList.Add(grid);
            gridCounter++;
            for (int i = 1; i < howManyToAdd; i++)
            {
                int index = (gridCounter - 1) * 15 + i;
                GameObject levelButton = Instantiate(levelButtonPrefab, grid.transform);
                LevelButtonManager buttonScript = levelButton.GetComponent<LevelButtonManager>();
                buttonScript.levelIndex = index;
                if(index < 10)
                {
                    buttonScript.levelNumberText.text = "0"+index.ToString();
                }
                else
                {
                    buttonScript.levelNumberText.text = index.ToString();
                }
                
                buttonScript.levelNumberText.color = colors[i%5];

                if (index > GameDataManager.Instance.highestLevel)
                {
                    levelButton.GetComponent<Button>().interactable = false;
                }
                else
                {
                    levelButton.GetComponent<Button>().interactable = true;
                }
            }
        }
    }
}
