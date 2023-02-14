using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public Text nameTxt;
    public Text dialogTxt;
    private Queue<string> npcDialog;
    // Start is called before the first frame update
    void Start()
    {
        npcDialog = new Queue<string>();
    }

    public void StartConversation(Dialog dialog)
    {
        Debug.Log("Conversation starting.");
        nameTxt.text = dialog.npcName;
        npcDialog.Clear();

        foreach(string dialogString in dialog.npcDialog)
        {
            npcDialog.Enqueue(dialogString);
        }
        ShowNextString();
    }

    public void ShowNextString()
    {
        if(npcDialog.Count == 0)
        {
            ExitDialog();
            return;
        }
        string dialogString = npcDialog.Dequeue();
        Debug.Log(dialogString);
        dialogTxt.text = dialogString;
    }

    public void ExitDialog()
    {
            Debug.Log("ExitDialog");
    }
   
}
