using Lean.Gui;
using Lean.Localization;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Menu menu;

    public List<Recipe> recipes = new List<Recipe>();
    public GameObject recipeUI;
    public GameObject mealReq;
    public Transform parent;
    public Transform recipeHolder;
    public LeanWindow recipePanel;

    private void Awake()
    {
        menu = this;
    }

    private void Start()
    {
        FillMenu();
    }

    public void FillMenu()
    {
        for (int p = 0; p < parent.childCount; p++)
        {
            Destroy(parent.GetChild(p).gameObject);
        }

        for (int i = 0; i < recipes.Count; i++)
        {
            GameObject clone = Instantiate(recipeUI, Vector3.zero, Quaternion.identity, parent);
            clone.transform.GetChild(0).Find("MealTitle").GetComponent<TextMeshProUGUI>().text = LeanLocalization.GetTranslationText(recipes[i].recipeName);
            clone.transform.GetChild(0).transform.GetChild(0).Find("MealIcon").GetComponent<Image>().sprite = recipes[i].recipeIcon;
            clone.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = (Restaurant.restourant.meals.Find(p => p.recipe == recipes[i]) == null ? 0 : Restaurant.restourant.meals.Find(p => p.recipe == recipes[i]).count).ToString();
            clone.GetComponent<MenuItem>().mealCount = Restaurant.restourant.meals.Find(p => p.recipe == recipes[i]) == null ? 0 : Restaurant.restourant.meals.Find(p => p.recipe == recipes[i]).count;
            clone.GetComponent<MenuItem>().recipe = recipes[i];

            for (int a = 0; a < recipes[i].requestCards.Count; a++)
            {
                GameObject cloneReq = Instantiate(mealReq, Vector3.zero, Quaternion.identity, clone.transform.Find("MealReq"));
                cloneReq.transform.Find("MealReqIcon").GetComponent<Image>().sprite = recipes[i].requestCards[a].requestCard.icon;
                cloneReq.GetComponentInChildren<TextMeshProUGUI>().text = recipes[i].requestCards[a].count.ToString();
                clone.GetComponent<MenuItem>().mealReqs.
                    Add(new MenuReqItem(recipes[i].requestCards[a].requestCard, recipes[i].requestCards[a].requestCard.icon, cloneReq.GetComponentInChildren<TextMeshProUGUI>()));
            }
        }
    }
}
