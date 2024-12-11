using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Gui;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class Hand
{
    public Transform handTransform;
    public string handName;
}
public class UIManager : MonoBehaviour, IDataPersistance
{
    private bool tutorialPanelShowed;
    public GameObject activePanel;
    public static UIManager uiManager;
    public GameObject uiCamera;
    public LeanPulse notfyPanel;
    public PostProcessVolume postVolume;
    public Image transitionImage;

    public GameObject inGameUI;
    public GameObject mainCanvas;
    public GameObject cookingPanel;
    public GameObject hand;
    public GameObject popupCanvas;
    public GameObject hoverPanel;
    public GameObject reputationHoverPanel;
    public GameObject tutorialPanel;
    public GameObject goldAudio;
    public GameObject recipeParent;
    public GameObject islandNotfyPanel;
    public GameObject GuidePanel;
    public LeanWindow RentPanel, newGamePanel, returnMenuPanel;
    public Text RentPanelText;
    public TextMeshProUGUI rentDayDistanceText;
    [HideInInspector] public GameObject unlockIsland;
    [HideInInspector] public GameObject cloneArrow;

    public Text unlockIslandPriceText;
    public TextMeshProUGUI dayCounter;
    public List<TextMeshProUGUI>  goldText;
    public TextMeshProUGUI hoverText;
    public TextMeshProUGUI cardCountText;    
    
    [FormerlySerializedAs("levelUpPanel")] public GameObject unlockPanel;
    public Button hoe, wateringCan,trowel;
    public GameObject comboLevelUp;
    public GameObject popUpStatic;
    public GameObject goldChangeText;

    [Space(20)]
    [Header("Shop")]
    public GameObject shopPanel;
    public GameObject shopButton;
    public Transform shopButtonImage;

    [Space(20)]
    [Header("Hand")]
    public List<Hand> hands = new List<Hand>();

    [Space(20)]
    [Header("Quest")]
    public GameObject questPrefab;
    public GameObject questParent;
    public List<Quest> allActiveQuest = new List<Quest>();

    [Space(20)] [Header("Experiance")]
    public GameObject expSpriteRenderer;
    public Slider farmLevelSlider;
    public TextMeshProUGUI currentFarmLevelText;
    public TextMeshProUGUI nextFarmLevelText;
    public TextMeshProUGUI farmLevelSliderText;

    [Space(20)]
    [Header("Main Menu")]
    public Button playButton;
    public GameObject mainMenuPanel;
    [SerializeField] public GameObject mainMenuCamera,inGameCamera;

    [Space(20)]
    [Header("Featuring Buttons")]
    [SerializeField] public GameObject dvButton, discordButton;

    [Space(20)]
    [Header("Restaurant")]
    public LeanWindow restaurantWindow;
    public LeanWindow restaurantMenu;
    public LeanWindow restaurantCustomizedWindow;
    public GameObject restaurantPanel;
    public GameObject openRestourantButton;
    public GameObject timeControlPanel;

    [Space(20)]
    [Header("Credits")]
    public LeanWindow creditsPanel;
    public LeanWindow graphicSettingsPanel;

    public GameObject creditsBtn;
    public GameObject backBtn;
    public GameObject settingsDoneBtn;


    public List<GameObject> closeOnMenu = new List<GameObject>();

    public GameObject floorPanel;

    [Space(10)]
    [Header("Localization")]
    public Text returnMainMenu;
    public Text returnMainMenuFromIsland;
    public Text newGameQuestion;
    public Text islandExpand;
    public Text goToRestaurant;

    private void Awake()
    {
        uiManager = this;             
    }

    private void Start()
    {
        GameManager.gameManager.inputActions.Player.Escape.performed += EscapeUI();
        GameManager.gameManager.handCardsChange.AddListener(ChangeCardCountText);
        GameManager.gameManager.nextDay.AddListener(delegate
        {
            if ((GameManager.gameManager.currentRentDayDistance-1) <=0)
            {
                rentDayDistanceText.text = LeanLocalization.GetTranslationText("Payday") + "!";
            }
            else
            {
                LeanLocalization.SetToken("PAYDAY_COUNT", (GameManager.gameManager.currentRentDayDistance - 1).ToString());               
                rentDayDistanceText.text = LeanLocalization.GetTranslationText("PaydayCountText");
            }
        });

        EventSystem.current.SetSelectedGameObject(playButton.gameObject);

        /*langDropdown.onValueChanged.AddListener(delegate {
            ChangeActiveLang(langDropdown);
        });
        
        langDropdown.value = PlayerPrefs.GetInt("lang", 0);*/
        foreach (TextMeshProUGUI text in goldText)
        {
            text.text = GameManager.gameManager.gold.ToString();
        }

        restaurantPanel.GetComponent<Image>().enabled = false;
        ChangeLocal();
    }

    public void ChangeLocal()
    {
        returnMainMenu.text = "<size=60>" + LeanLocalization.GetTranslationText("Warning") + "</size>\r\n\r\n" + LeanLocalization.GetTranslationText("QuestionReturn") + " <color=#EE2616>" + LeanLocalization.GetTranslationText("MainMenu") + "</color>?";
        returnMainMenuFromIsland.text = "<size=60>" + LeanLocalization.GetTranslationText("Warning") + "</size>\r\n\r\n" + LeanLocalization.GetTranslationText("QuestionReturn") + " <color=#EE2616>" + LeanLocalization.GetTranslationText("MainMenu") + "</color>?";
        newGameQuestion.text = "<size=60>" + LeanLocalization.GetTranslationText("Warning") + "</size>\r\n\r\n" + LeanLocalization.GetTranslationText("DoYouReallyWantToStart ") + " <color=#EE2616>" + LeanLocalization.GetTranslationText("NewGame") + "</color>?";
        islandExpand.text = "<size=60>" + LeanLocalization.GetTranslationText("Warning") + "</size>\r\n\r\n" + LeanLocalization.GetTranslationText("DoYouReallyWantToExpandYour") + " <color=#EE2616>" + LeanLocalization.GetTranslationText("Island") + "</color>?";
        goToRestaurant.text = "<size=60>" + LeanLocalization.GetTranslationText("Warning") + "</size>\r\n\r\n" + LeanLocalization.GetTranslationText("Question") + " <color=#EE2616>" + LeanLocalization.GetTranslationText("Restaurant") + "</color>?";
    }

    private Action<InputAction.CallbackContext> EscapeUI()
    {
        return (c) =>
        {
            if (activePanel == null)
            {
                if (CursorManager.cursorManager.index != 0)
                {
                    GameManager.gameManager.mouseClicked.RemoveAllListeners();
                    CursorManager.cursorManager.index = 0;
                    CursorManager.cursorManager.ChangeCursor();
                }

                else if (GameManager.gameManager.isActive && restaurantMenu.On == false)
                {
                    OpenLeanWindow(returnMenuPanel);
                }
            }

            else
            {
                if(activePanel == shopPanel)
                {
                    OpenShop();
                }
                else
                {
                    if (activePanel.GetComponent<LeanWindow>())
                        activePanel.GetComponent<LeanWindow>().TurnOff();

                    else
                        activePanel.SetActive(false);

                    activePanel = null;
                }                
            }
        };
    }

    public void TriggerTransition()
    {
        transitionImage.GetComponent<Animator>().SetTrigger("Transition");
    }

    public void spawnQuest(QuestData questDataParam)
    {
        Quest cloneQuest = Instantiate(questPrefab, questParent.transform.position, questParent.transform.rotation,questParent.transform)
            .GetComponent<Quest>();
        cloneQuest.questData = questDataParam;
        cloneQuest.FillData();
        allActiveQuest.Add(cloneQuest);

    }

    public void ShowNotfy(string massage)
    {
        notfyPanel.GetComponent<LeanPulse>().RemainingTime = 1;
        notfyPanel.RemainingPulses = 1;
        notfyPanel.gameObject.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = massage;
        notfyPanel.Pulse();
    }

    public void ShowNotfy(string massage, bool ignoreReminingTime)
    {
        if (!ignoreReminingTime)
        {
            notfyPanel.GetComponent<LeanPulse>().RemainingTime = 1;
        }
        notfyPanel.RemainingPulses = 1;
        notfyPanel.gameObject.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = massage;
        notfyPanel.Pulse();
    }

    /*public void CalculateSpacingHand()
    {
        float newSpacing = -7 * hand.transform.childCount;
        
        if (newSpacing <= -150f)
        {
            newSpacing = -150f;
        }
        hand.GetComponent<HorizontalLayoutGroup>().spacing = newSpacing;
    }*/

    public void SpawnExpSprite(Vector3 spawnPoint, int exp)
    {
        GameObject cloneExpPoint = Instantiate(expSpriteRenderer, spawnPoint, expSpriteRenderer.transform.rotation);
        cloneExpPoint.GetComponent<ExpSprite>().targetTransform = farmLevelSlider.gameObject.transform;
        cloneExpPoint.GetComponent<ExpSprite>().exp = exp;
    }

    public void FillAndShowLevelUpPanel(List<Card> unlockedCards)
    {
        if (cookingPanel.activeSelf==true)
        {
            RecipeManager.recipeManager.ClearAllCards(false);
            StartCoroutine(Cooldown(1f));
            OpenCloseCookPanel(cookingPanel);
        }

        for (int i = 0; i < unlockPanel.transform.GetChild(0).childCount; i++)
        {
            Destroy(unlockPanel.transform.GetChild(0).GetChild(i).gameObject);
        }
        
        for (int i = 0; i < unlockedCards.Count; i++)
        {
            GameObject cloneCard = Instantiate(GameManager.gameManager.cardCover,unlockPanel.transform.GetChild(0).transform.position,
                GameManager.gameManager.cardCover.transform.rotation,unlockPanel.transform.GetChild(0).transform);
            cloneCard.GetComponent<CardUI>().card = unlockedCards[i];
            cloneCard.GetComponent<CardUI>().FillCardInfo();
        }

        hand.SetActive(false);
        unlockPanel.SetActive(true);
    }

    public void CloseLevelUpPanel()
    {
        hand.SetActive(true);
        ClosePanel(unlockPanel);
    }

    public void ClosePanel(GameObject panel)
    {
        if (panel.activeSelf==true)
        {
            panel.SetActive(false);

            if(activePanel == panel)
                activePanel = null;

            GameManager.gameManager.isActive = true;
        }
    }

    public void CloseLeanWindow(LeanWindow panel)
    {
        if (activePanel == panel.gameObject)
            activePanel = null;

        panel.TurnOff();
    }

    public void OpenLeanWindow(LeanWindow panel)
    {
        panel.TurnOn();
        activePanel = panel.gameObject;
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        activePanel = panel;
    }

    public void OpenCloseCookPanel(GameObject panel)
    {
        if (panel.activeSelf==false)
        {
            GameManager.gameManager.isActive = false;
            inGameUI.SetActive(false);
            panel.SetActive(true);
            postVolume.profile.GetSetting<DepthOfField>().active = true;
        }
        else
        {
            GameManager.gameManager.isActive = true;
            panel.SetActive(false);
            inGameUI.SetActive(true);
            postVolume.profile.GetSetting<DepthOfField>().active = false;
        }
    }

    public void SpawnComboPopUp(string massage, Vector3 targetPosition)
    {
        GameObject clone = Instantiate(comboLevelUp, targetPosition, comboLevelUp.transform.rotation);
        clone.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = massage;
    }

    public void SpawnPopUpStatic(string massage,bool isNegative)
    {
        string symbol = "+";
        if (isNegative)
        {
            symbol = "-";
        }
        
        /*AudioManager.audioManager.ChangeAudioClip(3);
        AudioManager.audioManager.TriggerActiveClip();*/
        Instantiate(goldAudio, Vector3.zero, Quaternion.identity);
        TriggerGoldChange(massage,isNegative);
        GameObject clonePopupStatic =
            Instantiate(popUpStatic, Vector3.zero, popUpStatic.transform.rotation, mainCanvas.transform);
        clonePopupStatic.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = symbol + massage + "g";
        int randomY = Random.Range(20,60);
        int randomX = Random.Range(-30,30);
        clonePopupStatic.transform.position = new Vector3(Input.mousePosition.x + randomX, Input.mousePosition.y + randomY, Input.mousePosition.z);
    }

    public void TriggerGoldChange(string massage,bool isNegative)
    {
        string symbol = "+";
        if (isNegative)
        {
            symbol = "-";
        }
        //goldChangeText.GetComponent<TextMeshProUGUI>().text = symbol + massage + "g";
        //goldChangeText.GetComponent<Animator>().SetTrigger("Trigger");
    }

    public void ChangeCardCountText()
    {
        string colorHexCode;
        if (hands[FloorController.floorController.currentFloorIndex].handTransform.childCount < GameManager.gameManager.maksCardValue)
        {
            colorHexCode = "ffffffff";
        }
        else
        {
            colorHexCode = "ff0000ff";
        }
        
        cardCountText.text = "<color=#" + colorHexCode + ">" + hands[FloorController.floorController.currentFloorIndex].handTransform.childCount + "</color> /" + GameManager.gameManager.maksCardValue;
    }

    public void UnlockIsland()
    {
        if (unlockIsland != null  && unlockIsland.GetComponent<Island>().IsActive == false &&
            unlockIsland.GetComponent<Island>().price <= GameManager.gameManager.gold)
        {
            GameManager.gameManager.ChangeGold(unlockIsland.GetComponent<Island>().price, true);
            TriggerGoldChange(unlockIsland.GetComponent<Island>().price.ToString(), true);
            unlockIsland.SetActive(true);
            unlockIsland.GetComponent<Island>().CallUnlock();

            if (cloneArrow != null)
            {
                Destroy(cloneArrow);
            }            
        }
        else
        {
            ShowNotfy(LeanLocalization.GetTranslationText("NotEnoughGold"));
        }
        
        
        GameManager.gameManager.isActive = true;
    }

    public void SetGamemanagerTrue()
    {
        GameManager.gameManager.isActive = true;
    }

    public void SetUnlockIslandText()
    {
        unlockIslandPriceText.text = unlockIsland.GetComponent<Island>().price.ToString() + "g";
    }

    public IEnumerator CheckQuest()
    {
        if (!GameManager.gameManager.isActive)
            yield break; 
                
        yield return new WaitForSeconds(1.3f);
        Debug.Log("Check Quest");
        for (int i = 0; i < allActiveQuest.Count; i++)
        {
            List<CardUI> cardsInHand = new List<CardUI>(hand.GetComponentsInChildren<CardUI>());

            List<CardUI> requestCard = new List<CardUI>(cardsInHand.Where(p =>
                p.card.cardID == allActiveQuest[i].requestPlantID /*&& allActiveQuest[i].requestPieceLevel == p.level*/));
            
            Debug.Log("Request Card Count : " + requestCard.Count);
            for (int j = 0; j < requestCard.Count; j++)
            {
                if (allActiveQuest[i].pieceonWay < allActiveQuest[i].requestPiece && allActiveQuest[i].isActive)
                {
                    allActiveQuest[i].pieceonWay++;
                    allActiveQuest[i].AddCurrentPlant(requestCard[j].level);
                    allActiveQuest[i].rewardExp = Mathf.RoundToInt(requestCard[i].exp * 1.6f); // birden fazla kart isterse ?al??maz bu
                    allActiveQuest[i].rewardGold = Mathf.RoundToInt(requestCard[i].cardValue * 1.6f);

                    requestCard[j].GetComponent<CardUI>().MoveTo(allActiveQuest[i].transform.position);                    
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void TriggerCheckQuest()
    {
        StartCoroutine(CheckQuest());
    }

    public void StartGame()
    {
        GameManager.gameManager.isActive = true;
        uiCamera.SetActive(true);
        hand.SetActive(true);
        inGameUI.SetActive(true);
        mainMenuPanel.SetActive(false);
        mainMenuCamera.SetActive(false);
        inGameCamera.SetActive(true);
        FloorController.floorController.CreateFloorUI();

        if (PlayerPrefs.GetInt("tutorialPanelShowed") == 0)
        {
            tutorialPanel.gameObject.SetActive(true);
        }
    }

    private void Update()
    {        
    }

    public void OpenMainMenuPanel()
    {
        GameManager.gameManager.currentTilePart = null;
        GameManager.gameManager.isActive = false;
        uiCamera.SetActive(false);
        hand.SetActive(false);
        inGameUI.SetActive(false);
        mainMenuPanel.SetActive(true);
        mainMenuCamera.SetActive(true);
        inGameCamera.SetActive(false);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void OpenGuidePanel()
    {
        if (inGameUI.activeSelf==true)
        {
            inGameUI.SetActive(false);
            hand.SetActive(false);
            GuidePanel.SetActive(true);
        }
    }
    
    public void CloseGuidePanel()
    {
        if (inGameUI.activeSelf==false)
        {
            inGameUI.SetActive(true);
            hand.SetActive(true);
            GuidePanel.SetActive(false);   
        }
    }

    public void LoadData(GameData data)
    {
        for (int i = 0; i < data.handCards.Count; i++)
        {
            GameObject cloneCard = GameManager.gameManager.SpawnCardByID(data.handCards[i].cardID);
            cloneCard.transform.SetParent(hands.Find(p => p.handName == data.handCards[i].handName).handTransform);
        }

        if (hand.transform.childCount<=0)
        {
            //GameManager.gameManager.SpawnCards();
        }

        tutorialPanelShowed = data.tutorialShowed;        
    }

    public void SaveData(ref GameData data)
    {
        data.handCards.Clear();
        data.tutorialShowed = tutorialPanelShowed;
        /*foreach (Transform child in hand.transform)
        {
            CardData cardData = new CardData();
            cardData.cardID = child.GetComponent<CardUI>().card.cardID;
            cardData.level = child.GetComponent<CardUI>().level;
            data.handCards.Add(cardData);
        }*/

        List<GameObject> spawned = new List<GameObject>(GameManager.gameManager.spawnedCards);

        for (int i = 0; i < spawned.Count; i++)
        {
            CardUI cardUI = spawned[i].GetComponent<CardUI>();

            CardData cardData = new CardData();
            cardData.cardID = cardUI.card.cardID;
            cardData.level = cardUI.level;
            cardData.handName = hands.Find(p => p.handTransform == cardUI.transform.parent).handName;
            data.handCards.Add(cardData);
        }
    }

    public void TrytoPayRent()
    {
        Rent rent = GameManager.gameManager.rents.Find(p => p.farmLevel == GameManager.gameManager.farmLevel);
        if (rent.rentPrice<=GameManager.gameManager.gold)
        {
            GameManager.gameManager.ChangeGold(rent.rentPrice,true);
            TriggerGoldChange(rent.rentPrice.ToString(),true);
            ShowNotfy(LeanLocalization.GetTranslationText("Done"));
            RentPanel.OffTransitions.Begin();
            GameManager.gameManager.isActive = true;
        }
        else
        {
            ShowNotfy(LeanLocalization.GetTranslationText("NotEnoughGold"));
        }
    }

    public void CloseTutorialPanel()
    {
        PlayerPrefs.SetInt("tutorialPanelShowed", 1);
        tutorialPanelShowed = true;
        tutorialPanel.SetActive(false);
    }

    public void NewGame()
    {
        GameManager.gameManager.GameOver();
        StartGame();
    }

    public IEnumerator Cooldown(float second)
    {
        yield return new WaitForSeconds(second);
    }

    public void FeaturingButton(string url)
    {
        Application.OpenURL(url);        
    }

    public void OpenShop()
    {
        if (shopPanel.active == false)
        {
            activePanel = shopPanel;

            shopPanel.SetActive(true);            
            UIManager.uiManager.hands[FloorController.floorController.currentFloorIndex].handTransform.gameObject.SetActive(false);
        }
        else
        {
            shopPanel.SetActive(false);

            if(activePanel == shopPanel)
                activePanel = null;

            hands[FloorController.floorController.currentFloorIndex].handTransform.gameObject.SetActive(true);
        }
    }


    public void OpenCloseCredits()
    {
        if(creditsPanel.On == true)
        {
            creditsPanel.TurnOff();
            graphicSettingsPanel.TurnOn();

            creditsBtn.SetActive(true);
            backBtn.SetActive(false);
            settingsDoneBtn.SetActive(true);
        }
        else
        {

            creditsPanel.TurnOn();
            graphicSettingsPanel.TurnOff();

            creditsBtn.SetActive(false);
            backBtn.SetActive(true);
            settingsDoneBtn.SetActive(false);
        }
    }    
}
