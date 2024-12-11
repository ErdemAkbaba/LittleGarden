using DG.Tweening;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;
using Steamworks.Data;
using Image = UnityEngine.UI.Image;
using Unity.VisualScripting;
using System.Drawing;
using UnityEngine.UIElements;
using JetBrains.Annotations;

[Serializable]
public class NPCStatusIcon
{
    public enum Status
    {
        happy,
        angry, 
        wantFood,
        pathNotWalkable
    }

    public Status status;
    public Sprite icon;
}

[Serializable]
public class NPCSkins
{
    public Mesh body;
    public Material bodySkinMaterial;
    public Mesh leftArm;
    public Material leftArmSkinMaterial;
    public Mesh rightArm;
    public Material rightArmSkinMaterial;
    public Mesh leftLeg;
    public Material leftLegSkinMaterial;
    public Mesh rightLeg;
    public Material rightLegSkinMaterial;

    public NPCSkins(Mesh body, Material bodySkinMaterial, Mesh leftArm, Material leftArmSkinMaterial, Mesh rightArm, Material rightArmSkinMaterial, Mesh leftLeg, Material leftLegSkinMaterial, Mesh rightLeg, Material rightLegSkinMaterial)
    {
        this.body = body;
        this.bodySkinMaterial = bodySkinMaterial;
        this.leftArm = leftArm;
        this.leftArmSkinMaterial = leftArmSkinMaterial;
        this.rightArm = rightArm;
        this.rightArmSkinMaterial = rightArmSkinMaterial;
        this.leftLeg = leftLeg;
        this.leftLegSkinMaterial = leftLegSkinMaterial;
        this.rightLeg = rightLeg;
        this.rightLegSkinMaterial = rightLegSkinMaterial;
    }
}

public class NPC : MonoBehaviour
{
    public GameObject npcStatusCanvas;
    public GameObject npcWaitCanvas;
    public List<NPCStatusIcon> npcStatusIcon;
    public List<NPCSkins> npcSkins;
    public GameObject bodySkin;
    public GameObject rightArmSkin;
    public GameObject leftArmSkin;
    public GameObject rightLegSkin;
    public GameObject leftLegSkin;

    public static NPC npc;

    private Vector3 spawnPoint;

    private List<Chairs> chairs;
    Chairs chair;
    public float mealTime;

    public Transform targetTransform;
    public Animator animator;

    NavMeshAgent agent;

    [HideInInspector] public UnityEvent NpcStatusChanged;

    private Vector3 defImageSize;

    private void Awake()
    {
        npc = this;
        NpcStatusChanged = new UnityEvent();
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.updateRotation = true;        
        animator = GetComponentInChildren<Animator>();

        int r = UnityEngine.Random.Range(0, npcSkins.Count);
        bodySkin.GetComponent<MeshFilter>().mesh = npcSkins[r].body;
        bodySkin.GetComponent<MeshRenderer>().sharedMaterial = npcSkins[r].bodySkinMaterial;
        rightArmSkin.GetComponent<MeshFilter>().mesh = npcSkins[r].rightArm;
        rightArmSkin.GetComponent<MeshRenderer>().sharedMaterial = npcSkins[r].rightArmSkinMaterial;
        leftArmSkin.GetComponent<MeshFilter>().mesh = npcSkins[r].leftArm;
        leftArmSkin.GetComponent<MeshRenderer>().sharedMaterial = npcSkins[r].leftArmSkinMaterial;
        leftLegSkin.GetComponent<MeshFilter>().mesh = npcSkins[r].leftLeg;
        leftLegSkin.GetComponent<MeshRenderer>().sharedMaterial = npcSkins[r].leftLegSkinMaterial;
        rightLegSkin.GetComponent<MeshFilter>().mesh = npcSkins[r].rightLeg;
        rightLegSkin.GetComponent<MeshRenderer>().sharedMaterial = npcSkins[r].rightLegSkinMaterial;
        //npcSkin.GetComponent<MeshFilter>().mesh = npcSkins[r].skin;
        //npcSkin.GetComponent<MeshRenderer>().sharedMaterial = npcSkins[r].skinMaterial;

        Vector3 defScale = transform.localScale;
        //transform.localScale = Vector3.zero;
        /*transform.DOScale(defScale, 0.2f).OnComplete(delegate
        {
            agent.enabled = true;
        });*/

        transform.position = RandomNavmeshLocation(100);
        defImageSize = npcStatusCanvas.GetComponent<Image>().rectTransform.localScale;
        GetComponent<Outline>().enabled = false;

        SetChair();
    }

    public void SetChair()
    {
        avaliable.Clear();
        avaliable = new List<Chairs>(FindAnyObjectByType<Restaurant>().chairs.Where(p => p.isSeatable == true));
        if (avaliable.Count > 0)
        {
            chair = avaliable[UnityEngine.Random.Range(0, avaliable.Count)];
            chair.isSeatable = false;
        }
    }

    public void StatusChange(int npcStatusIndex)
    {
        npcStatusCanvas.GetComponent<Image>().rectTransform.localScale = defImageSize;
        npcStatusCanvas.GetComponent<Image>().sprite = npcStatusIcon[npcStatusIndex].icon;                
    }


    public List<Chairs> avaliable = new List<Chairs>();
    bool seat;

    public float eatTime = 20f;
    public float waitTime = 10f;
    bool isEated = false;
    bool isWaiting = false;
    bool startEat = false;

    public IEnumerator SitNpc()
    {
        animator.SetBool("Sit", true);
        yield return new WaitForSeconds(2f);        
        CanEatMeal();
    }

    private void Update()
    {
        if (targetTransform == null)
        {
            if (avaliable.Count > 0)
            {
                Walk(false);
            }
        }
        if (!seat)
        {
            if (chair != null)
            {
                //Debug.Log(Vector2.Distance(transform.position, targetTransform.position));
                if (Vector2.Distance(transform.position, targetTransform.position) <= 0.5f && targetTransform != null && agent.enabled)
                {
                    StopCoroutine(AngryCustomer());
                    agent.enabled = false;
                    //chair.isSeatable = false;
                    seat = true;
                    transform.SetParent(targetTransform.GetComponentInChildren<MeshRenderer>().transform.parent, true);
                    transform.DOScale(new Vector3(4f, 4f, 4f) , .1f);
                    //transform.localScale = new Vector3(4f, 4f, 4f);
                    transform.localEulerAngles = Vector3.zero;
                    transform.DOLocalMove(new Vector3(0, 2, 0), 0.2f);
                    StartCoroutine(SitNpc());
                }                
            }
            else if (targetTransform == null)
            {
                StartCoroutine(AngryCustomer());
            }
        }

        /*if (randomPos != null && Vector2.Distance(transform.position, randomPos.transform.position) <= .1 && targetTransform != null)
        {
            Walk(true);
        }*/

        if (eatTime > 0 && startEat == true)
        {
            eatTime -= Time.deltaTime;           
            npcWaitCanvas.GetComponent<Image>().fillAmount = eatTime * .2f;
            startEat = true;
        }
        else if (eatTime <= 0)
        {
            startEat = false;
            isEated = true;
            StartCoroutine(HappyCustomer());
            StatusChange(0);
            npcWaitCanvas.GetComponent<Image>().fillAmount = 1;
        }

        if (waitTime > 0 && isWaiting == true)
        {
            waitTime -= Time.deltaTime;
            npcWaitCanvas.GetComponent<Image>().fillAmount = waitTime * .1f;
            isWaiting = true;
        }
        else if (waitTime <= 0)
        {
            isWaiting = false;
            npcWaitCanvas.GetComponent<Image>().fillAmount = 1;
        }

        if(targetTransform != null && !seat)
        {
            Vector3 direction = targetTransform.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(direction, transform.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 5 * Time.deltaTime);
        }
    }

    public Transform randomSetDestinationPlace;
    GameObject randomPos;
    public void Walk(bool isRandom)
    {
        animator.SetBool("Eat", false);

        if (isRandom == false)
        {
            //agent.enabled = true;
            targetTransform = chair.chair.transform;
            agent.SetDestination(targetTransform.position);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            if (randomPos == null)
            {
                randomPos = Instantiate(randomSetDestinationPlace.gameObject);
                randomPos.transform.position = RandomNavmeshLocation(100);
            }
            targetTransform = randomPos.transform;
            agent.SetDestination(randomPos.transform.position);
            animator.SetBool("IsWalking", true);
        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
    again:
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }

        if (finalPosition == null ||finalPosition == Vector3.zero)
            goto again;

        return finalPosition;
    }

    public IEnumerator AngryCustomer()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        agent.enabled = true;

        if (seat)
        {
            Restaurant.restourant.chairs.Find(p => p.chair == transform.root.gameObject).isSeatable = true;
            //chair.isSeatable = true;
        }
        transform.SetParent(null);
        Walk(true);
        StatusChange(1);
        yield return new WaitForSeconds(4f);
        int r = UnityEngine.Random.Range(0, 100);
        if (r < 51)
        {
            Restaurant.restourant.ReduceReputation(UnityEngine.Random.Range(.01f, .15f));
        }
        Destroy(this.gameObject);
    }

    public IEnumerator HappyCustomer()
    {
        animator.SetBool("Eat", false);
        animator.SetBool("Sit", false);
        agent.enabled = true;
        transform.SetParent(null);
        Walk(true);
        yield return new WaitForSeconds(1f);
        if (isEated == true)
        {
            int r = UnityEngine.Random.Range(0, 100);
            if (r < 51)
            {
                List<CellData> cells = new List<CellData>(GridBuildingSystem.Instance.cellData.Where(p => p.isObjectPlaced));
                float totalRepValue = 0;
                foreach (CellData cell in cells)
                {
                    totalRepValue += cell.objectCard.reputationValue;
                }
                Receipt.Instance.totalTip += (int)(10 * totalRepValue);
                //GameManager.gameManager.ChangeGold((int)(10 * totalRepValue), false);
            }

            Restaurant.restourant.mealCountChange.Invoke();
            //chair.isSeatable = true;
            Restaurant.restourant.chairs.Find(p => p == chair).isSeatable = true;
            seat = false;
            transform.DOScale(new Vector3(0, 0, 0), .1f).OnComplete(delegate {
                Destroy(this.gameObject);
            });
        }
    }

    public Vector3 RandomSpawnpoint()
    {
        return transform.position = RandomNavmeshLocation(100);
    }

    private void CanEatMeal()
    {
        if (CanMealChoose() == true)
        {
            animator.SetBool("Eat", true);
            startEat = true;
        }
        else
        {
            StartCoroutine(AngryCustomer());
        }
    }

    bool CheckLineOfSight()
    {
        bool inSight = false;
        RaycastHit hit;

        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        Vector3 end = targetTransform.position;

        if (Physics.Linecast(origin, end, out hit))
        {
            if (hit.transform == targetTransform)
            {
                inSight = true;
            }
        }

        return inSight;
    }
    public bool CanMealChoose()
    {

        List<Recipe> choosenRecipes = new List<Recipe>();

        for (int i = 0; i < Restaurant.restourant.meals.Count; i++)
        {
            choosenRecipes.Add(Restaurant.restourant.meals[i].recipe);
        }

        if (choosenRecipes.Count == 0)
        {
            Destroy(gameObject);
        }

        Recipe recipe = choosenRecipes[UnityEngine.Random.Range(0, choosenRecipes.Count - 1)];
        //Debug.Log(recipe.recipeName);
        npcStatusCanvas.GetComponent<Image>().rectTransform.localScale = Vector3.one;
        npcStatusCanvas.GetComponent<Image>().sprite = recipe.recipeIcon;
        Meal meal = Restaurant.restourant.meals.Find(p => p.recipe == recipe);
        
        if (meal != null)
        {
            meal.count--;
            Silo.silo.AddSiloStock(-1, meal.recipe.resultCardData[0]);

            ReceiptItem receiptItem = Receipt.Instance.receiptItems.Find(p => p.meal == meal);
            if (receiptItem == null)
            {
                Receipt.Instance.receiptItems.Add(new ReceiptItem(meal.recipe.recipeIcon, meal.recipe.recipeName, 1, meal.recipe.resultCardData[0].price, meal));
                Receipt.Instance.totalEarning += meal.recipe.resultCardData[0].price;
            }
            else
            {
                receiptItem.mealCount++;
                Receipt.Instance.totalEarning += meal.recipe.resultCardData[0].price;
                receiptItem.totalValue += meal.recipe.resultCardData[0].price;
            }

            if (meal.count < 1)
            {
                Restaurant.restourant.meals.Remove(meal);
            }

            return true;
        }

        return false;        
    }

    private void OnMouseDown()
    {
        OnClick();
    }

    public void OnClick()
    {
        
    }

    private void OnMouseOver()
    {
        GetComponent<Outline>().enabled = true;     
    }

    private void OnMouseExit()
    {
        GetComponent<Outline>().enabled = false;
    }

    private void OnDestroy()
    {
        seat = false;
        Destroy(randomPos);
    }
}
