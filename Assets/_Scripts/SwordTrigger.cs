using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTrigger : MonoBehaviour
{
    public static int SwordDamage = 10;
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
            col.GetComponent<Enemy>().TakeDamage(SwordDamage);
        }
    }

    public void EnableSwordCollider(bool enableStatus)
    {
        _swordCollider.enabled = enableStatus;
    }
}
