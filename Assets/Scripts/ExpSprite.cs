using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpSprite : UISprite
{
    [HideInInspector] public int exp;
    public GameObject farmLevelSliderFX;
    private void Start()
    {
        base.TriggerFollow();
    }
    
    public override void OnPathComplete()
    {
        GameManager.gameManager.LevelProgress(exp);
        Instantiate(farmLevelSliderFX, transform.position, farmLevelSliderFX.transform.rotation);
        Destroy(gameObject);
    }
}
