using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static bool goStartPage = true;
    public static UIManager Instance;
    public GameObject startScreen;
    public GameObject inGameScreen;
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject levelSelectionScreen;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (goStartPage == true)
        {
            startScreen.SetActive(true);
        }
        else // load level directly
        {
            StartInGameLevelUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextLevel()
    {
        ColorWheelController.loadDeckDirectly = true;
        UIManager.goStartPage = false;
        GameDataManager.Instance.currentLevel++;
        SceneManager.LoadScene(0);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
    

    public void OnStartButtonClicked()
    {
        ColorWheelController.Instance.StartCreatingEnvironment(GameDataManager.Instance.currentLevel);
        PlayUISound();
        StartInGameLevelUI();
    }
    public void StartInGameLevelUI()
    {
        startScreen.SetActive(false);
        inGameScreen.SetActive(true);

    }

    public void OnLevelsButtonClicked()
    {
        levelSelectionScreen.SetActive(true);
        startScreen.SetActive(false);
        PlayUISound();
    }

    public void PlayUISound()
    {

    }
}
