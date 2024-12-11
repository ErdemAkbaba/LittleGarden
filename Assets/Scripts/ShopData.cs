using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop Data", menuName = "RedrubyFarm/Shop Data")]
public class ShopData : ScriptableObject
{    
    public List<ShopTab> tabs = new List<ShopTab>();
}

[Serializable]
public class ShopTab
{
    public Sprite buttonIcon;
    public String buttonName;
    public List<Card> cards = new List<Card>();
}
