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

    [NonSerialized] public List<Segment> Segments = new ();
    
    public EnvironmentData CurrentEnvironment => _environments[EnvironmentIndex];

    public void Awake()
    {
        
    }

    public void ExpandSegment(Segment segment)
    {
        foreach (Transform exit in segment.ExitPoints)
        {
            EnvironmentData.SegmentProfile profile = CurrentEnvironment.RandomProfile();
            GameObject prefab = profile.Prefab;
            Vector3 worldExitPos = exit.position;
            Vector3 localEntryPos = profile.Segment.EntryPoints[Random.Range(0, profile.Segment.EntryPoints.Length)].position;
            Vector3 localPrefabPos = prefab.transform.position;

            Vector3 worldPrefabPos = worldExitPos + localPrefabPos - localEntryPos;
            
            GameObject newPrefab = Instantiate(prefab, worldPrefabPos, exit.rotation);
            Segments.Add(newPrefab.GetComponent<Segment>());
        }
    }
}
