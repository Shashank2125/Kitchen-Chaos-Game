using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI RecipeDeliveredText;
    private void Start()
    {
        GameManager.Instance.OnStateChange += GameManager_OnStateChange;
        Hide();
    }
    private void GameManager_OnStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            //we show the gameobject
            Show();
            //recipe delivered
            RecipeDeliveredText.text = DeliveryManager.Instance.GetSuccessfulDdelivery().ToString();
        }
        else
        {
            Hide();

        }

    }
    private void Update()
    {

       
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
