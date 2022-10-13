using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "EnvironmentData", menuName = "ScriptableObjects/EnvironmentData", order = 1)]
public class EnvironmentData : ScriptableObject
{
    public string Name = "Environment";
    
    public SegmentProfile[] SegmentProfiles;

    public float WeightSum;
    public AnimationCurve WeightCurve;
    
    [Serializable]
    public class SegmentProfile
    {
        [Tooltip("Segment Prefab used for instantiation.")]
        public GameObject Prefab;
        [Tooltip("Segment MonoBehaviour attached to the Segment Prefab.")]
        [ReadOnly] public Segment Segment;
        [Tooltip("Weight used when selecting a random Segment from this Environment. Weights are normalized.")]
        [Range(0f, 1f)] public float Weight = 0.5f;

        public bool Validate()
        {
            if (!Prefab)
            {
                Segment = null;
                return false;
            }

            if (!Prefab.TryGetComponent(out Segment segment))
            {
                Debug.LogError($"Missing Segment MonoBehaviour on {Prefab.name}'s root.", Prefab);

                return false;
            }

            Segment = segment;
            return true;
        }
    }

    private void OnValidate()
    {
        WeightCurve = new AnimationCurve();
        WeightSum = 0f;
        Keyframe key;
        for (int i = 0; i < SegmentProfiles.Length; i++)
        {
            SegmentProfile profile = SegmentProfiles[i];
            float probability = profile.Weight;

            if (probability > 0f)
            {
                key = new Keyframe(WeightSum, i, Mathf.Infinity, Mathf.Infinity);
                WeightCurve.AddKey(key);
            }

            WeightSum += probability;
            
            profile.Validate();
        }
        
        key = new Keyframe(WeightSum, WeightSum > 0f ? SegmentProfiles.Length-1 : -1, Mathf.Infinity, Mathf.Infinity);
        WeightCurve.AddKey(key);
    }

    public int RandomProfileIndex()
    {
        return (int)WeightCurve.Evaluate(Random.Range(0f, 1f));
    }

    public SegmentProfile RandomProfile()
    {
        int profileIndex = RandomProfileIndex();
        return profileIndex >= 0 ? SegmentProfiles[profileIndex] : null;
    }
    
    public GameObject RandomSegmentPrefab()
    {
        int profileIndex = RandomProfileIndex();
        return profileIndex >= 0 ? SegmentProfiles[profileIndex].Prefab : null;
    }
}
