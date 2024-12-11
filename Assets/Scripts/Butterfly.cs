using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Butterfly : RandomEventObject
{
    public Transform center;

    private bool isCollected;
    private Outline outline;

    public override void Start()
    {
        center = GameObject.FindAnyObjectByType<RandomEvents>().transform.Find("AirSpawnpoint").transform;
        transform.position = RandomSpawnpointPos();
        //transform.position = new Vector3(transform.position.x, center.position.y, transform.position.z);
        outline = GetComponent<Outline>();

        price = price * GameManager.gameManager.farmLevel;

        GetRandomPos();
    }

    public override void OnMouseEnter()
    {
        if (isCollected) return;
        //transform.DOScale(defSize * 1.2f, 0.2f);
        outline.enabled = true;
    }

    public override void OnMouseDown()
    {
        if (isCollected)
            return;

        isCollected = true;
        GetComponent<Animator>().enabled = false;
        //transform.DOScale(Vector3.zero, 0.3f);
        UIManager.uiManager.SpawnPopUpStatic(price.ToString(), false);
        GameManager.gameManager.ChangeGold(Mathf.CeilToInt(price), false);
        CameraShake.camerShake.Shake(0.2f, 0.15f);
        RandomEvents.randomEvents.currentEventCount--;
        Destroy(gameObject, 0.4f);
    }

    public override void OnMouseExit()
    {
        if (isCollected) return;
        outline.enabled = false;
        //transform.DOScale(defSize, 0.2f);
    }

    private Vector3 RandomSpawnpointPos()
    {
        Vector3 pos = new Vector3();

        float dist = UnityEngine.Random.Range(20, 30);
        float angle = UnityEngine.Random.Range(0, 360);

        pos.x = center.position.x + (dist * Mathf.Cos(angle / (180f / Mathf.PI)));
        pos.y = center.position.y;
        pos.z = center.position.z + (dist * Mathf.Sin(angle / (180f / Mathf.PI)));

        return pos;
    }

    private void GetRandomPos()
    {
        Vector3 pos = new Vector3();

        float dist = UnityEngine.Random.Range(0, 10);
        float angle = UnityEngine.Random.Range(0, 360);

        pos.x = center.position.x + (dist * Mathf.Cos(angle / (180f / Mathf.PI)));
        pos.y = center.position.y;
        pos.z = center.position.z + (dist * Mathf.Sin(angle / (180f / Mathf.PI)));

        transform.DOMove(pos, (stepSize * Vector3.Distance(transform.localPosition, pos))).OnComplete(delegate
        {
            GetRandomPos();
        }).SetEase(Ease.InOutSine);

        transform.DOLookAt(pos, 0.5f);
    }
}
