using UnityEngine;

public class GameController : MonoBehaviour
{
	public GameObject itemPrefab;

	private int numItems = 6;

	void Start() 
	{
		// for(int i = 0; i < 500; i++) {
			// var position = new Vector3(Random.Range(1,90)+0.5f, Random.Range(1,70)+0.5f, 0);
			// GameObject item =  Instantiate(itemPrefab, position, Quaternion.identity);
			// item.GetComponent<itemObject>().updateItem(Random.Range(1,numItems));
		// }	
	}

	void Update() {
		
	}

}
