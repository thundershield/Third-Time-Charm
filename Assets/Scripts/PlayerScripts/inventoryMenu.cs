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

    public struct statBlock {
        public int speed;
        public int armor;
    }

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
        public GameObject itemPrefab;

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

        public void updateStatWindow() {
            PlayerControler player = GameObject.Find("Player(Clone)").GetComponent<PlayerControler>();
            TextMeshProUGUI health = transform.Find("PlayerStats/health").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI armor = transform.Find("PlayerStats/armor").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI speed = transform.Find("PlayerStats/speed").GetComponent<TextMeshProUGUI>();

            health.text = "Health: " + player.health.ToString();
            armor.text = "Armor: " + player.armor.ToString();
            speed.text = "Speed: " + player.speed.ToString();
        }

        public void updateStats() {
            statBlock playerStats;
            playerStats.armor = 0;
            playerStats.speed = 0;

            GameObject statGrid = transform.Find("StatObjects").gameObject; 
            for(int j = 0; j < 20; j++) {
                Transform tile = statGrid.transform.GetChild(j);
                tile.gameObject.GetComponent<Image>().enabled = false;
                if(tile.childCount > 0) {
                    int itemId = tile.GetChild(0).gameObject.GetComponent<item>().itemId;
                    itemInfo item = queryItem(itemId);
                    if(item.statType == "defense") {
                        playerStats.armor += item.statAmount;
                    }
                    else if(item.statType == "speed") {
                        playerStats.speed += item.statAmount;
                    }
                }
            }
            GameObject player = GameObject.Find("Player(Clone)");
            player.GetComponent<PlayerControler>().updateStats(playerStats);
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

            GameObject statGrid = transform.Find("StatObjects").gameObject; 
            statGrid.GetComponent<Image>().enabled = false;
            for(int j = 0; j < 20; j++) {
                Transform tile = statGrid.transform.GetChild(j);
                tile.gameObject.GetComponent<Image>().enabled = false;
                if(tile.childCount > 0) {
                    tile.GetChild(0).gameObject.GetComponent<Image>().enabled = false;
                }
            }

            transform.Find("DropButton").gameObject.SetActive(false);
            transform.Find("PlayerStats").gameObject.SetActive(false);

            if(selected != null) {
                if(!selected.transform.GetChild(0).gameObject.GetComponent<item>().statItem) {
                    selected.GetComponent<Image>().color = new Color32(140,140,140,255);
                }
                else {
                    selected.GetComponent<Image>().color = new Color32(212,212,212,255);
                }   
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
            GameObject statGrid = transform.Find("StatObjects").gameObject; 
            statGrid.GetComponent<Image>().enabled = true;
            for(int j = 0; j < 20; j++) {
                Transform tile = statGrid.transform.GetChild(j);
                tile.gameObject.GetComponent<Image>().enabled = true;
                if(tile.childCount > 0) {
                    tile.GetChild(0).gameObject.GetComponent<Image>().enabled = true;
                }
            }

            transform.Find("PlayerStats").gameObject.SetActive(true);
            updateStatWindow();
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
                if(!selected.transform.GetChild(0).gameObject.GetComponent<item>().statItem) {
                    selected.GetComponent<Image>().color = new Color32(140,140,140,255);
                }
                else {
                    selected.GetComponent<Image>().color = new Color32(212,212,212,255);  
                }
                selected = null;
                deselectItem();
            }
            if(Input.GetKeyDown(KeyCode.Q) && !isOpen) {
                rotateInventory();
            }
            if(Input.GetKeyDown(KeyCode.V)) {
                dropItem();
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

            transform.Find("DropButton").gameObject.SetActive(true);
            // itemPopup.position = selected.transform.position + new Vector3(220,45-(itemPopup.rect.height/2),0);
        }

        public void deselectItem() {
            Transform itemPopup = transform.Find("itemPopup");
            itemPopup.gameObject.SetActive(false);
            transform.Find("DropButton").gameObject.SetActive(false);
        }

        public void dropItem() {
            if(selected != null) {
                Destroy(selected.transform.GetChild(0).gameObject);

                item selectedItem = selected.transform.GetChild(0).GetComponent<item>();
                if(!selected.transform.GetChild(0).gameObject.GetComponent<item>().statItem) {
                    selected.GetComponent<Image>().color = new Color32(140,140,140,255);
                }
                else {
                    selected.GetComponent<Image>().color = new Color32(212,212,212,255);   
                }
                selected = null;
                deselectItem();

                Transform playerPosition = GameObject.Find("Player(Clone)").transform;
                GameObject item = Instantiate(itemPrefab, playerPosition.position, Quaternion.identity);
                item.GetComponent<itemObject>().updateItem(selectedItem.itemId);

                updateStats();
            }
        }
    }
// }