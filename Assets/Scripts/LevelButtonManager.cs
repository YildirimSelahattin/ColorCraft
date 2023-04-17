using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelButtonManager : MonoBehaviour
{
    public int levelIndex;
    public TextMeshProUGUI levelNumberText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnButtonClicked()
    {
        UIManager.Instance.PlayUISound();
        GameDataManager.Instance.currentLevel = levelIndex;
        ColorWheelController.Instance.StartCreatingEnvironment(levelIndex);
        UIManager.Instance.levelSelectionScreen.SetActive(false);
        UIManager.Instance.inGameScreen.SetActive(true);
    }
}
