using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsContoller : MonoBehaviour
{
    public GameObject mainMenu;

    public void ShowMainMenu(){
        mainMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
