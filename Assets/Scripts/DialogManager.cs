using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public Text nameTxt;
    public Text dialogTxt;
    private Queue<string> npcDialog;
    private Queue<string> npcNoAnswerDialog;

    // Start is called before the first frame update
    void Start()
    {
        npcDialog = new Queue<string>();
        npcNoAnswerDialog = new Queue<string>();
    }

    public void StartYesDialog(Dialog dialog)
    {
        Debug.Log("Yes dialog starting.");
        nameTxt.text = dialog.npcName;
        npcDialog.Clear();

        foreach(string dialogString in dialog.npcDialog)
        {
            npcDialog.Enqueue(dialogString);
        }
        ShowNextString();
    }

    public void StartNoDialog(Dialog dialog)
    {
        Debug.Log("No dialog starting.");
        nameTxt.text = dialog.npcName;
        npcNoAnswerDialog.Clear();

        foreach(string dialogString in dialog.npcNoAnswerDialog)
        {
            npcNoAnswerDialog.Enqueue(dialogString);
        }
        ShowNextNoString();
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

    public void ShowNextNoString()
    {
        if(npcNoAnswerDialog.Count == 0)
        {
            ExitDialog();
            return;
        }
        string dialogString = npcNoAnswerDialog.Dequeue();
        Debug.Log(dialogString);
        dialogTxt.text = dialogString;
    }
    public void ExitDialog()
    {
            Debug.Log("ExitDialog");
    }
   
}
