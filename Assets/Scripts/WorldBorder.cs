using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBorder : MonoBehaviour
{
    public Transform spawnPos;

    void OnTriggerEnter(Collider other){
        //collision with the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            other.gameObject.transform.position = spawnPos.position;
        }
    }
}
