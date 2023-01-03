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
        if (col.CompareTag("Player"))
        {
            _chestAnimator.SetBool(ChestOpenHash, true);
            GameManager.Instance.AddPlayerCoins(ChestCoinsAmount, true);
            GetComponent<BoxCollider2D>().enabled = false;
            //Destroy(gameObject, 1f);
        }
    }

    public void DestroyThisObjectFromAnimationEvent()//called from last keyframe of chest open animation.
    {
        Debug.Log(GameManager.RedConsole("Called From Chest Animation Event."));
        GameManager.Instance.AddPlayerCoins(ChestCoinsAmount, true);
        Destroy(gameObject, 1f);
    }
}
