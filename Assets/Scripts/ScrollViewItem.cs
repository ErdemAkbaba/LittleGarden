using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Lean.Gui;

public class ScrollViewItem : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {

        if (GetComponent<CardUI>().card.unlockLevel > GameManager.gameManager.farmLevel)
            return;

        if (UIManager.uiManager.hands[FloorController.floorController.currentFloorIndex].handTransform.childCount > 9)
        {
            UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("NotEnoughSpace"));
            return;
        }

        if (!GameManager.gameManager.Buy(GetComponent<CardUI>().card.price))
        {
            UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("NotEnoughGold"));
            return;
        }

        Vector3 upPosition = new Vector3(Random.RandomRange(2f,-2f),50f,0f);
        Vector3 downPosition = new Vector3(Random.RandomRange(2f,-2f),10f,0f);
        GameObject currentBuyCard = Shop.shop.CardCreate(GetComponent<CardUI>().card, transform);

        currentBuyCard.transform.parent = UIManager.uiManager.mainCanvas.transform;
        currentBuyCard.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.3f);
        currentBuyCard.transform.DOLocalMoveY(currentBuyCard.transform.localPosition.y + 500f, 0.3f).OnComplete(delegate
        {
            currentBuyCard.transform.SetSiblingIndex(0);

            currentBuyCard.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f);
            currentBuyCard.transform.DOLocalMoveY(currentBuyCard.transform.localPosition.y - 500f, 0.2f).OnComplete ( delegate 
            {
                Destroy(currentBuyCard.gameObject);
            });
        });

        GameManager.gameManager.SpawnCardByCard(currentBuyCard.GetComponent<CardUI>().card, true);
    }
}
