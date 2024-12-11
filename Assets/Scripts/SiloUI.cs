using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiloUI : MonoBehaviour
{
    public void Open()
    {
        gameObject.SetActive(true);
        gameObject.transform.DOScale(Vector3.one, 0.2f).OnComplete(delegate
        {
            gameObject.GetComponent<Animator>().SetBool("Active", true);
        });
    }

    public void Close()
    {
        GetComponent<Animator>().SetBool("Active", false);
        gameObject.transform.DOScale(Vector3.zero, 0.2f).OnComplete(delegate
        {
            gameObject.SetActive(false);
        });
    }
}
