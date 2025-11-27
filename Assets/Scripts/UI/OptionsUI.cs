using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }
    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamepadInteractButton;
    [SerializeField] private Button gamepadInteractAlternateButton;
    [SerializeField] private Button gamepadPauseButton;
    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAlternateText;
    [SerializeField] private TextMeshProUGUI gamepadPauseText;
    [SerializeField] private TextMeshProUGUI gamepadInteractText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAlternateText;
    [SerializeField] private Transform pressToRebindKeyTransform;

    private Action onCloseButtonAction; 

    private void Awake()
    {
        Instance = this;

        soundEffectsButton.onClick.AddListener(() =>
        {
            //change the volume 
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();

        });
        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();

        });
        closeButton.onClick.AddListener(() =>
        {
            //when close button is pressed call hide
            Hide();
            onCloseButtonAction();
        });
        //listener for all the buttons in option menu
        moveUpButton.onClick.AddListener(() =>
        {
            RebindBinding(Inputs.Binding.Move_Up);

        });
            moveDownButton.onClick.AddListener(() =>
        {
            RebindBinding(Inputs.Binding.Move_Down);
            
        });
            moveLeftButton.onClick.AddListener(() =>
        {
            RebindBinding(Inputs.Binding.Move_Left);
            
        });
            moveRightButton.onClick.AddListener(() =>
        {
            RebindBinding(Inputs.Binding.Move_Right);
            
        });
            interactButton.onClick.AddListener(() =>
        {
            RebindBinding(Inputs.Binding.Interact);
            
        });
        interactAlternateButton.onClick.AddListener(() =>
        {
        RebindBinding(Inputs.Binding.InteractAlternate);

        });
        pauseButton.onClick.AddListener(() =>
        {
            RebindBinding(Inputs.Binding.Pause);

        });
             gamepadInteractButton.onClick.AddListener(() =>
        {
            RebindBinding(Inputs.Binding.GamePad_Interact);
            
        });
        gamepadInteractAlternateButton.onClick.AddListener(() =>
        {
        RebindBinding(Inputs.Binding.GamePad_InteractAlternate);

        });
        gamepadPauseButton.onClick.AddListener(() =>
        {
            RebindBinding(Inputs.Binding.GamePad_Pause);
            
        });


    }
    private void Start()
    {
        UpdateVisual();
        Hide();
    }
    private void UpdateVisual()
    {
        GameManager.Instance.OnGameUnpause += GameManager_OnGamePaused;
        //update the visual on optionsUI
        soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);
        moveUpText.text = Inputs.Instance.GetBindingText(Inputs.Binding.Move_Up);
        moveDownText.text = Inputs.Instance.GetBindingText(Inputs.Binding.Move_Down);
        moveLeftText.text = Inputs.Instance.GetBindingText(Inputs.Binding.Move_Left);
        moveRightText.text = Inputs.Instance.GetBindingText(Inputs.Binding.Move_Right);
        interactText.text = Inputs.Instance.GetBindingText(Inputs.Binding.Interact);
        interactAlternateText.text = Inputs.Instance.GetBindingText(Inputs.Binding.InteractAlternate);
        pauseText.text = Inputs.Instance.GetBindingText(Inputs.Binding.Pause);
        gamepadInteractText.text = Inputs.Instance.GetBindingText(Inputs.Binding.GamePad_Interact);
        gamepadInteractAlternateText.text = Inputs.Instance.GetBindingText(Inputs.Binding.GamePad_InteractAlternate);
        gamepadPauseText.text = Inputs.Instance.GetBindingText(Inputs.Binding.GamePad_Pause);

    }
    private void GameManager_OnGamePaused(object sender,System.EventArgs e)
    {
        Hide();
    }
    public void Show(Action onCloseButtonAction)
    {
        this.onCloseButtonAction = onCloseButtonAction;
        gameObject.SetActive(true);

        soundEffectsButton.Select();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void ShowPressToRebind()
    {
        pressToRebindKeyTransform.gameObject.SetActive(true);

    }
    private void HidePressToRebind()
    {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }
    private void RebindBinding(Inputs.Binding binding)
    {
        
        ShowPressToRebind();
        Inputs.Instance.RebindBinding(binding,()=> {

            HidePressToRebind();
            UpdateVisual();
            });
        
    }
    
}
