using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUIManager : MonoBehaviour
{
    public GameObject gridPrefab;
    public List<GameObject> gridList;
    public GameObject levelButtonPrefab;
    public GameObject gridParent;
    public float gridWidth;
    // Start is called before the first frame update
    void Start()
    {
        CreateLevelPanels();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateLevelPanels()
    {
        int gridCounter = 0;
        int howManyToAdd = 0;
        int temp = GameDataManager.Instance.totalLevelNumber;
        while (temp != 0)
        {
            if (temp >= 9)
            {
                temp -= 9;
                howManyToAdd = 9;
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
            for (int i = 0; i < howManyToAdd; i++)
            {
                int index = (gridCounter - 1) * 9 + i + 1;
                GameObject levelButton = Instantiate(levelButtonPrefab, grid.transform);
                LevelButtonManager buttonScript = levelButton.GetComponent<LevelButtonManager>();
                buttonScript.levelIndex = index;
                buttonScript.levelNumberText.text = index.ToString();

                if (index > GameDataManager.Instance.currentLevel)
                {
                    
                }
                else if (index == GameDataManager.Instance.currentLevel)
                {
                  
                }
                else if (index < GameDataManager.Instance.currentLevel)
                {
                    
                }
            }
        }
    }
}
