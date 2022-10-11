using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSectionTriggerScript : MonoBehaviour
{
    public delegate void OnNewSectionTriggerEnter();
    public static event OnNewSectionTriggerEnter onNewSectionTriggerEnter;



    void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            onNewSectionTriggerEnter?.Invoke();
        }
    }
}
