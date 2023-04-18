using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rangedItem: usableItem {
	public PlayerControler player;
    public int damage = 10;

    public GameObject arrowPrefab;

	void Awake() {
		player = GameObject.Find("Player(Clone)").GetComponent<PlayerControler>();
		arrowPrefab = GameObject.Find("Inventory").GetComponent<inventoryMenu>().projectilePrefab;
	}

	public override void use() {
		Vector3 worldPosition = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position).normalized;
		GameObject arrow = Instantiate(arrowPrefab, player.transform.position + worldPosition * 2, Quaternion.identity);
		arrow.AddComponent(typeof(arrow));
		arrow.GetComponent<Rigidbody2D>().AddForce(Vector3.Normalize(worldPosition) * 50, ForceMode2D.Impulse);
	}
}