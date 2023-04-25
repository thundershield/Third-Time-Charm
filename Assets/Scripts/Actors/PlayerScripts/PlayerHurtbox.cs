using System;
using UnityEngine;

public class PlayerHurtbox : MonoBehaviour
{
    private const int DamagePerHit = 20;
    
    [SerializeField] private PlayerControler _playerController;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        _playerController.TakeDamage(DamagePerHit);
    }
}
