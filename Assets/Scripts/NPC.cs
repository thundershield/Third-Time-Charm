using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameObject npcDialogBox;
    public Text npcName;
    public Text npcDialogText;
    public string npcNameString;
    public string npcTextString;
    public bool npcTriggered;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && npcTriggered)
        {
            if(npcDialogBox.activeInHierarchy)
            {
                npcDialogBox.SetActive(false);
            }
            else
            {
                npcDialogBox.SetActive(true);
                npcName.text = npcNameString;
                npcDialogText.text = npcTextString;
                
                
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("player triggered npc");
            npcTriggered = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("player left npc area");
            npcTriggered = false;
            npcDialogBox.SetActive(false);
        }
    }
}
