using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TutorialHandManager : MonoBehaviour
{
    Vector3 originalScale;
    public int greenPushCount;
    public int redPushCount;
    // Start is called before the first frame update
    void Start()
    {
        originalScale =transform.localScale;
    }

    private void OnEnable()
    {
        if(GameDataManager.Instance.currentLevel == 1)
        {
            MoveToButtons(0);
        }

        if (GameDataManager.Instance.currentLevel == 2)
        {
            MoveToButtons(1);
        }
        if (GameDataManager.Instance.currentLevel == 3)
        {
            MoveToButtons(0);
        }
        if (GameDataManager.Instance.currentLevel == 4)
        {
            MoveToButtons(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PressLoop()
    {

        transform.DOScale(originalScale * 0.7f, 0.3f).OnComplete(() =>
        {

            transform.DOScale(originalScale, 0.3f).OnComplete(() =>
            {
                PressLoop();
            });
        });
    }

    public void MoveToButtons(int buttonIndex)
    {
        Vector3 targetPosition = ColorManager.Instance.sideButtonArray[buttonIndex].transform.position;
        targetPosition = new Vector3(targetPosition.x, targetPosition.y - 200,targetPosition.z);
        transform.DOMove(targetPosition, 0.5f).OnComplete(() =>
        {
            PressLoop();
        });
    }

    public void MoveToBlenderButton()
    {
        Vector3 targetPosition = UIManager.Instance.blendColorsButton.transform.position;
        targetPosition = new Vector3(targetPosition.x, targetPosition.y - 200, targetPosition.z);
        transform.DOMove(targetPosition, 0.5f).OnComplete(() =>
        {
            transform.DOKill();
            Hold();
        });
    }

    public void Hold()
    {
        transform.DOScale(originalScale * 0.7f, 0.3f);
    }
    public void Release()
    {
        transform.DOScale(originalScale, 0.3f);
    }
    public void Level2Loop()
    {
    }
}
