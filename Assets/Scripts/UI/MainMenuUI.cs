using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    private void Awake()
    {
        //it is c# delegates
        //lambda expression
        playButton.onClick.AddListener(() =>
        {
            //click code
            Loader.Load(Loader.Scene.GameScene);
        });
        quitButton.onClick.AddListener(() =>
     {
         //click code
         Application.Quit();
     });
        //whenever we go into main menu the paused game resumes
        Time.timeScale = 1f;
    }
}
