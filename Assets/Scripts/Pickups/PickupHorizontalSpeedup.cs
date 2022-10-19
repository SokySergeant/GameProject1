using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHorizontalSpeedup : PickupAbstract
{
    public float speedIncrease = 5f;

    public override void DoPickupAction(GameManager gameManager){
        gameManager.playerMovement.playerSpeed += speedIncrease;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Objects/Pickups/PickupTech");
        Destroy(gameObject);
    }
}
