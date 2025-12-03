using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialsUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI keyMoveUpText;
    [SerializeField] TextMeshProUGUI keyMoveDownText;
    [SerializeField] TextMeshProUGUI keyMoveLeftText;
    [SerializeField] TextMeshProUGUI keyMoveRightText;
    [SerializeField] TextMeshProUGUI keyInteractText;
    [SerializeField] TextMeshProUGUI keyInteractAlternateText;
    [SerializeField] TextMeshProUGUI keyPauseText;
    [SerializeField] TextMeshProUGUI keyGamePadInteractText;
    [SerializeField] TextMeshProUGUI keyGamePadInteractAlternateText;
    [SerializeField] TextMeshProUGUI keyGamePadPauseText;
    private void Start()
    {
        Inputs.Instance.OnBindingRebind += Inputs_OnBindingRebind;
        GameManager.Instance.OnStateChange += GameManager_OnStateChange;
        GameManager.Instance.OnLocalPlayerReadyChanged+=GameManager_OnLocalPlayerReadyChanged;
        UpdateVisual();
        Show();
    }
    //Event listener for the event in game manager
    private void GameManager_OnLocalPlayerReadyChanged(object sender,System.EventArgs e)
    {
        //If the local player is ready then hide tutorial and start the game 
        if (GameManager.Instance.IsLocalPlayerReady())
        {
            Hide();
        }
    }
    private void GameManager_OnStateChange(object sender,System.EventArgs e)
    {
        //if state changed to countdown to start then hide the tutorial
        if (GameManager.Instance.IsCountdownToStartActive()|| GameManager.Instance.IsGamePlaying() || GameManager.Instance.IsGameOver())
        {
            Hide(); 
        }
        
    }
    private void Inputs_OnBindingRebind(object sender,System.EventArgs e)
    {
        UpdateVisual();
    }
    private void UpdateVisual()
    {
        keyMoveUpText.text = Inputs.Instance.GetBindingText(Inputs.Binding.Move_Up);
        keyMoveDownText.text = Inputs.Instance.GetBindingText(Inputs.Binding.Move_Down);
        keyMoveLeftText.text = Inputs.Instance.GetBindingText(Inputs.Binding.Move_Left);
        keyMoveRightText.text = Inputs.Instance.GetBindingText(Inputs.Binding.Move_Right);
        keyPauseText.text = Inputs.Instance.GetBindingText(Inputs.Binding.Pause);
        keyInteractAlternateText.text = Inputs.Instance.GetBindingText(Inputs.Binding.InteractAlternate);
        keyInteractText.text = Inputs.Instance.GetBindingText(Inputs.Binding.Interact);
        keyGamePadInteractAlternateText.text = Inputs.Instance.GetBindingText(Inputs.Binding.GamePad_InteractAlternate);
        keyGamePadInteractText.text = Inputs.Instance.GetBindingText(Inputs.Binding.GamePad_Interact);
        keyGamePadPauseText.text = Inputs.Instance.GetBindingText(Inputs.Binding.GamePad_Pause);


    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
