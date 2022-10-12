using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Segment : MonoBehaviour
{
    public Transform[] EntryPoints;
    public Transform[] ExitPoints;

    [HideInInspector] public Vector3[] PathPoints = new Vector3[0];
    
    public Prop[] Props;
}