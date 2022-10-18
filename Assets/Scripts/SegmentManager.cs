using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SegmentManager : MonoBehaviour
{
    public readonly int PrewarmCount = 1;
    [SerializeField] private EnvironmentData[] _environments;
    [NonSerialized] public int EnvironmentIndex;
    
    public static event Action<Vector3> OnMoveSegments;
    
    public Segment CurrentSegment { get; private set; }
    public EnvironmentData CurrentEnvironment => _environments[EnvironmentIndex];
    
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

        // TODO: Figure out a better way to expand child-segments.
        List<Segment> segmentsToExpand = new List<Segment>{segment};
        for (int i = 0; i < PrewarmCount; i++)
        {
            List<Segment> newSegmentsToExpand = new List<Segment>();
            for (int j = 0; j < segmentsToExpand.Count; j++)
            {
                Segment currentSegment = segmentsToExpand[j];
                newSegmentsToExpand.AddRange(ExpandSegment(currentSegment));
            }
            
            segmentsToExpand = newSegmentsToExpand;
        }
    }

    public void MoveSegments(Vector3 motion)
    {
        OnMoveSegments?.Invoke(motion);
    }
    
    public Segment[] ExpandSegment(Segment segment)
    {
        if (segment.IsExpanded)
            return segment.ExitSegments;
            
        segment.ExitSegments = new Segment[segment.ExitPoints.Length];
        for (int i = 0; i < segment.ExitPoints.Length; i++)
        {
            Transform exit = segment.ExitPoints[i];
            EnvironmentData.SegmentProfile profile = CurrentEnvironment.RandomProfile();
            GameObject prefab = profile.Prefab;
            Vector3 worldExitPos = exit.position;
            Vector3 localEntryPos = profile.Segment.EntryPoints[Random.Range(0, profile.Segment.EntryPoints.Length)].position;
            Vector3 localPrefabPos = prefab.transform.position;
            Vector3 worldPrefabPos = worldExitPos + localPrefabPos - localEntryPos;
            
            GameObject newPrefab = Instantiate(prefab, worldPrefabPos, exit.rotation);
            Segment newSegment = newPrefab.GetComponent<Segment>();

            segment.ExitSegments[i] = newSegment;
        }

        segment.IsExpanded = true;
        return segment.ExitSegments;
    }

    public void RemoveSegment(Segment segment)
    {
        for (int i = 0; i < segment.ExitSegments.Length; i++)
        {
            Segment childSegment = segment.ExitSegments[i];
            if (childSegment != CurrentSegment)
                RemoveSegment(childSegment);
        }

        Destroy(segment.gameObject);
    }
}
