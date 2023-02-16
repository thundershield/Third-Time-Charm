using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialog
{
    public string npcName;
    [TextArea(3, 9)]
    public string[] npcDialog;
}

