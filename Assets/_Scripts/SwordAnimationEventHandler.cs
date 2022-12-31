using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimationEventHandler : MonoBehaviour
{
    private PlayerController _playerController;
    private SwordTrigger _swordTrigger;

    private void Start()
    {
        //Cache Player Controller reference.
        _playerController = FindObjectOfType<PlayerController>();
        _swordTrigger = FindObjectOfType<SwordTrigger>();
    }

    public void LockMovement()//Called by first Keyframe of Sword Attack Animation event.
    {
        _playerController.LockMovement();
        _swordTrigger.EnableSwordCollider(true);
    }

    public void UnlockMovement()//Called by last Keyframe of sword attack animation event.
    {
        _playerController.UnlockMovement();
        _swordTrigger.EnableSwordCollider(false);
    }
}
