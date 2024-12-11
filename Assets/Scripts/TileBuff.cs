using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBuff : MonoBehaviour
{
    public string name;
    [TextArea] public string description;
    public int buffRate;
    public bool isPlantBuff;
    [ConditionalField("isPlantBuff")]
    public string targetPlantID;
   
    public virtual void Execute()
    {
        
    }

    public virtual void PassiveBuff()
    {
        
    }
}
