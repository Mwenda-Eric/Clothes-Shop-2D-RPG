using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private const int ChestCoinAmount = 100;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameManager.Instance.AddPlayerCoins(ChestCoinAmount);
            //Destroy(gameObject);
        }
    }
}
