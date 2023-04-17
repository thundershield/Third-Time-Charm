using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// namespace inventoryClass {
	public class inventory : MonoBehaviour {

	    public GameObject itemPrefab;

	    public inventoryMenu mainInventory;

	    public int itemWidth = 6;
	    public int itemHeight = 2;

	    public itemSlot getItemFromSlot(int x, int y) {
	    	if( transform.childCount >= (x * itemWidth + y)) {
		    	return transform.GetChild(x * itemWidth + y).GetComponent<itemSlot>();
		    }
		    return null;
	    }

	    public bool isFull() {
	    	for(int i = 0; i < itemHeight; i++) {
	    		for(int j = 0; j < itemWidth; j++) {
	    			if (getItemFromSlot(i,j).transform.childCount == 0) { return false; }
	    		}
	    	}
	    	return true;
	    }

	    public bool addItem(int id) {
	    	itemInfo info = mainInventory.queryItem(id);
	    	if(info.itemType == "stat") {
	    		GameObject grid = GameObject.Find("StatObjects");
	    		if(!grid) {Debug.Log("NOOO GRID");}
	    		for(int i = 0; i < 20;i++) {
	    			if(grid.transform.GetChild(i).childCount == 0) {
	    				GameObject newItem = Instantiate(itemPrefab, new Vector3(40, 40, 1), Quaternion.identity);
	    				newItem.transform.SetParent(grid.transform.GetChild(i),false);
	    				newItem.GetComponent<item>().statItem = true; 
	    				newItem.GetComponent<item>().updateItem(id);

	    				if(!transform.parent.gameObject.GetComponent<inventoryMenu>().isOpen) { 
		    					newItem.GetComponent<Image>().enabled = false; 
		    			}
		    			mainInventory.updateStats();
		    			mainInventory.updateStatWindow();
	    				return true;
	    			}
	    		}
	    	}
	    	else {
		    	for(int i = 0; i < itemHeight; i++) {
		    		for(int j = 0; j < itemWidth; j++) {
				    	if (getItemFromSlot(i,j).transform.childCount == 0) {

				    		GameObject newItem = Instantiate(itemPrefab, new Vector3(40, 40, 1), Quaternion.identity);
		    				newItem.transform.SetParent(transform.GetChild(i * itemWidth + j),false);
		    				newItem.GetComponent<item>().updateItem(id);

		    				if(i == 1 && !transform.parent.gameObject.GetComponent<inventoryMenu>().isOpen) { 
		    					newItem.GetComponent<Image>().enabled = false; 
		    				}
		    				return true;
				    	}
				    }
		    	}
		    }
	    	return false;
	    }
	}
// }