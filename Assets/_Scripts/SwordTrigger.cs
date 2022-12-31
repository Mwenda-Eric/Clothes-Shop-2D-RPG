using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTrigger : MonoBehaviour
{
    private readonly int _swordDamage = 10;
    private BoxCollider2D _swordCollider;

    private void Start()
    {
        _swordCollider = GetComponent<BoxCollider2D>();
        _swordCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            //Debug.Log("Give Slime Damage!");
            col.GetComponent<Enemy>().TakeDamage(_swordDamage);
        }
    }

    public void EnableSwordCollider(bool enableStatus)
    {
        _swordCollider.enabled = enableStatus;
    }
}
