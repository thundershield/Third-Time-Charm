using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMultMenu : MonoBehaviour {


    [SerializeField] private Button authenticateButton;


    private void Awake() {
        authenticateButton.onClick.AddListener(() => {
            LobbyManager.Instance.Authenticate(PlayerName.Instance.GetPlayerName());
            makeNotVisible();
        });
    }

    private void makeNotVisible() 
    {
        gameObject.SetActive(false);
    }

}