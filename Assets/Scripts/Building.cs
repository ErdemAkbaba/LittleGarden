using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class Building : MonoBehaviour
{
    public string requestCardID;
    public int maxAnimalCount=0;
    public int currentAnimalCount;
    public Card rewardCard;
    public Card parentCard;
    public string description;
    
    public virtual void AddCard()
    {
        
    }
}
