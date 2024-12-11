using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventObject : MonoBehaviour
{

    private Vector3 defSize;
    public float price;

    public float r;

    private Vector3 targetPos;
    public float stepSize;

    private Outline outline;
    private bool isCollected;

    private Vector3 center;

    public virtual void Start()
    {
        center = Vector3.zero;
        outline = GetComponent<Outline>();

        price = price * GameManager.gameManager.farmLevel;
        defSize = transform.localScale;

        GetRandomPos();
    }

    public virtual void OnMouseEnter()
    {
        if (isCollected) return;
        transform.DOScale(defSize * 1.2f, 0.2f);
        outline.enabled = true;
    }

    public virtual void OnMouseDown()
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

    public virtual void OnMouseExit()
    {
        if (isCollected) return;
        outline.enabled = false;
        transform.DOScale(defSize, 0.2f);
    }

    private void GetRandomPos()
    {
        targetPos = r * UnityEngine.Random.insideUnitSphere;
        targetPos.y = transform.position.y;

        transform.DOMove(targetPos, (stepSize * Vector3.Distance(center, targetPos))).OnComplete(delegate
        {
            GetRandomPos();
        }).SetEase(Ease.InOutSine);

        transform.DOLookAt(targetPos, 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, r);
    }
}
