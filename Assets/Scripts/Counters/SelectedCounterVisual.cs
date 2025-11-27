using System;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;
    private void Start()
    {
        
        if (PlayerController.LocalInstance != null)
        {


            PlayerController.LocalInstance.OnSelectedCounterChanged += PlayerController_OnSelectedCounterChanged;

        }
        else
        {
            PlayerController.OnAnyPlayerSpawned += PlayerController_OnAnyPlayerSpawned;
        }
    }

    private void PlayerController_OnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (PlayerController.LocalInstance != null)
        {
            //for having single listener and avoiding duplicates
            PlayerController.LocalInstance.OnSelectedCounterChanged -= PlayerController_OnSelectedCounterChanged;
            PlayerController.LocalInstance.OnSelectedCounterChanged += PlayerController_OnSelectedCounterChanged;
        }
    }
    

    private void PlayerController_OnSelectedCounterChanged(object sender, PlayerController.OnSelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == baseCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    private void Show()
    {

        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);

        }
    }
    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }
}