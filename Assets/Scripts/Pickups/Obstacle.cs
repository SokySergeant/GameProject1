using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : PickupAbstract
{
    public int damage = 1;

    public override void DoPickupAction(GameManager gameManager){
        gameManager.playerHp.ChangeHp(-damage);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Objects/Obstacles/Obstacle1");
        Destroy(gameObject);
    }
}
