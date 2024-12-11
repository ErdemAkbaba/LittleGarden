using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSprite : MonoBehaviour
{
    [HideInInspector] public Transform targetTransform;
    public float minDuration, maxDuration;
    public SpriteRenderer icon;


    private void Start()
    {
    }

    public void TriggerFollow()
    {
        float duration = Random.Range(minDuration, maxDuration);
        Vector3 targetPosition = targetTransform.position;
        targetPosition.z = (transform.position - Camera.main.transform.position).z;
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(targetPosition);
        transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), duration / 2f).OnComplete(delegate
        {
            transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), duration / 2);
        });
        transform.DOMove(targetPos, duration).SetEase(Ease.InQuint).OnComplete(delegate
        {
            OnPathComplete();
        });
    }


    public virtual void OnPathComplete()
    {
        targetTransform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).OnComplete(delegate
        {
            targetTransform.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
            Destroy(gameObject);
        });        
    }
}
