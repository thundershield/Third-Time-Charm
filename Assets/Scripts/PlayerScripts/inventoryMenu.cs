using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;

// namespace inventoryClass {
    public struct itemInfo {
            public int id;
            public string name;
            public string flavorText;
            public string statDescriptiom;
            public string itemType;
            public string statType;
            public int statAmount;
            public string sprite;
    };

    public class inventoryMenu : MonoBehaviour {

        public bool isOpen = true;

        private Dictionary<int, itemInfo> items = new Dictionary<int, itemInfo>();

        public itemInfo queryItem(int id) {
            if(items.ContainsKey(id)) {
                return items[id];
            }
            return items[0];
        }

        public GameObject selected;

        public void Awake() {
            TextAsset dataset = Resources.Load<TextAsset>("items");

            string[] splitDataset = dataset.text.Split(new char[] {'\n'});
     
            for (var i = 1; i < splitDataset.Length; i++) {
                string[] row = splitDataset[i].Split(new char[] {','});
                if(row.Length >= 8) {
                    itemInfo newItem = new itemInfo();
                    newItem.id = Int32.Parse(row[0]);
                    newItem.name = row[1];
                    newItem.flavorText = row[2];
                    newItem.statDescriptiom = row[3];
                    newItem.itemType = row[4];
                    newItem.statType = row[5];
                    newItem.statAmount = Int32.Parse(row[6]);
                    newItem.sprite = row[7];
                    items.Add(Int32.Parse(row[0]), newItem);
                }
            }
        }

        public void hidePopup() {
            Transform itemPopup = transform.Find("itemPopup");
            itemPopup.gameObject.GetComponent<Image>().enabled = false;
            Transform textGroup = itemPopup.Find("TextGroup");
            textGroup.gameObject.GetComponent<Image>().enabled = true;
            textGroup.Find("itemName").gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
            textGroup.Find("itemText").gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
            textGroup.Find("itemStats").gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            {
                transform.Find("Background").localScale -= new Vector3(0.0f,0.77f,0.0f); 
                transform.Find("Background").position += new Vector3(0.0f,215f,0.0f); 

                isOpen = false;
                // Grid
                GameObject grid = transform.Find("Grid").gameObject; 
                for(int j = 0; j < 10; j++) {
                    Transform tile = grid.transform.GetChild(10+j);
                    tile.gameObject.GetComponent<Image>().enabled = false;
                    if(tile.childCount > 0) {
                        tile.GetChild(0).gameObject.GetComponent<Image>().enabled = false;
                    }
                }

                transform.Find("StatObjects").gameObject.SetActive(false);
                transform.Find("PlayerStats").gameObject.SetActive(false);

                if(selected != null) {
                    hidePopup();
                }
            }
            else if(Input.GetKeyDown(KeyCode.Escape) && !isOpen) {
                transform.Find("Background").localScale += new Vector3(0.0f,0.77f,0.0f); 
                transform.Find("Background").position -= new Vector3(0.0f,215f,0.0f); 

                isOpen = true;
                // Grid
                GameObject grid = transform.Find("Grid").gameObject; 
                for(int j = 0; j < 10; j++) {
                    Transform tile = grid.transform.GetChild(10+j);
                    tile.gameObject.GetComponent<Image>().enabled = true;
                    if(tile.childCount > 0) {
                        tile.GetChild(0).gameObject.GetComponent<Image>().enabled = true;
                    }
                }

                // Stat Objects
                transform.Find("StatObjects").gameObject.SetActive(true);

                //
                transform.Find("PlayerStats").gameObject.SetActive(true);  
            }
        }

        public void selectItem(int itemId) {
            RectTransform itemPopup = (RectTransform)transform.Find("itemPopup");
            itemPopup.gameObject.GetComponent<Image>().enabled = true;
            Transform textGroup = itemPopup.Find("TextGroup");
            textGroup.gameObject.GetComponent<Image>().enabled = true;
            textGroup.Find("itemName").gameObject.GetComponent<TextMeshProUGUI>().enabled = true; 
            textGroup.Find("itemText").gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
            textGroup.Find("itemStats").gameObject.GetComponent<TextMeshProUGUI>().enabled = true;

            itemInfo itemStats = queryItem(itemId);
            itemPopup.position = selected.transform.position + new Vector3(220,45-(itemPopup.rect.height/2),0);
            Transform name = itemPopup.Find("TextGroup").Find("itemName");
            name.gameObject.GetComponent<TextMeshProUGUI>().text = itemStats.name;
            Transform text = itemPopup.Find("TextGroup").Find("itemText");
            name.gameObject.GetComponent<TextMeshProUGUI>().text = itemStats.flavorText;
            Transform stats = itemPopup.Find("TextGroup").Find("itemStats");
            name.gameObject.GetComponent<TextMeshProUGUI>().text = itemStats.statDescriptiom;

        }

        public void deselectItem() {
            hidePopup();   
        }

    }
// }