using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupAbstract : MonoBehaviour
{
    private GameManager gameManager;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnTriggerEnter(Collider other){
        //collision with the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            DoPickupAction(gameManager);
        }
    }

    public abstract void DoPickupAction(GameManager gameManager);
}
