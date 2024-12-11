using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class testoutline : MonoBehaviour
{
    void Awake()
    {
        Outline outline = gameObject.AddComponent<Outline>();
        outline.OutlineWidth = 6;
    }
    
    void Update()
    {
        
    }
}
