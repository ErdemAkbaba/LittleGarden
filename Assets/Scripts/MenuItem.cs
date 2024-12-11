using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[SerializeField]
public class MenuReqItem
{
    public Card card;
    public Sprite icon;
    public TextMeshProUGUI reqText;

    public MenuReqItem(Card _card, Sprite _icon, TextMeshProUGUI _reqText)
    {
        card = _card;
        icon = _icon;
        reqText = _reqText;
    }
}

public class MenuItem : MonoBehaviour
{
    public Recipe recipe;
    public Button removeArrow, addArrow;
    public int mealCount;
    public TextMeshProUGUI count;    
    public List<MenuReqItem> mealReqs = new List<MenuReqItem>();
    bool makeable;

    private void Start()
    {
        makeable = true;
        Check();
        Silo.silo.stockChange.AddListener(Check);
    }

    public void Add()
    {
        if (makeable)
        {
            mealCount++;

            for (int i = 0; i < recipe.requestCards.Count; i++)
            {
                Silo.silo.RemoveSiloStock(recipe.requestCards[i].count, recipe.requestCards[i].requestCard);
            }
        }

        count.text = mealCount.ToString();

        Meal meal = Restaurant.restourant.meals.Find(p => p.recipe == recipe);

        if (meal == null)
        {
            Meal mealClone = new Meal(recipe, 1);
            Restaurant.restourant.meals.Add(mealClone);
            Silo.silo.AddSiloStock(1, mealClone.recipe.resultCardData[0]);
        }
        else
        {
            meal.Count++;
            Silo.silo.AddSiloStock(1, meal.recipe.resultCardData[0]);
        }

        Check();
    }

    public void Remove()
    {        
        mealCount--;

        for (int i = 0; i < recipe.requestCards.Count; i++)
        {
            Silo.silo.AddSiloStock(recipe.requestCards[i].count, recipe.requestCards[i].requestCard);
        }

        count.text = mealCount.ToString();
        Check();

        Meal meal = Restaurant.restourant.meals.Find(p => p.recipe == recipe);
        if (meal != null)
        {
            meal.Count--;

            Silo.silo.AddSiloStock(-1, meal.recipe.resultCardData[0]);

            if (meal.Count <= 0)
                Restaurant.restourant.meals.Remove(meal);
        }
    }

    public void Check()
    {
        makeable = true;
        SiloItem siloItem;

        for (int i = 0; i < recipe.requestCards.Count; i++)
        {
            siloItem = Silo.silo.stock.Find(p => p.plant == recipe.requestCards[i].requestCard);

            if (siloItem == null || (siloItem != null && siloItem.count < recipe.requestCards[i].count))
            {
                makeable = false;
                mealReqs.Find(p => p.card == recipe.requestCards[i].requestCard).reqText.color = Color.red;
            }
            else
            {
                mealReqs.Find(p => p.card == recipe.requestCards[i].requestCard).reqText.color = Color.white;
            }
        }

        if (makeable)
        {
            addArrow.interactable = true;
        }
        else
        {
            addArrow.interactable = false;
        }

        if (mealCount == 0)
        {
            removeArrow.interactable = false;
        }
        else
        {
            removeArrow.interactable = true;
        }
    }
}
