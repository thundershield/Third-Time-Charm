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
        public bool clickingItem = false;

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

            deselectItem();
            closeInventory();
        }

        public void closeInventory() {
            transform.Find("Background").localScale -= new Vector3(0.0f,0.77f,0.0f); 
                transform.Find("Background").position += new Vector3(0.0f,215f,0.0f); 

                isOpen = false;
                // Grid
                GameObject grid = transform.Find("Grid").gameObject; 
                for(int j = 0; j < 6; j++) {
                    Transform tile = grid.transform.GetChild(6+j);
                    tile.gameObject.GetComponent<Image>().enabled = false;
                    if(tile.childCount > 0) {
                        tile.GetChild(0).gameObject.GetComponent<Image>().enabled = false;
                    }
                }

                transform.Find("StatObjects").gameObject.SetActive(false);
                transform.Find("PlayerStats").gameObject.SetActive(false);

                if(selected != null) {
                    selected.GetComponent<Image>().color = new Color32(140,140,140,255);
                    selected = null;
                    deselectItem();
                }
        }

        public void openInventory() {
            transform.Find("Background").localScale += new Vector3(0.0f,0.77f,0.0f); 
            transform.Find("Background").position -= new Vector3(0.0f,215f,0.0f); 

            isOpen = true;
            // Grid
            GameObject grid = transform.Find("Grid").gameObject; 
            for(int j = 0; j < 6; j++) {
                Transform tile = grid.transform.GetChild(6+j);
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

        public void rotateInventory() {
            GameObject grid = transform.Find("Grid").gameObject;
            for(int j = 0; j < 10; j++) {
                Transform topSlot = grid.transform.GetChild(j);
                Transform bottomSlot = grid.transform.GetChild(10+j);

                if(topSlot.childCount > 0 && bottomSlot.childCount > 0) {
                    Transform topTile = topSlot.transform.GetChild(0);
                    Transform bottomTile = bottomSlot.transform.GetChild(0);
                    bottomTile.gameObject.GetComponent<Image>().enabled = true;
                    topTile.gameObject.GetComponent<Image>().enabled = false;
                    topTile.SetParent(bottomSlot);
                    bottomTile.SetParent(topSlot);
                }
                else if(topSlot.childCount > 0) {
                    Transform topTile = topSlot.transform.GetChild(0);
                    topTile.gameObject.GetComponent<Image>().enabled = false;
                    topTile.SetParent(bottomSlot);  
                }
                else if(bottomSlot.childCount > 0) {
                    Transform bottomTile = bottomSlot.transform.GetChild(0);
                    bottomTile.gameObject.GetComponent<Image>().enabled = true;
                    bottomTile.SetParent(topSlot);
                }

            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            {
                closeInventory();
            }
            else if(Input.GetKeyDown(KeyCode.Escape) && !isOpen) {
                openInventory();
            }
            if(Input.GetMouseButtonDown(0) && selected != null && !clickingItem) {
                selected.GetComponent<Image>().color = new Color32(140,140,140,255);
                selected = null;
                deselectItem();
            }
            else if(Input.GetKeyDown(KeyCode.Q) && !isOpen) {
                rotateInventory();
            }
        }

        public void selectItem(int itemId) {
            RectTransform itemPopup = (RectTransform)transform.Find("itemPopup");

            itemPopup.gameObject.SetActive(true);

            itemInfo itemStats = queryItem(itemId);
            Transform name = itemPopup.Find("TextGroup").Find("itemName");
            name.gameObject.GetComponent<TextMeshProUGUI>().text = itemStats.name;
            Transform text = itemPopup.Find("TextGroup").Find("itemText");
            text.gameObject.GetComponent<TextMeshProUGUI>().text = itemStats.flavorText;
            Transform stats = itemPopup.Find("TextGroup").Find("itemStats");
            stats.gameObject.GetComponent<TextMeshProUGUI>().text = itemStats.statDescriptiom;

            Canvas.ForceUpdateCanvases();
            itemPopup.gameObject.GetComponent<VerticalLayoutGroup>().enabled = false;
            itemPopup.gameObject.GetComponent<VerticalLayoutGroup>().enabled = true;
            itemPopup.gameObject.GetComponent<ContentSizeFitter>().enabled = false;
            itemPopup.gameObject.GetComponent<ContentSizeFitter>().enabled = true;
            itemPopup.ForceUpdateRectTransforms();

            // itemPopup.position = selected.transform.position + new Vector3(220,45-(itemPopup.rect.height/2),0);
        }

        public void deselectItem() {
            Transform itemPopup = transform.Find("itemPopup");
            itemPopup.gameObject.SetActive(false);
        }
    }
// }