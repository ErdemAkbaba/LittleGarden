using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    public static MergeManager mergeManager;
    public List<CardUI> cards = new List<CardUI>();
    private void Awake()
    {
        mergeManager = this;
    }

    private void Update()
    {
    }

    public IEnumerator CheckCards() //refactor!! // check is plant
    {
        yield return new WaitForSeconds(1f); // belki bir tık kısılabilir
        
        List<GameObject> mergableCards = new List<GameObject>();
        mergableCards.AddRange(GameManager.gameManager.spawnedCards.Where(p => p.GetComponent<CardUI>().card.isMergable));

        for (int i = 0; i < GameManager.gameManager.spawnedCards.Count; i++)
        {
            string targetPlantID = "";
            
            if (GameManager.gameManager.spawnedCards[i].GetComponent<CardUI>().card.isMergable)
            {
                targetPlantID = GameManager.gameManager.spawnedCards[i].GetComponent<CardUI>().card.cardID;
                
                List<GameObject> sameCards = new List<GameObject>();
        
                for (int l = 1; l < 3; l++)  // level check
                {
                    sameCards.Clear();
                    int targetLevel = l;
                    sameCards.AddRange(mergableCards.Where(p =>
                        p.GetComponent<CardUI>().card.cardID == targetPlantID && 
                        p.GetComponent<CardUI>().level == targetLevel));

                    if (sameCards.Count >= 3)
                    {
                        Debug.LogWarning( sameCards.Count / 3 + " Tane Merge Edilebilir Cart Kümesi Var!!");

                        for (int j = 0; j < sameCards.Count / 3; j++)
                        {
                            Card sameCardClone;
                            sameCardClone = sameCards[j].GetComponent<CardUI>().card;
                            for (int k = 0; k < 3; k++)
                            {
                                GameManager.gameManager.spawnedCards.Remove(sameCards[k]);
                                mergableCards.Remove(sameCards[k]);
                                sameCards[k].transform.DOMoveY(sameCards[k].transform.position.y + 50, 0.5f)
                                    .SetEase(Ease.InCubic);
                            }

                            yield return new WaitForSeconds(0.5f);
                    
                            for (int k = 0; k < 3; k++)
                            {
                                Destroy(sameCards[0].gameObject);   
                                sameCards.Remove(sameCards[0]);  
                            }
                            
                            GameObject cloneCard = GameManager.gameManager.SpawnCardByCard(sameCardClone,true);
                            for (int k = 1; k<targetLevel+1; k++)
                            {
                                //cloneCard.GetComponent<CardUI>().LevelUp();
                            }                           
                        }
                    }

                }
                
            }
        }
        yield return new WaitForSeconds(0.1f);
    }
}
