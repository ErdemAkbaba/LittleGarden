using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Recipepanel : MonoBehaviour
{
    public static Recipepanel recipepanel;

    public List<Recipe> recipes = new List<Recipe>();
    public GameObject recipeUI;
    public GameObject mealReq;
    public Transform parent;

    private void Start()
    {
        FillRecipe();
    }

    public void FillRecipe()
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }

        for (int i = 0; i < recipes.Count; i++)
        {
            GameObject clone = Instantiate(recipeUI, Vector3.zero, Quaternion.identity, parent);
            clone.transform.GetChild(0).Find("MealTitle").GetComponent<TextMeshProUGUI>().text = LeanLocalization.GetTranslationText(recipes[i].recipeName);
            clone.transform.GetChild(0).transform.GetChild(0).Find("MealIcon").GetComponent<Image>().sprite = recipes[i].recipeIcon;

            for (int a = 0; a < recipes[i].requestCards.Count; a++)
            {
                GameObject cloneReq = Instantiate(mealReq, Vector3.zero, Quaternion.identity, clone.transform.Find("MealReq"));
                cloneReq.transform.Find("MealReqIcon").GetComponent<Image>().sprite = recipes[i].requestCards[a].requestCard.icon;
                cloneReq.GetComponentInChildren<TextMeshProUGUI>().text = recipes[i].requestCards[a].count.ToString();
            }
        }        
    }
}
