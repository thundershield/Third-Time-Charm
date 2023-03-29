using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// namespace inventoryClass {
	public class itemSlot: MonoBehaviour, IDropHandler {
		
		public void OnDrop(PointerEventData eventData) {
			if (transform.childCount == 0) {
				GameObject dropped = eventData.pointerDrag;
				item droppedItem = dropped.GetComponent<item>();
				droppedItem.finalPosition = transform;
			}
		}
	}		
// }
