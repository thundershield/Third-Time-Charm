using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{

	private LevelGeneration.Map map;
	public GameObject player;
	public GameObject camera;
	// public GameObject inventory;

	IEnumerator Start() 
	{
		map = GameObject.Find("Tilemap").GetComponent<Tilemap>().GetComponent<LevelGeneration.Map>();

		yield return new WaitUntil(() => map.isDone);

		Vector2 start = map.getStartLocation();
		GameObject newPlayer = Instantiate(player, new Vector3(start.x, start.y, 1), Quaternion.identity);
		GameObject playerCamera = Instantiate(camera, new Vector3(start.x, start.y, -10), Quaternion.identity);
		PlayerCamera pCam = playerCamera.GetComponent<PlayerCamera>();
		pCam.player = newPlayer;
		pCam.startGame();
		// player.GameController = this.gameObject;
		// inventory = Instantiate(inventory);
		// inventory.player = player;
	}
}
