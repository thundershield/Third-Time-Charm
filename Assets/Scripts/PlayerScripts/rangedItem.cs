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
		Vector2 worldPosition = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position);
		worldPosition.Normalize();

		GameObject arrow = Instantiate(arrowPrefab, player.transform.position + new Vector3(worldPosition.x, worldPosition.y,0), Quaternion.identity);
		arrow.AddComponent(typeof(arrow));
		var angle = Mathf.Atan2(worldPosition.y, worldPosition.x) * Mathf.Rad2Deg;
     	arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle+180));
		// Debug.Log(direction);
		arrow.GetComponent<Rigidbody2D>().AddForce(new Vector3(worldPosition.x, worldPosition.y,0) * 15, ForceMode2D.Impulse);
	}
}