using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentTrigger : MonoBehaviour
{
    public delegate void OnNewSectionTriggerEnter();
    public static event OnNewSectionTriggerEnter onSegmentTriggerEnter;



    void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            onSegmentTriggerEnter?.Invoke();
        }
    }
}
