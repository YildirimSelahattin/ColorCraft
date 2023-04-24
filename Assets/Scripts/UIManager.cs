using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static bool goStartPage = true;
    public static UIManager Instance;
    public GameObject startScreen;
    public GameObject inGameScreen;
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject levelSelectionScreen;
    public GameObject player;
    public GameObject optionBar;
    public GameObject startButton;
    public GameObject optionButton;
    public GameObject levelButton;
    public Image winTargetImage;

    public Button musicOn;
    public Button musicOff;
    public Button soundOn;
    public Button soundOff;

    int isSoundOn;
    int isMusicOn;
    int isVibrateOn;
    
    // Start is called before the first frame update
    void Start()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }

        player.GetComponent<SpriteRenderer>().enabled = false;
       
 
        if (goStartPage == true)
        {
            startScreen.SetActive(true);
        }
        else // load level directly
        {
            StartInGameLevelUI();
            player.GetComponent<SpriteRenderer>().enabled = true;
        }
        
        UpdateSound();
        UpdateMusic();
    }


    public void NextLevel()
    {
        ColorWheelController.loadDeckDirectly = true;
        UIManager.goStartPage = false;
        GameDataManager.Instance.currentLevel++;
        SceneManager.LoadScene(0);
        player.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
        player.GetComponent<SpriteRenderer>().enabled = true;
    }
    

    public void OnStartButtonClicked()
    {
        ColorWheelController.Instance.StartCreatingEnvironment(GameDataManager.Instance.currentLevel);
        StartInGameLevelUI();
        player.GetComponent<SpriteRenderer>().enabled = true;
        
    }

    public void HomeButton()
    {
        startScreen.SetActive(true);
        inGameScreen.SetActive(false);
        winScreen.SetActive(false);
        player.GetComponent<SpriteRenderer>().enabled = false;
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
    }

    public void UpdateSound()
    {
        isSoundOn = GameDataManager.Instance.playSound;
        if (isSoundOn == 0)
        {
            soundOff.gameObject.SetActive(true);
            SoundsOff();
        }

        if (isSoundOn == 1)
        {
            soundOn.gameObject.SetActive(true);
            SoundsOn();
        }
    }
    public void UpdateMusic()
    {
        isMusicOn = GameDataManager.Instance.playMusic;
        if (isMusicOn == 0)
        {
            musicOff.gameObject.SetActive(true);
            MusicOff();
        }

        if (isMusicOn == 1)
        {
            musicOn.gameObject.SetActive(true);
            MusicOn();
        }
    }

    public void MusicOff()
    {
        GameDataManager.Instance.playMusic = 0;
        musicOn.gameObject.SetActive(false);
        musicOff.gameObject.SetActive(true);
        //GameMusic.SetActive(false);
        PlayerPrefs.SetInt("PlayMusicKey", GameDataManager.Instance.playMusic);

        //UpdateMusic();
    }

    public void MusicOn()
    {
        GameDataManager.Instance.playMusic = 1;
        musicOff.gameObject.SetActive(false);
        musicOn.gameObject.SetActive(true);
        //GameMusic.SetActive(true);
        PlayerPrefs.SetInt("PlayMusicKey", GameDataManager.Instance.playMusic);

        //UpdateMusic();
    }

    public void SoundsOff()
    {
        GameDataManager.Instance.playSound = 0;
        soundOn.gameObject.SetActive(false);
        soundOff.gameObject.SetActive(true);
        PlayerPrefs.SetInt("PlaySoundKey", GameDataManager.Instance.playSound);

        //UpdateSound();
    }

    public void SoundsOn()
    {
        GameDataManager.Instance.playSound = 1;
        soundOff.gameObject.SetActive(false);
        soundOn.gameObject.SetActive(true);
        PlayerPrefs.SetInt("PlaySoundKey", GameDataManager.Instance.playSound);

        //UpdateSound();
    }

    public void VibratePhone()
    {
        Handheld.Vibrate();
    }

    public void OpenCloseOptionBar()
    {
        if (optionBar.active)
         {
             optionBar.SetActive(false);
             startButton.SetActive(true);
             optionButton.SetActive(true);
             levelButton.SetActive(true);
         }
         else
         {
             optionBar.SetActive(true);
             startButton.SetActive(false);
             optionButton.SetActive(false);
             levelButton.SetActive(false);
         }
    }
    
    public void OpenLevelPanel()
    {
        levelSelectionScreen.SetActive(true);
    }

    public void CloseLevelPanel()
    {
        levelSelectionScreen.SetActive(false);
        startScreen.SetActive(true);
    }
}
