using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine;

public class testing : MonoBehaviour
{
[SerializeField] private Button host;
[SerializeField] private Button Client;



    private void Awake() 
    {
    host.onClick.AddListener(() => {
    Debug.Log("Host");
    NetworkManager.Singleton.StartHost();
    Hide();
    });
    Client.onClick.AddListener(() => {
    Debug.Log("Client");
    NetworkManager.Singleton.StartClient();
    Hide();
    });
    }


private void Hide()
{
    gameObject.SetActive(false);
}
}