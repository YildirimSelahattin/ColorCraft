using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class HandController : MonoBehaviour
{
    Vector3 originalScale;
    public int greenPushCount;
    public int redPushCount;
    // Start is called before the first frame update
    void Update()
    {
        originalScale = transform.localScale ;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary)
        {
            transform.DOScale(1.5f, 0.3f).OnComplete(() =>
            {
            
            });
        }
        else
        {
            transform.DOScale(2f, 0.3f).OnComplete(() =>
            {
            
            });
        }
        transform.DOMove(Input.GetTouch(0).position+Vector2.down*70+Vector2.right*10f,.5f).OnComplete((() =>
        {
           
        }));
    }

    private void OnMouseDown()
    {
        
    }

    private void OnMouseExit()
    {
        transform.DOScale(1, .3f);
    }
}
