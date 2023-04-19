using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemies;

public class meleeItem: usableItem {
	public PlayerControler player;

	private const float AttackTime = 0.30f;
    private const float AttackHitboxSize = 1f;

    public int damage = 10;


	void Awake() {
		player = GameObject.Find("Player(Clone)").GetComponent<PlayerControler>();

	}

	public override void use() {
		var directionVec = new Vector2(player.directionX, player.directionY);
        var hitResults = new List<Collider2D>();
        var hitFilter = new ContactFilter2D().NoFilter();
        Physics2D.OverlapBox((Vector2)player.transform.position + directionVec, Vector2.one * AttackHitboxSize, 0f, hitFilter, hitResults);

        foreach (var result in hitResults)
        {
            if (!result.CompareTag("Enemy")) continue;

            result.GetComponent<EnemyController>().TakeDamage(damage, player.gameObject);
        }
	}
}