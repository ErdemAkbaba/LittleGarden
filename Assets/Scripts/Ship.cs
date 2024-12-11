using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SerializeField]
public class RequiredCards
{
    public Card card;
    public int count;
}

public class Ship : MonoBehaviour
{    
    public List<RequiredCards> requiredCardList = new List<RequiredCards>();

    private void Start()
    {
        RequestPlant();
    }

    void RequestPlant()
    {
        List<Card> avaliableCards = new List<Card>();
        avaliableCards.AddRange(GameManager.gameManager.AllCards.Where(p => p.unlockLevel <= GameManager.gameManager.farmLevel));

    }
}
