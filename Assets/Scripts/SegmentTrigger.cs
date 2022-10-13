using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentTrigger : MonoBehaviour
{
    public Segment Segment;
    
    public static event Action<Segment> OnSegmentEnter;

    private void Awake()
    {
        Segment = GetComponentInParent<Segment>();
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            OnSegmentEnter?.Invoke(Segment);
        }
    }
}
