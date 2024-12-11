using Lean.Gui;
using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RestaurantCustomize : MonoBehaviour
{
    public List<Sprite> logos = new List<Sprite>();
    public Image restaurantImage;
    public Button rightButton;
    public Button leftButton;
    public LeanButton editButton;
    public TMP_InputField restaurantName;

    private bool isEdit = true;

    int currentIndex = 0;

    private void Start()
    {
        if (logos != null)
        {
            restaurantImage.sprite = logos[0];
        }
    }

    public void RightButton()
    {
        currentIndex++;
        if (currentIndex == logos.Count)
        {
            currentIndex = 0;
        }
        restaurantImage.sprite = logos[currentIndex];
    }

    public void LeftButton()
    {
        if (currentIndex < 0)
        {
            currentIndex = logos.Count - 1;
        }
        restaurantImage.sprite = logos[currentIndex];
        Debug.Log(currentIndex);
        currentIndex--;
    }

    public void HoverInfoEnter()
    {
        
        Hover reputationInfo = UIManager.uiManager.reputationHoverPanel.GetComponent<Hover>();
        reputationInfo.gameObject.SetActive(true);
        LeanLocalization.SetToken("RAPUTATION_LEVEL", ((int)(Restaurant.restourant.reputationSlider.value * 10)).ToString());
        reputationInfo.FillData(LeanLocalization.GetTranslationText("Raputation"), LeanLocalization.GetTranslationText("YourRaputationLevel"));
    }

    public void HoverInfoExit()
    {
       
        Hover reputationInfo = UIManager.uiManager.reputationHoverPanel.GetComponent<Hover>();
        reputationInfo.gameObject.SetActive(false);
        reputationInfo.FillData("", "");
    }

    public void Edit()
    {
        if (isEdit)
        {
            rightButton.interactable = true;
            leftButton.interactable = true;
            restaurantName.interactable = true;
            editButton.transform.Find("Cap").Find("Pen").gameObject.SetActive(false);
            editButton.transform.Find("Cap").Find("Done").gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(restaurantName.gameObject);
            restaurantName.placeholder.gameObject.SetActive(false);
            RectTransform rt = editButton.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(120, 90);
            //editButton.transform.Find("Cap").Find("Text").GetComponent<TextMeshProUGUI>().text = LeanLocalization.GetTranslationText("Done");
            isEdit = false;
        }
        else
        {
            rightButton.interactable = false;
            leftButton.interactable = false;
            restaurantName.interactable = false;
            editButton.transform.Find("Cap").Find("Pen").gameObject.SetActive(true);
            editButton.transform.Find("Cap").Find("Done").gameObject.SetActive(false);
            restaurantName.placeholder.gameObject.SetActive(true);
            RectTransform rt = editButton.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(90, 90);
            isEdit = true;
        }
    }
}
