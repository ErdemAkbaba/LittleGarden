using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Localization;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BeeHouse : Building
{
    [Range(0, 100)] public int beeSpawnRate;
    public Card beeCard;
    public GameObject addCardParticle;
    public TextMeshProUGUI countTextUI;
    private int beeRemoveRemaningDay = 3;
    
    void Start()
    {
        countTextUI.text = currentAnimalCount + "/" + maxAnimalCount;
        GameManager.gameManager.nextDay.AddListener(delegate
        {
            StartCoroutine(SpawnHoney());
            SpawnBee();
        });
    }

    public override void AddCard()
    {
        base.AddCard();

        if (currentAnimalCount>=maxAnimalCount)
        {
            UIManager.uiManager.ShowNotfy(LeanLocalization.GetTranslationText("MaximumNumberReached"));
            return;
        }

        Instantiate(addCardParticle, transform.position, addCardParticle.transform.rotation);
        currentAnimalCount++;
        countTextUI.text = currentAnimalCount + "/" + maxAnimalCount;
    }

    public IEnumerator SpawnHoney()
    {
        float randomwait = Random.Range(2.5f, 3f);
        yield return new WaitForSeconds(randomwait);
        for (int i = 0; i < currentAnimalCount; i++)
        {
            Instantiate(addCardParticle, transform.position, addCardParticle.transform.rotation);   
            GameObject cloneCard = GameManager.gameManager.SpawnCardByCard(rewardCard,true);
        }
    }
    
    public void SpawnBee()
    {
        int beeSpawn = Random.Range(0, 101);
        if (currentAnimalCount<maxAnimalCount && beeSpawn<beeSpawnRate)
        {
            GameManager.gameManager.SpawnCardByCard(beeCard,false);
        }
    }

    private void OnDestroy()
    {
        GameManager.gameManager.nextDay.RemoveListener(delegate
        {
            StartCoroutine(SpawnHoney());
        });
    }
}
