using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SegmentManager : MonoBehaviour
{
    public readonly int PrewarmCount = 1;
    [SerializeField] private GameObject _spawnSegmentPrefab;
    [SerializeField] private EnvironmentData[] _environments;
    [NonSerialized] public int EnvironmentIndex;
    
    public static event Action<Vector3> OnMoveSegments;

    private List<Segment> _activeSegments = new List<Segment>();

    public Segment CurrentSegment { get; private set; }
    public EnvironmentData CurrentEnvironment => _environments[EnvironmentIndex];
    
    public float DistanceGenerated { get; private set; }

    private void Awake()
    {
        SegmentTrigger.OnSegmentEnter += OnSegmentEnter;
    }

    private void OnDisable()
    {
        SegmentTrigger.OnSegmentEnter -= OnSegmentEnter;
    }

    int segmentsPassed = -1;
    private void OnSegmentEnter(Segment segment)
    {
        Debug.Log("Segment: " + ++segmentsPassed);
        Segment oldSegment = CurrentSegment;
        CurrentSegment = segment;
        
        if (oldSegment != null)
        {
            RemoveSegment(oldSegment);
        }

        for (int i = 0; i < PrewarmCount; i++)
        {
            AppendSegment();
        }
    }

    public void MoveSegments(Vector3 motion)
    {
        OnMoveSegments?.Invoke(motion);
    }

    public void AppendSegment()
    {
        Segment segment = _activeSegments.Count <= 0 ? _spawnSegmentPrefab.GetComponent<Segment>() : CurrentEnvironment.RandomSegment();
        AppendSegment(segment);
    }

    public void AppendSegment(Segment segment)
    {
        Vector3 worldPrefabPos;
        Quaternion worldPrefabRot;
        if (_activeSegments.Count <= 0)
        {
            worldPrefabPos = segment.transform.position;
            worldPrefabRot = Quaternion.identity;
        }
        else
        {
            Segment lastSegment = _activeSegments[_activeSegments.Count - 1];

            Vector3 worldExitPos = lastSegment.ExitPoint.position;
            Vector3 localEntryPos = segment.EntryPoint.position;
            Vector3 localPrefabPos = segment.transform.position;
            worldPrefabPos = worldExitPos + localPrefabPos - localEntryPos;
            worldPrefabRot = lastSegment.ExitPoint.rotation;
        }

        GameObject newPrefab = Instantiate(segment.gameObject, worldPrefabPos, worldPrefabRot);
        Segment newSegment = newPrefab.GetComponent<Segment>();

        _activeSegments.Add(newSegment);
    }

    public void RemoveSegment()
    {
        if (_activeSegments.Count <= 0)
            return;

        RemoveSegment(_activeSegments[0]);
    }

    public void RemoveSegment(Segment segment)
    {
        if (_activeSegments.Count <= 0)
            return;

        // Better to loop over entries or remove everything before index?
        Segment parentSegment = segment;
        while (parentSegment.HasEntry)
        {
            Segment childSegment = parentSegment;
            parentSegment = parentSegment.EntrySegment;

            _activeSegments.Remove(childSegment);
            Destroy(childSegment.gameObject);
        }

        _activeSegments.Remove(parentSegment);
        Destroy(parentSegment.gameObject);
    }
}
