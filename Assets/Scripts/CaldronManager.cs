using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaldronManager : MonoBehaviour
{
    public static CaldronManager Instance;
    public Image spiral;
    public Image water;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
