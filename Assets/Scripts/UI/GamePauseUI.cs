using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    private void Awake()
    {
        //when pause button clicked
        resumeButton.onClick.AddListener(() =>
        {
            //toggle the pause to active 
            GameManager.Instance.TogglePauseGame();

        });
        //when click main menu
        mainMenuButton.onClick.AddListener(() =>
        {
            //load main menu
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        optionsButton.onClick.AddListener(() =>
        {
            Hide();
            OptionsUI.Instance.Show(Show);
        });
    }
    private void Start()
    {
        GameManager.Instance.OnLocalGamePause += GameManager_OnLocalGamePause;
        GameManager.Instance.OnLocalGameUnpause += GameManager_OnLocalGameUnpause;

        Hide();
    }
    private void GameManager_OnLocalGamePause(object sender, System.EventArgs e)
    {
        //show pause window
        Show();

    }
    private void GameManager_OnLocalGameUnpause(object sender, System.EventArgs e)
    {
        //hide the pause window
        Hide();

    }

    private void Show()
    {
        gameObject.SetActive(true);
        resumeButton.Select();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
