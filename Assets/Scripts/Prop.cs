using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Prop : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] private float _chance = 0.5f;
    [SerializeField] private PropDependency[] _dependencies;

    [System.Serializable]
    private struct PropDependency
    {
        public Prop Prop;
        public bool State;
    }

    private bool _isInitialized;

    private float _value;
    
    public float Value
    {
        get
        {
            if (!_isInitialized)
            {
                _value = Random.Range(0f, 1f);
            }
            
            return _value;
        }
    }

    private void Awake()
    {
        if (_chance == 0f)
            Debug.LogWarning($"{this} spawn chance is 0%", this);
        else if (_chance == 1f)
            Debug.LogWarning($"{this} spawn chance is 100%", this);
    }
}
