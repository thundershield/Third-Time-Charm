using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class potion: usableItem {
	public PlayerControler player;
    public int health = 10;

	void Awake() {
		player = GameObject.Find("Player(Clone)").GetComponent<PlayerControler>();
	}

	public override void use() {
		if(player.health > 100 - health) {
			player.health = 100;
		}
		else {
			player.health += health;
		}
		Destroy(player.playerInventory.mainInventory.currentItem);
		GameObject grid = GameObject.Find("Grid").gameObject; 
		GameObject potion = grid.transform.GetChild(player.playerInventory.mainInventory.hotBarSelected - 1).GetChild(0).gameObject;
		Destroy(potion);
		player.playerInventory.mainInventory.currentItem = null;
		player.playerInventory.mainInventory.currentUsableItem = null;
	}
}