using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopkeeperScript : MonoBehaviour
{
    private Transform _transform;
    private PlayerController _playerController;
    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
        _playerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerController.PlayerTransform.position.x > _transform.position.x)
        {
            _transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameManager.Instance.EnableOutfitSelectionPanel();
            _playerController.LockMovement();
        }
    }
}
