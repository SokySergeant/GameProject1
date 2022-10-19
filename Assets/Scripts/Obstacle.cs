using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : PickupAbstract
{
    public int damage = 1;

    public override void DoPickupAction(GameManager gameManager){
        gameManager.playerHp.ChangeHp(-damage);
        Destroy(gameObject);
    }
}
