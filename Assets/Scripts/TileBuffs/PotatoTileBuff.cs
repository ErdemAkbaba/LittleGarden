using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoTileBuff : TileBuff
{
    private void Start()
    {
        gameObject.GetComponent<TilePart>().tileHarvest.AddListener(Execute);
    }

    public override void Execute()
    {
        base.Execute();
        Debug.Log(name);
        Debug.Log(description);
    }

    public override void PassiveBuff()
    {
        base.PassiveBuff();
    }
}
