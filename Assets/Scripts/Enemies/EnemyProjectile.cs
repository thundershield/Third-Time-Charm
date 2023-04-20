using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemies
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] protected int damage;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")){
                collision.gameObject.GetComponent<PlayerControler>().TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
