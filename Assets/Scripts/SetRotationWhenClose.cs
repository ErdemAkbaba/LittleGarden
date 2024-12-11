using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRotationWhenClose : MonoBehaviour
{

    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnDisable()
    {
        anim.enabled = false;
        
        transform.eulerAngles = Vector3.zero;

        anim.enabled = true;
    }
}
