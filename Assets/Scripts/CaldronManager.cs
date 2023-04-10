using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaldronManager : MonoBehaviour
{
    public static CaldronManager Instance;
    public SpriteRenderer spiral;
    public SpriteRenderer water;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    
}
