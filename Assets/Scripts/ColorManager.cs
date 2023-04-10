using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    public Button redButton;
    public Button greenButton;
    public Button blueButton;

    public void OnClickRedButton()
    {
        CaldronManager.Instance.spiral.color = new Color(1, 51/255f, 51/255f,1);
    }
    
    public void OnClickGreenButton()
    {
        CaldronManager.Instance.spiral.color = new Color(51/255f, 1, 51/255f,1);
    }
    
    public void OnClickBlueButton()
    {
        CaldronManager.Instance.spiral.color = new Color(51/255f, 51/255f,1, 1);
    }
}
