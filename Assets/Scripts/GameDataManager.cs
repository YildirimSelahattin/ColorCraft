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
        }
        LoadData();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void LoadData()
    {
        rawData = JsonUtility.FromJson<DataList>(JSONText.text);
        totalLevelNumber = rawData.levelsArray.Length;
    }
}
