using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    public Button redButton;
    public Button greenButton;
    public Button blueButton;
    public GameObject movingR;
    public GameObject movingG;
    public GameObject movingB;
    public void OnClickRedButton()
    {
        Colorize(new Color(1, 51 / 255f, 51 / 255f, 1));
    }
    
    public void OnClickGreenButton()
    {
        Colorize(new Color(51 / 255f, 1, 51 / 255f, 1));
    }
    
    public void OnClickBlueButton()
    {
        Colorize(new Color(51 / 255f, 51 / 255f, 1, 1));
    }

/*    public void MoveToEmptySpot(GameObject movingColor,Button button)
    {
        if(CaldronManager.Instance.lastHexagonPos != -1)
        {
            GameObject temp =  Instantiate(movingColor, button.gameObject.transform);
            temp.transform.DOMove(CaldronManager.Instance.hexagonSpotsList[CaldronManager.Instance.lastHexagonPos].transform.position, 1f).OnComplete(() =>
            {
                CaldronManager.Instance.hexagonSpotsList[CaldronManager.Instance.lastHexagonPos].GetComponent<Image>().sprite = temp.GetComponent<Image>().sprite;
                Destroy(temp.gameObject);
            });
            CaldronManager.Instance.lastHexagonPos++;

            if (CaldronManager.Instance.lastHexagonPos > 2)
            {
                CaldronManager.Instance.lastHexagonPos = -1;
            }
        }
    }*/
    public void Colorize(Color color)
    {
        CaldronManager.Instance.spiral.color = color;
    }

}
