using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Localization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StatusIcon
{
    public int targetNegativeStatusCount;
    public Sprite statusIcon;
}

public class Hover : MonoBehaviour
{
    public int negativeStatusCount;
    public List<StatusIcon> statusIcons = new List<StatusIcon>();

    public Sprite badStatus, mehStatus, goodStatus;
    public Color badStatusColor, mehStatusColor, goodStatusColor;
    public GameObject hoverMassage;
    public Transform hoverMassageGrid;
    public Image statusImage;
    public Image icon;
    public TextMeshProUGUI objectName;
    public TextMeshProUGUI objectLevel;
    public TextMeshProUGUI cooldown;

    [Header("Reputation")]
    public Image reputationLevelImage;
    public TextMeshProUGUI header;
    public TextMeshProUGUI description;
    public bool isReputationPanel;

    private GameObject _farmObject;
    private Plant currentPlant = null;

    void Update()
    {        
        Vector2 mousePos = GameManager.gameManager.inputActions.Player.MousePos.ReadValue<Vector2>();
        transform.DOMove(new Vector3(mousePos.x + 40f, mousePos.y, 0), 0.2f);

        if (!isReputationPanel)
        {
            if(hoverMassageGrid.childCount > 0)
            {
                if (!hoverMassageGrid.gameObject.activeSelf)
                    hoverMassageGrid.gameObject.SetActive(true);
            }
            else
            {
                if (hoverMassageGrid.gameObject.activeSelf)
                    hoverMassageGrid.gameObject.SetActive(false);
            }
        }

        if (_farmObject != null && _farmObject.GetComponent<Plant>())
        {            
            currentPlant = _farmObject.GetComponent<Plant>();
            cooldown.text = FormatTime(currentPlant.growthCooldown);
        }
    }

    public void AddStatus(string massage, bool isNegative)
    {
        GameObject newHoverMassage = Instantiate(hoverMassage, hoverMassageGrid.position,
            hoverMassage.transform.rotation, hoverMassageGrid);

        newHoverMassage.GetComponent<TextMeshProUGUI>().text = massage;
        
        if (isNegative)
        {
            newHoverMassage.GetComponent<TextMeshProUGUI>().color = Color.red;
            negativeStatusCount++;
        }

        if (negativeStatusCount == 0)
        {
            statusImage.sprite = goodStatus;
            statusImage.color = goodStatusColor;
        }
        else if(negativeStatusCount < 3)
        {
            statusImage.sprite = mehStatus;
            statusImage.color = mehStatusColor;
        }
        else
        {
            statusImage.sprite = badStatus;
            statusImage.color = badStatusColor;
        }
    }

    public void FillNewData(Sprite _objectIcon, string _objectName, string? _level = null, GameObject farmObject = null)
    {
        ClearStatus();

        if (farmObject != null)
        {
            _farmObject = farmObject;
        }

        if (_level == null)
        {
            objectLevel.gameObject.SetActive(false);
        }
        else
        {
            objectLevel.gameObject.SetActive(true);

            LeanLocalization.SetToken("HOVER_LEVEL", _level);
            objectLevel.text = LeanLocalization.GetTranslationText("HoverLvl");
        }
            
        icon.sprite = _objectIcon;
        objectName.text = LeanLocalization.GetTranslationText(_objectName);
    }

    public void ClearStatus()
    {
        negativeStatusCount = 0;
        for (int i = 0; i < hoverMassageGrid.childCount; i++)
        {
            Destroy(hoverMassageGrid.GetChild(i).gameObject);
        }
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void FillData(String headerText, String descText)
    {
        header.text = headerText;
        description.text = descText;
    }
}

