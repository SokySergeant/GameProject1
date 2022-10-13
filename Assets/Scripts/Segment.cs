using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Segment : MonoBehaviour
{
    public Transform[] EntryPoints;
    public Transform[] ExitPoints;

    [NonSerialized] public Segment[] EntrySegments;
    [NonSerialized] public Segment[] ExitSegments;

    [NonSerialized] public bool IsExpanded = false;
    
    public Prop[] Props;

    private void Awake()
    {
        SegmentManager.OnMoveSegments += Move;
    }

    private void OnDestroy()
    {
        SegmentManager.OnMoveSegments -= Move;
    }

    private void Move(Vector3 motion)
    {
        transform.position += motion;
    }
}