using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Gui;
using Unity.VisualScripting;
using UnityEngine;

public class IslandExpand : MonoBehaviour
{
    private Vector3 normalScale;
    public GameObject island;
    public int price;
    private GameObject mesh;
    private void Start()
    {
        mesh = gameObject.transform.GetChild(0).gameObject;
        price = island.GetComponent<Island>().price;
        normalScale = transform.localScale;

        GameManager.gameManager.unlockedIsland.AddListener(delegate
        {
            if (island.GetComponent<Island>().IsActive == true)
            {
                Destroy(gameObject);
            }
        });
    }
        

    private void OnMouseEnter()
    {
        transform.DOScale(new Vector3(normalScale.x+0.1f,normalScale.y+0.1f,normalScale.z+0.1f),0.2f);
    }

    private void OnMouseExit()
    {
        transform.DOScale(normalScale,0.2f);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.gameManager.activeCard == null)
        {
            ClickAnimation();
        }
    }

    private void ClickAnimation()
    {
        if(GameManager.gameManager.isActive==false)
            return;
        transform.DOScale(new Vector3(normalScale.x+0.2f,normalScale.y+0.2f,normalScale.z+0.2f),0.1f).OnComplete(
            delegate
            {
                transform.DOScale(new Vector3(normalScale.x + 0.1f, normalScale.y + 0.1f, normalScale.z + 0.1f), 0.2f);
                UIManager.uiManager.unlockIsland = island;
                UIManager.uiManager.cloneArrow = this.gameObject;
                UIManager.uiManager.islandNotfyPanel.GetComponent<LeanWindow>().TurnOn();
                UIManager.uiManager.SetUnlockIslandText();
                GameManager.gameManager.isActive = false;
            });
    }
    
    private void Update()
    {
        mesh.SetActive(FloorController.floorController != null && FloorController.floorController.currentFloorIndex == 0);
    }
}
