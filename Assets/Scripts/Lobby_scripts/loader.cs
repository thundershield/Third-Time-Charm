using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader {
    public enum Scene {
        Menu,
        Game,
        LobbyMenu,
    }


    private static Scene targetScene;


    public static void LoadNetwork(Scene targetScene) 
    {
       string gameScenePath = SceneUtility.GetScenePathByBuildIndex(1);
       SceneManager.LoadScene(1);
    }

    public static void LoaderCallback() 
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
  

}