using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemies
{
    public class HitBox : MonoBehaviour
    {
        [SerializeField] protected int damage;
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            collision.gameObject.GetComponent<PlayerControler>().TakeDamage(damage);
        }
    }
}