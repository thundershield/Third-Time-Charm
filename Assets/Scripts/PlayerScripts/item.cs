using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// namespace inventoryClass {
	public class item: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler {
		
		public Image image;
		private inventoryMenu inventory;
		[HideInInspector] public Transform finalPosition;
		public int itemId;

		void Awake() {
			inventory = GameObject.FindGameObjectsWithTag("Inventory")[0].transform.parent.GetComponent<inventoryMenu>();
		}

		public void OnBeginDrag(PointerEventData eventData) {
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
			transform.position = Input.mousePosition;
		}

		public void OnEndDrag(PointerEventData eventData) {
			transform.SetParent(finalPosition);
			image.raycastTarget = true;
		}

		public void OnPointerDown(PointerEventData eventData) {}

		public void OnPointerUp(PointerEventData pointerEventData)
	    {
	    	if(!pointerEventData.dragging) {
		        //Output the name of the GameObject that is being clicked
		        if(inventory.selected != null) {
		        	inventory.selected.GetComponent<Image>().color = new Color32(140,140,140,255);
		        }
		        if(inventory.selected == transform.parent.gameObject) {
		        	inventory.selected.GetComponent<Image>().color = new Color32(140,140,140,255);
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