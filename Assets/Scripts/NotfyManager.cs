using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class NotfyManager : MonoBehaviour
{
    public static NotfyManager notfyManager;
    public TextMeshProUGUI notfyText;
    public GameObject notfyPanel;
    public float showDuration;
    private RectTransform rectTransform;
    public float animationDuration;

    private void Awake()
    {
        notfyManager = this;
        rectTransform = GetComponent<RectTransform>();
    }

    public void TriggerNotfy(string notfyDescription)
    {
        StartCoroutine(ShowHideNotfy(notfyDescription));
    }
    
    public IEnumerator ShowHideNotfy(string notfyDescription)
    {
        notfyText.text = notfyDescription;
        rectTransform.DOMoveY(rectTransform.position.y - 5,0.5f);
        yield return new WaitForSeconds(animationDuration);
        rectTransform.DOMoveY(rectTransform.position.y - 5,0.5f);
        
        
    }
}
