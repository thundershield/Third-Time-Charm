using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemies;

public class arrow: MonoBehaviour {
	public int damage = 10;

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("Enemy")) {
			other.gameObject.GetComponent<EnemyController>().TakeDamage(damage, DamageType.physical, gameObject);
		}
		Destroy(gameObject);
	}
} 