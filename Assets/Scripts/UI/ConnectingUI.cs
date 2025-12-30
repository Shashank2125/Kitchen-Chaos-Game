using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Video;

public class ConnectingUI : MonoBehaviour
{

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame+=KitchenGameMultiplayer_OnTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame+=KitchenGameMultiplayer_OnFailedToJoinGame;
        Hide();
    }
    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender,System.EventArgs e)
    {
        Hide();
    }
    private void KitchenGameMultiplayer_OnTryingToJoinGame(object sender,System.EventArgs e)
    {
        Show();
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
