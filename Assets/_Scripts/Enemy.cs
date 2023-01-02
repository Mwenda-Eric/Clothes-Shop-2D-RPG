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
    private PlayerController _player;
    private float _followSpeed = 0.5f;
    private float _radiusVision = 1f;
    private float _playerDistance;
    private float _enemyDamage = 10f;
    private float _knockBackForce = 5f;
    
    #region Animator Hashes
    private static readonly int SlimeDeathTrigger = Animator.StringToHash("SlimeDeath");
    private static readonly int SlimeJumpFarHash = Animator.StringToHash("slime_jump_far");
    private static readonly int SlimeJumpLowHash = Animator.StringToHash("slime_jump_low");
    private static readonly int SlimeIdleHash = Animator.StringToHash("slime_idle");
    private static readonly int SlimeUpAndDownHash = Animator.StringToHash("slime_up_down");
    #endregion
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
        _player = FindObjectOfType<PlayerController>();
        _enemyAnimator = GetComponent<Animator>();
        int[] randomAnimationId = { SlimeIdleHash, SlimeJumpFarHash, SlimeJumpFarHash, SlimeUpAndDownHash, SlimeJumpLowHash };
        _enemyAnimator.Play(randomAnimationId[Random.Range(0, randomAnimationId.Length)]);
    }

    private void Update()
    {
        _playerDistance = Vector2.Distance(transform.position, _player.transform.position);
        if (_playerDistance < _radiusVision)
        {
            transform.position = Vector2.MoveTowards(transform.position, 
                _player.transform.position, _followSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("I'm Enemy Triggered by : " + col.name);
        if (col.CompareTag("Player"))
        {
            //Give the player damage.
            _player.ReceiveDamage(_enemyDamage);
            Debug.Log(GameManager.RedConsole("Player Health = " + _player.PlayerHealth));
            
        }

        if (col.CompareTag("Sword"))
        {
            //Receive KnockBack when player attacks.
            Vector2 direction = (transform.position - col.transform.position).normalized;
            Vector2 knockBack = direction * _knockBackForce;

            GetComponent<Rigidbody2D>().AddForce(knockBack, ForceMode2D.Impulse);
        }
    }

    private void ReceiveKnockBack()
    {
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
