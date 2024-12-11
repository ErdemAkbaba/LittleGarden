using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovebleObjects : MonoBehaviour
{
    private void Start()
    {
        transform.parent.GetComponent<TilePart>().tileStatusCahnged.AddListener(delegate
        {
            Destroy(gameObject);
        });
    }
}
