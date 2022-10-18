using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Segment : MonoBehaviour
{
    public Transform EntryPoint;
    public Transform ExitPoint;

    [NonSerialized] public Segment EntrySegment;
    [NonSerialized] public Segment ExitSegment;

    [NonSerialized] public bool IsExpanded = false;
    
    public Prop[] Props;

    public bool HasEntrySegment => EntrySegment != null;
    public bool HasExitSegment => ExitSegment != null;

    public float Length => Vector3.Distance(EntryPoint.position, ExitPoint.position);

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

#if UNITY_EDITOR

[CustomEditor(typeof(Segment))]
public class SegmentEditor : Editor
{
    public float ArrowSize = 2.5f;

    protected virtual void OnSceneGUI()
    {
        Segment t = target as Segment;
        
        Handles.color = Handles.xAxisColor;
        if (t.EntryPoint != null)
            Handles.ArrowHandleCap(0, t.EntryPoint.position, t.EntryPoint.rotation, ArrowSize, EventType.Repaint);
    
        Handles.color = Handles.yAxisColor;
        if (t.ExitPoint != null)
            Handles.ArrowHandleCap(0, t.ExitPoint.position, t.ExitPoint.rotation, ArrowSize, EventType.Repaint);

        Debug.Log(t.HasEntrySegment);
    }
}

#endif