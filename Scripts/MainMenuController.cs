using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    public GameObject credits;
    public void StartGame(){
        SceneManager.LoadScene(1); //0: Main Menu, 1: Game
    }
    public void ShowCredits(){
        credits.SetActive(true);
        this.gameObject.SetActive(false);
    }
    public void QuitGame(){
        Debug.Log("Player has quit the game");
        Application.Quit();
    }
}
