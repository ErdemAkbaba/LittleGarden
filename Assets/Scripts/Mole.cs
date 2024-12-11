using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Mole : RandomEventObject
{
    private Outline outline;
    private bool isCollected;

    private Animator animator;

    public override void Start()
    {
        outline = GetComponent<Outline>();

        List<TilePart> activeTilePart = new List<TilePart>(GameManager.gameManager.activeTiles.Where(p => !p.childPlant && p.statusIndex == 0));

        price = price * GameManager.gameManager.farmLevel;

        int r = Random.Range(0, activeTilePart.Count);
        transform.SetParent(activeTilePart[r].transform);
        transform.localPosition = new Vector3(0, -.1f, 0);
        activeTilePart.Remove(activeTilePart[r]);
        animator = GetComponent<Animator>();
        animator.SetTrigger("Start");
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
        animator.SetTrigger("Escape");
        //GetComponent<Animator>().enabled = false;
        //transform.DOScale(Vector3.zero, 0.3f);
        UIManager.uiManager.SpawnPopUpStatic(price.ToString(), false);
        GameManager.gameManager.ChangeGold(Mathf.CeilToInt(price), false);
        CameraShake.camerShake.Shake(0.2f, 0.15f);
    }

    public void DestroyObject()
    {
        RandomEvents.randomEvents.currentEventCount--;
        Destroy(gameObject, 0.4f);
    }

    public virtual void OnMouseExit()
    {
        if (isCollected) return;
        outline.enabled = false;
    }

    public void MoleEscape()
    {
        Destroy(this.gameObject, .5f);
    }
}
