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
    public int levelToLoad = 0;
    public AudioClip breakSound;
    public AudioClip crackSound;
    public AudioClip winSound;
    public AudioClip bossDamageSound;
    public AudioClip collectSound;

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
        levelToLoad = PlayerPrefs.GetInt("levelToLoad", 1);
        currentLevel = levelToLoad;
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("PlaySound", playSound);
        PlayerPrefs.SetInt("PlayMusic", playMusic);
        PlayerPrefs.SetInt("PlayVibrate", playVibrate);
        PlayerPrefs.SetInt("levelToLoad", levelToLoad);
    }
}
