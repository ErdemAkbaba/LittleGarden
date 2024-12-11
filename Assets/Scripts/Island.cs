using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Island : MonoBehaviour, IDataPersistance
{
    public string islandName;
    public int islandId;
    public List<GameObject> islandParts = new List<GameObject>();
    public List<GameObject> arrows = new List<GameObject>();    
    public int price;
    public bool unlockable;
    private bool isActive;
    public bool isMain;
    public bool isFirstIsland;
    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;
        }
    }

    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<IslandExpand>())
            {
                arrows.Add(child.gameObject);
            }
        }
    }

    public IEnumerator Unlock()
    {
        if(FloorController.floorController != null)
        {
            FloorController.floorController.isActive = false;
        }

        yield return new WaitForSeconds(0.5f);

        if (unlockable)
        {
            isActive = true;

            for (int i = 0; i < arrows.Count; i++)
            {
                arrows[i].SetActive(true);
            }
            
            for (int i = 0; i < islandParts.Count; i++)
            {
                islandParts[i].SetActive(true);
                islandParts[i].transform.position += new Vector3(0, 10, 0);
                islandParts[i].GetComponent<TilePart>().GoStartPos();
                yield return new WaitForSeconds(0.1f);
            }


            string islandName = gameObject.name;

            for (int i = 0; i < GameManager.gameManager.bigIslandDatas.Count; i++)
            {
                Island island = GameManager.gameManager.bigIslandDatas[i].islands.Find(p => p.gameObject.name == islandName).GetComponent<Island>();
                island.isActive = true;
            }

        }
        GameManager.gameManager.unlockedIsland.Invoke();

        if (FloorController.floorController != null)
        {
            FloorController.floorController.isActive = true;
        }


        yield return new WaitForSeconds(0.5f);
    }

    public void CallUnlock()
    {
        StartCoroutine(Unlock());
    }

    public void LoadData(GameData data)
    {
        if (isMain || islandName == "Restaurant" || islandName == "Mashroom")
            return;

        for (int i = 0; i < islandParts.Count; i++)
        {
            islandParts[i].SetActive(false);
        }

        if (data.islandIDs.Contains(islandId))
        {
            CallUnlock();
        }       
    }

    public void SaveData(ref GameData data)
    {
        if (isMain || islandName == "Restaurant" || islandName == "Mashroom")
            return;

        if (isActive && !data.islandIDs.Contains(islandId))
        {
            data.islandIDs.Add(islandId);
        }
    }
}
