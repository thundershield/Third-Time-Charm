using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class dropButton: MonoBehaviour, IPointerDownHandler {
	public inventoryMenu inventory;

	public void OnPointerDown(PointerEventData eventData) {
		inventory.clickingItem = true;
	}

}