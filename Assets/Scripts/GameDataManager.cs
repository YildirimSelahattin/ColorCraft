using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public DataList rawData;
    public static GameDataManager Instance;
    public int totalLevelNumber;
    public TextAsset JSONText;
    public int currentLevel = 0;
    public int highestLevel = 0;
    public AudioClip colorSelectSound;
    public AudioClip colorDeselectSound;
    public AudioClip winSound;
    public AudioClip blenderSound;
    public AudioClip uiClickSound;

    public int playSound;
    public int playMusic;
    public int playVibrate;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        LoadData();
    }

    public void LoadData()
    {
        rawData = JsonUtility.FromJson<DataList>(JSONText.text);
        totalLevelNumber = rawData.levelsArray.Length;
        playSound = PlayerPrefs.GetInt("PlaySound", 1);
        playMusic = PlayerPrefs.GetInt("PlayMusic", 1);
        playVibrate = PlayerPrefs.GetInt("PlayVibrate", 1);
        currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
        highestLevel = PlayerPrefs.GetInt("highestLevel", 1);
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("PlaySound", playSound);
        PlayerPrefs.SetInt("PlayMusic", playMusic);
        PlayerPrefs.SetInt("PlayVibrate", playVibrate);
        if(currentLevel > highestLevel)
        {
            PlayerPrefs.SetInt("highestLevel", highestLevel);
            PlayerPrefs.SetInt("currentLevel", currentLevel);
        }
    }
}
