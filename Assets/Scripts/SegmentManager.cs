using System;
using System.Collections.Generic;
using UnityEngine;

public class SegmentManager : MonoBehaviour
{
    [SerializeField] [Min(1)] private int _prewarmCount = 1;
    //[SerializeField] [Min(1)] private float _prewarmDistance = 200f;
    [SerializeField] private GameObject _spawnSegmentPrefab;
    [SerializeField] private EnvironmentProfile[] _environmentProfiles;

    public static event Action<Vector3> OnMoveSegments;

    [NonSerialized] public int EnvironmentIndex;

    private List<Segment> _activeSegments = new List<Segment>();

    private SegmentManagerState _managerState;

    private bool _finalEnvironment;
    
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
    
    public float DistanceGenerated { get; private set; }
    public float DistanceSinceEnvironmentStart { get; private set; }
    public float SegmentsPassed { get; private set; }
    public float EnvironmentsPassed { get; private set; }
    
    public EnvironmentData CurrentEnvironment => _environmentProfiles[EnvironmentIndex].Environment;

    [Serializable]
    public class EnvironmentProfile
    {
        public EnvironmentData Environment;
        [Min(0f)] public float Length = 1000f;
    }

    public enum SegmentManagerState
    {
        Enter,
        Repeat,
        Exit
    }
    
    private void Awake()
    {
        SegmentTrigger.OnSegmentEnter += OnSegmentEnter;
    }

    private void OnDisable()
    {
        SegmentTrigger.OnSegmentEnter -= OnSegmentEnter;
    }

    private void OnSegmentEnter(Segment segment)
    {
        Segment oldSegment = CurrentSegment;
        CurrentSegment = segment;
        
        if (oldSegment != null)
        {
            SegmentsPassed++;
            RemoveSegment(oldSegment);
        }
    }

    public void MoveSegments(Vector3 motion)
    {
        OnMoveSegments?.Invoke(motion);
    }

    public Segment AppendSegment()
    {
        Segment segment;
        if (_activeSegments.Count <= 0)
            segment = _spawnSegmentPrefab.GetComponent<Segment>();
        else
            switch (_managerState) // whaaaat iiiis thiiiis
            {
                case SegmentManagerState.Enter:
                    _managerState = DistanceSinceEnvironmentStart >= _environmentProfiles[EnvironmentIndex].Length ? SegmentManagerState.Exit : SegmentManagerState.Repeat;
                    
                    segment = CurrentEnvironment.EntrySegment ? CurrentEnvironment.EntrySegment : CurrentEnvironment.RandomSegment();
                    //DistanceSinceEnvironmentStart -= environmentLength;
                    DistanceSinceEnvironmentStart = 0f; // Make sure overlap doesn't make the next environment shorter.
                    break;

                case SegmentManagerState.Exit:
                    _managerState = SegmentManagerState.Enter;
                    
                    segment = CurrentEnvironment.ExitSegment ? CurrentEnvironment.ExitSegment : CurrentEnvironment.RandomSegment();
                    if (++EnvironmentIndex >= _environmentProfiles.Length)
                    {
                        EnvironmentIndex = _environmentProfiles.Length - 1;
                        _finalEnvironment = true;
                    }
                    break;

                default:
                    if (!_finalEnvironment && DistanceSinceEnvironmentStart >= _environmentProfiles[EnvironmentIndex].Length)
                        _managerState = SegmentManagerState.Exit;
                    
                    segment = CurrentEnvironment.RandomSegment();
                    break;
            }

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
            Vector3 localPrefabPos = segment.transform.position;
            Vector3 localEntryPos = segment.EntryPoint.position - localPrefabPos;
            Quaternion undoRot = Quaternion.Inverse(segment.EntryPoint.rotation) * segment.transform.rotation;
            Vector3 adjustedEntryPos = undoRot * localEntryPos;
            Vector3 worldPrefabPos = worldExitPos - adjustedEntryPos;

            GameObject newPrefab = Instantiate(segment.gameObject, worldPrefabPos, undoRot);
            newSegment = newPrefab.GetComponent<Segment>();

            ConnectSegments(lastSegment, newSegment);
        }

        DistanceGenerated += newSegment.Length;
        DistanceSinceEnvironmentStart += newSegment.Length;

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
            Destroy(childSegment.gameObject, 2f);
        }

        _activeSegments.Remove(parentSegment);
        Destroy(parentSegment.gameObject, 2f);
    }

    public void PrewarmSegment(Segment segment)
    {
        Segment nextSegment = segment;
        for (int i = 0; i < _prewarmCount; i++)
        {
            if (nextSegment.HasExitSegment)
            {
                nextSegment = nextSegment.ExitSegment;
                continue;
            }

            nextSegment = AppendSegment();
        }
    }

    private static void ConnectSegments(Segment entry, Segment exit)
    {
        entry.ExitSegment = exit;
        exit.EntrySegment = entry;
    }
}
