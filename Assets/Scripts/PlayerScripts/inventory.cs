using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// namespace inventoryClass {
	public class inventory : MonoBehaviour {

	    public GameObject itemPrefab;

	    public item[] grid;

	    public itemSlot getItemFromSlot(int x, int y) {
	    	if( transform.childCount >= (x * 10 + y)) {
		    	return transform.GetChild(x * 10 + y).GetComponent<itemSlot>();
		    }
		    return null;
	    }

	    public bool addItem(item item) {
	    	for(int i = 0; i < 2; i++) {
	    		for(int j = 0; j < 10; j++) {

			    	if (getItemFromSlot(i,j).transform.childCount == 0) {

			    		GameObject newItem = Instantiate(itemPrefab, new Vector3(40, 40, 1), Quaternion.identity);
	    				newItem.transform.parent = transform.GetChild(i * 10 + j);
	    				if(i == 1 && !transform.parent.gameObject.GetComponent<inventoryMenu>().isOpen) { 
	    					newItem.GetComponent<Image>().enabled = false; 
	    				}
	    				return true;
			    	}
			    }
	    	}
	    	return false;
	    }
	}
// }