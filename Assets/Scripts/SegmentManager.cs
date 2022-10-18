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

    private Segment _currentSegment;
    public Segment CurrentSegment
    {
        get => _currentSegment;
        private set
        {
            _currentSegment = value;
            PrewarmSegment(_currentSegment);
        }
    }

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
    }

    public void MoveSegments(Vector3 motion)
    {
        OnMoveSegments?.Invoke(motion);
    }

    public Segment AppendSegment()
    {
        Segment segment = _activeSegments.Count <= 0 ? _spawnSegmentPrefab.GetComponent<Segment>() : CurrentEnvironment.RandomSegment();
        return AppendSegment(segment);
    }

    public Segment AppendSegment(Segment segment)
    {
        Segment newSegment;
        if (_activeSegments.Count <= 0)
        {
            Vector3 worldPrefabPos = segment.transform.position;
            Quaternion worldPrefabRot = Quaternion.identity;

            GameObject newPrefab = Instantiate(segment.gameObject, worldPrefabPos, worldPrefabRot);
            newSegment = newPrefab.GetComponent<Segment>();
        }
        else
        {
            Segment lastSegment = _activeSegments[_activeSegments.Count - 1];

            Vector3 worldExitPos = lastSegment.ExitPoint.position;
            Vector3 localEntryPos = segment.EntryPoint.position;
            Vector3 localPrefabPos = segment.transform.position;
            Vector3 worldPrefabPos = worldExitPos + localPrefabPos - localEntryPos;
            Quaternion worldPrefabRot = lastSegment.ExitPoint.rotation;

            GameObject newPrefab = Instantiate(segment.gameObject, worldPrefabPos, worldPrefabRot);
            newSegment = newPrefab.GetComponent<Segment>();

            ConnectSegments(lastSegment, newSegment);
        }

        _activeSegments.Add(newSegment);

        return newSegment;
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
        while (parentSegment.HasEntrySegment)
        {
            Segment childSegment = parentSegment;
            parentSegment = parentSegment.EntrySegment;

            _activeSegments.Remove(childSegment);
            Destroy(childSegment.gameObject);
        }

        _activeSegments.Remove(parentSegment);
        Destroy(parentSegment.gameObject);
    }

    public void PrewarmSegment(Segment segment)
    {
        Segment nextSegment = segment;
        for (int i = 0; i < PrewarmCount; i++)
        {
            if (nextSegment.HasExitSegment)
            {
                nextSegment = nextSegment.ExitSegment;
                continue;
            }

            Debug.Log("Adding another segment.");
            nextSegment = AppendSegment();
        }
    }

    private static void ConnectSegments(Segment entry, Segment exit)
    {
        entry.ExitSegment = exit;
        exit.EntrySegment = entry;
    }
}
