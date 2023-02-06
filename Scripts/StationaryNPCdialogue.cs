using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StationaryNPCdialogue : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    public string dialog;
    public bool dialogStateTriggered;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && dialogStateTriggered){
            if(dialogBox.activeInHierarchy){
                dialogBox.SetActive(false);
            }
            else{
                dialogBox.SetActive(true);
                dialogText.text = dialog;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("player1")){
           // Debug.Log("player triggered npc");
           dialogStateTriggered = true;
        }        
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("player1")){
          //  Debug.Log("player left npc");
          dialogStateTriggered = false;
        }    
    }
}
