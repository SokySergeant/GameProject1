using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PickupEnergy : PickupAbstract
{
    private bool _hasAnimator;
    private Animator _animator;

    public float energyGained = 50f;

    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        
        if (_hasAnimator)
            _animator.SetFloat("Random", Random.Range(-1f, 1f));
    }

    public override void DoPickupAction(GameManager gameManager){

        gameManager.playerMovement.currentEnergy += energyGained;

        if (_hasAnimator)
        {
            _animator.SetTrigger("OnPickup");
        }
        else
            Destroy(gameObject);
    }

    private void OnAnimationEnd()
    {
        Destroy(gameObject);
    }
}
