using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    public GameObject chestDialogBox;
    public Text npcName;
    public Text chestDialogText;
    public string chestNameString;
    public string chestTextString;
    public bool chestTriggered;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && chestTriggered)
        {
            if(chestDialogBox.activeInHierarchy)
            {
                chestDialogBox.SetActive(false);
            }
            else
            {
                chestDialogBox.SetActive(true);
                npcName.text = chestNameString;
                chestDialogText.text = chestTextString;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("player triggered chest");
            chestTriggered = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("player left chest area");
            chestTriggered = false;
            chestDialogBox.SetActive(false);
        }
    }
}
