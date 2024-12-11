using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float lifeTime;
    public bool isRandom;
    void Start()
    {
        if(isRandom)
            lifeTime = Random.Range(1, lifeTime);
        
        Destroy(this.gameObject,lifeTime);
    }
}
