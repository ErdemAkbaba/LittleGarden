using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Quest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public QuestData questData;
    [FormerlySerializedAs("rewardCoin")] public int rewardGold;
    public int rewardExp;
    public int requestPiece;
    public string requestPlantID;
    public int requestPieceLevel=1;
    public int currentPiece;
    public int pieceonWay;
    public Sprite iconSprite;
    public GameObject ProgressFx;
    [HideInInspector] public bool isActive;
    private Animator animator;
    [Space(10)] [Header("UI")] 
    public TextMeshProUGUI requestPieceText;
    public TextMeshProUGUI descriptionText;
    public Image icon;
    void Start()
    {
        animator = GetComponent<Animator>();
        isActive = true;
        icon.sprite = iconSprite;
        requestPieceText.text = requestPiece.ToString();

        //LeanLocalization.OnLocalizationChanged += FillData;        
    }

    private void OnEnable()
    {
        LeanLocalization.OnLocalizationChanged += FillData;
    }

    public void AddCurrentPlant(int level)
    {
        currentPiece++;
        animator.SetTrigger("Progress");
        requestPieceText.text = (requestPiece - currentPiece).ToString();

        if (currentPiece >= requestPiece)
        {
            CompleteQuest(level);
        }
    }

    public void CompleteQuest(int level = 1)
    {
        isActive = false;
        StartCoroutine(SpawnRewards(level));
    }

    public void DestroyQuest()
    {        
        Destroy(this.gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("Open", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("Open", false);
    }

    private void OnDisable()
    {
        //LeanLocalization.OnLocalizationChanged -= FillData;
    }

    public void FillData()
    {        
        name = questData.name;
        iconSprite = questData.requestPlantIcon;
        requestPiece = questData.requestPlantCount;
        rewardGold = questData.rewardGold;
        rewardExp = questData.rewardExp;
        requestPlantID = questData.requestCard_ID;
        requestPieceLevel = questData.requestPlantLevel;

        LeanLocalization.SetToken("RANDOM_PLANT_COUNT", questData.requestPlantCount.ToString());
        LeanLocalization.SetToken("CARD_NAME", LeanLocalization.GetTranslationText(questData.requestPlantName));
        LeanLocalization.SetToken("RANDOM_LEVEL", questData.requestPlantLevel.ToString());
        questData.Description = LeanLocalization.GetTranslationText("QuestDesc");

        descriptionText.text = 
            questData.Description + "\n" + $"{LeanLocalization.GetTranslationText("Rewards")}:" + "\n" + "<color #FFB900>" + questData.rewardExp + "g" + "</color>" + 
             "<color #3DDD77>" + "\n" + questData.rewardGold + "exp" + "</color>"; 
    }

    private IEnumerator SpawnRewards(int level)
    {
        Vector3 targetPosition = transform.position;
        targetPosition.z = (transform.position - Camera.main.transform.position).z;
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(targetPosition);

        Card questCard = GameManager.gameManager.AllCards.Where(p => p.cardID == requestPlantID).First();

        if (questCard)
        {
            Card rewardCard = GameManager.gameManager.AllCards.Find(p => p.cardID == requestPlantID);
            //int rewardExpCal = rewardExp;
            //int rewardPriceCal = ;

            UIManager.uiManager.SpawnExpSprite(targetPos, rewardExp);
            yield return new WaitForSeconds(0.2f);

            GameManager.gameManager.gold += rewardGold;
            UIManager.uiManager.TriggerGoldChange(rewardGold.ToString(), false);
            animator.SetTrigger("Complete");
            UIManager.uiManager.allActiveQuest.Remove(this);            
        }
    }
}
