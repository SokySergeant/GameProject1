using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHealth : PickupAbstract
{
    public override void DoPickupAction(GameManager gameManager){
        Debug.Log(gameManager.player);
        Destroy(gameObject);
    }
}
