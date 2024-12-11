using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolCard : CardUIParent
{
    public override void PlaceCard()
    {
        base.PlaceCard();
        if (GameManager.gameManager.currentTilePart != null)
        {
            Instantiate(card.spawnObject, GameManager.gameManager.currentTilePart.plantPos.position,
                card.spawnObject.transform.rotation,
                GameManager.gameManager.currentTilePart.transform);
            
            GameManager.gameManager.currentTilePart.isPlanted = true;
        }
    }
}
