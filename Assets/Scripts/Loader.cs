using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    //this object is static which is loader script and 
    //it does not extend monobehaviour so the object and function 
    //in this class would not be destroyed throughout the scene change

    public enum Scene
    {
        MainMenuScene,
        LoadingScene,
        GameScene,
        LobbyScene,
        CharacterSelectScene,

    }
    private static Scene targetScene;


    public static void Load(Scene targetScene)
    {//asigning the above scene to the target scene we recieve
        Loader.targetScene = targetScene;
        //we load the loading scene before the target scene
        SceneManager.LoadScene(Scene.LoadingScene.ToString());

    }
    //we load the scene after the first update loader call back till
    //then the scene which is to be loaded is loading scene 
    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(),LoadSceneMode.Single);
    }
    public static void LoaderCallback()
    {
        //loading the target scene after loading scene
        SceneManager.LoadScene(targetScene.ToString());
    }
}
//this script is for communication between the main-menu scene 
//game-scene and loading-scene
