using UnityEngine;
using UnityEngine.UI;

public class StartScreenSize : MonoBehaviour
{
    public Canvas _Canvas;

    void Start()
    {
        int _width = Display.main.systemWidth;

        Debug.Log("Width: " + _width);

        if (_width > 1450)
        {
            _Canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        }
    }
}