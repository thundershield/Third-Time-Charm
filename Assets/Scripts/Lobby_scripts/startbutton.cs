using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;


public class startbutton : MonoBehaviour
{
    public startbutton Instance { get; private set; }
    public GameObject StartLobbyButton;
    public  void visible(int i)
    {
    if (i==1) 
        {
            StartLobbyButton.gameObject.SetActive(true);
        }
        else
        {
            StartLobbyButton.gameObject.SetActive(false);
        }

    }
    public void startgame()
    {
        SceneManager.LoadScene(1);
    }
}
