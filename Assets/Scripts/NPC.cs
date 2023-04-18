using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public Dialog yesDialog;
    public Dialog noDialog;
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
    void FixedUpdate()
    {
        if(npcTriggered == true) // && currentState == NpcState.talking)
        {
            npcDialogBox.SetActive(true);
            npcName.text = npcNameString;
            npcDialogText.text = npcTextString;
        }
       // else
      //  {
      //      npcDialogBox.SetActive(false);
      //  }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("player triggered npc");
            npcTriggered = true;
            //npcDialogBox.SetActive(true);
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

    public void TriggerYesDialog()
    {
        FindObjectOfType<DialogManager>().StartYesDialog(yesDialog);
    }

    public void TriggerNoDialog()
    {
        FindObjectOfType<DialogManager>().StartNoDialog(noDialog);
    }
}
