using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tool : MonoBehaviour
{
    public Card parentCard;
    public int Level=1;
    [TextArea] public string description;
    public virtual void Start()
    {
        GameManager.gameManager.nextDay.AddListener(Execute);   
    }

    public virtual void Execute()
    {
        
    }

    private void OnDestroy()
    {
        GameManager.gameManager.nextDay.RemoveListener(Execute);
    }
}
