using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReqcipeCard
{
    public int count;
    public Card requestCard;
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "RedrubyFarm/Recipe")]
public class Recipe : ScriptableObject
{
   public int recipeId; 
   public string recipeName;
   public Sprite recipeIcon;
   [TextArea]
   public string recipeDescription;
   public List<ReqcipeCard> requestCards;
   public List<Card> resultCardData;
    public int unlockLevel;
    public int exp;
}
