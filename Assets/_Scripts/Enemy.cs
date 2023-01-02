using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    //Script attached to the Slime Enemy.
    
    private Animator _enemyAnimator;
    private float _enemyHealth = 100f;
    private static readonly int SlimeDeathTrigger = Animator.StringToHash("SlimeDeath");
    private static readonly int SlimeJumpFarHash = Animator.StringToHash("slime_jump_far");
    private static readonly int SlimeJumpLowHash = Animator.StringToHash("slime_jump_low");
    private static readonly int SlimeIdleHash = Animator.StringToHash("slime_idle");
    private static readonly int SlimeUpAndDownHash = Animator.StringToHash("slime_up_down");
    private float EnemyHealth
    {
        set
        {
            _enemyHealth = value;
            if (_enemyHealth <= 0)
            {
                EnemyDefeated();
            }
        }
        get => _enemyHealth;
    }

    private void Start()
    {
        _enemyAnimator = GetComponent<Animator>();
        int[] randomAnimationId = { SlimeIdleHash, SlimeJumpFarHash, SlimeJumpFarHash, SlimeUpAndDownHash, SlimeJumpLowHash };
        _enemyAnimator.Play(randomAnimationId[Random.Range(0, randomAnimationId.Length)]);
    }

    public void TakeDamage(float damageAmount)
    {
        EnemyHealth -= damageAmount;
    }
    private void EnemyDefeated()
    {
        _enemyAnimator.SetTrigger(SlimeDeathTrigger);
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
