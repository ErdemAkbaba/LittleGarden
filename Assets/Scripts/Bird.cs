using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Bird : RandomEventObject
{
    public Transform center;

    public static Bird bird;

    private Vector3 defSize;

    private Outline outline;
    private bool isCollected;

    private void Awake()
    {
        bird = this;
    }
    public override void Start()
    {
        center = GameObject.FindAnyObjectByType<RandomEvents>().transform.Find("AirSpawnpoint").transform;
        transform.position = RandomSpawnpointPos();
        TargetPlant();
        outline = GetComponent<Outline>();

        defSize = transform.localScale;
        price = price * GameManager.gameManager.farmLevel;
    }

    public override void OnMouseEnter()
    {
        if (isCollected) return;
        outline.enabled = true;
    }

    public override void OnMouseDown()
    {
        if (isCollected)
            return;

        isCollected = true;
        //RandomEvents.randomEvents.Invoke("GetEvent", RandomEvents.randomEvents.nextEventTime);
        UIManager.uiManager.SpawnPopUpStatic(price.ToString(), false);
        GameManager.gameManager.ChangeGold(Mathf.CeilToInt(price), false);
        CameraShake.camerShake.Shake(0.2f, 0.15f);
        RandomEvents.randomEvents.currentEventCount--;

        if (this.transform.GetComponentInChildren<Plant>() != null)
        {
            Plant carrot = GetComponentInChildren<Plant>();
            carrot.transform.SetParent(null,true);
            carrot.ReadytoHarvest();
        }

        if (transform.Find("PlantHolder").childCount == 0)
        {
            Destroy(gameObject);
        }
    }

    public override void OnMouseExit()
    {
        if (isCollected) return;
        outline.enabled = false;
    }

    private void TargetPlant()
    {
        List<TilePart> tileParts = new List<TilePart>(GameManager.gameManager.activeTiles.Where(p => p.childPlant));
        int r = Random.Range(0, tileParts.Count);

        if (tileParts[r].childPlant.GetComponent<Plant>().isGrowth)
        {
            Plant plant = tileParts[r].childPlant.GetComponent<Plant>();
            Vector3 targetPlant = new Vector3(plant.transform.position.x, plant.transform.position.y + 1f, plant.transform.position.z);
            transform.DOMove(targetPlant, 5).OnComplete(delegate
            {
                tileParts[r].HarvestAnimation(true);
                tileParts[r].childPlant.GetComponent<Plant>().transform.SetParent(transform.Find("PlantHolder"));
                Vector3 randomFarAwayPos = RandomSpawnpointPos();
                transform.LookAt(randomFarAwayPos);
                transform.DOMove(randomFarAwayPos, 5).OnComplete(delegate
                {
                    Destroy(gameObject, .4f);
                });
            });
        }
    }

    private Vector3 RandomSpawnpointPos()
    {
        float dist = UnityEngine.Random.Range(20, 30);
        float angle = UnityEngine.Random.Range(0, 360);

        Vector3 pos = new Vector3();

        pos.x = center.position.x + (dist * Mathf.Cos(angle / (180f / Mathf.PI)));
        pos.y = center.position.y;
        pos.z = center.position.z + (dist * Mathf.Sin(angle / (180f / Mathf.PI)));

        return pos;
    }
}
