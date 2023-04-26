using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButtonManager : MonoBehaviour
{
    public int levelIndex;
    public TextMeshProUGUI levelNumberText;

    public void OnButtonClicked()
    {
        GameDataManager.Instance.currentLevel = levelIndex;
        ColorWheelController.Instance.StartCreatingEnvironment(levelIndex);
        UIManager.Instance.levelSelectionScreen.SetActive(false);
        UIManager.Instance.inGameScreen.SetActive(true);
        UIManager.Instance.player.GetComponent<SpriteRenderer>().enabled = true;
    }
}
