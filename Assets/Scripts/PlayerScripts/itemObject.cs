using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class itemObject: MonoBehaviour {
	public int id;
	public string name;

	private inventoryMenu inventory;

	void Awake() {
		inventory = GameObject.FindGameObjectsWithTag("Inventory")[0].transform.parent.GetComponent<inventoryMenu>();
	}


	public void updateItem(int newId) {
		itemInfo info = inventory.queryItem(newId);
		id = newId;
		name = info.name;
		if (string.Compare(info.sprite.Substring(0,4),"none") == 0) {
			GameObject text = new GameObject("name");
			text.AddComponent(typeof(TextMeshPro));
			text.GetComponent<TextMeshPro>().fontSize = 3;
			text.GetComponent<TextMeshPro>().text = info.name.Substring(0,5);
			text.GetComponent<MeshRenderer>().sortingOrder = 2;
			text.GetComponent<RectTransform>().sizeDelta = new Vector2(1,1);
			text.transform.SetParent(gameObject.transform.root);
			text.transform.localPosition = new Vector3(0,0,0);
		}
	}
}