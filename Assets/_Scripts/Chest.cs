using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private const int ChestCoinsAmount = 100;
    private Animator _chestAnimator;
    private static readonly int ChestOpenHash = Animator.StringToHash("ChestOpen");

    private void Start()
    {
        _chestAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Sword"))
        {
            _chestAnimator.SetBool(ChestOpenHash, true);
            Debug.Log("Player Opened Chest.");
            GameManager.Instance.AddPlayerCoins(ChestCoinsAmount);
            Destroy(gameObject);
        }
    }

    public void DestroyThisObjectFromAnimationEvent()//called from last keyframe of chest open animation.
    {
        GameManager.Instance.AddPlayerCoins(ChestCoinsAmount);
        Destroy(gameObject);
    }
}
