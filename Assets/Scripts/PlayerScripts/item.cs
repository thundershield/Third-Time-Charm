using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// namespace inventoryClass {
	public class item: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler {
		
		public Image image;
		private inventoryMenu inventory;
		[HideInInspector] public Transform finalPosition;
		public int itemId;
		public bool statItem;

		void Awake() {
			inventory = GameObject.FindGameObjectsWithTag("Inventory")[0].transform.parent.GetComponent<inventoryMenu>();
		}

		public void updateItem(int newId) {
			itemInfo info = inventory.queryItem(newId);
			itemId = newId;
		}

		public void OnBeginDrag(PointerEventData eventData) {
			if(statItem) { return; }
			if(inventory.selected != null) {
		        	inventory.selected.GetComponent<Image>().color = new Color32(140,140,140,255);
		        	inventory.selected = null;
                    inventory.deselectItem();
		    }

			finalPosition = transform.parent;
			transform.SetParent(transform.root);
			transform.SetAsLastSibling();
			image.raycastTarget = false;
		}
		
		public void OnDrag(PointerEventData eventData) {
			if(statItem) { return; }
			transform.position = Input.mousePosition;
		}

		public void OnEndDrag(PointerEventData eventData) {
			if(statItem) { return; }
			transform.SetParent(finalPosition);
			image.raycastTarget = true;
		}

		public void OnPointerDown(PointerEventData eventData) {
			inventory.clickingItem = true;
		}

		public void OnPointerUp(PointerEventData pointerEventData)
	    {	
	    	inventory.clickingItem = false;
	    	if(!pointerEventData.dragging && inventory.isOpen) {
		        if(inventory.selected != null) {
		        	if(!statItem) {
			        	inventory.selected.GetComponent<Image>().color = new Color32(140,140,140,255);
			        }
			        else {
			        	inventory.selected.GetComponent<Image>().color = new Color32(212,212,212,255);	
			        }
		        }
		        if(inventory.selected == transform.parent.gameObject) {
		        	if(!statItem) {
		        	inventory.selected.GetComponent<Image>().color = new Color32(140,140,140,255);
		        	}
		        	else {
		        		inventory.selected.GetComponent<Image>().color = new Color32(212,212,212,255);	
		        	}
		        	inventory.selected = null;
                    inventory.deselectItem();
		        }
		        else {
			        inventory.selected = transform.parent.gameObject;
			        transform.parent.gameObject.GetComponent<Image>().color = new Color32(250, 241, 122, 255);
                    inventory.selectItem(itemId);
			    }
			}
	    }
	}
// }