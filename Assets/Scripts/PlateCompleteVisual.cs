using UnityEngine;
using System;
using System.Collections.Generic;

public class PlateCompleteVisual : MonoBehaviour
{
    //we are using struct beacuse we are simply storing data without any 
    //logic 
    [Serializable]
    public struct kitchenObjectSO_GameObject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List <kitchenObjectSO_GameObject> kitchenObjectSOGameObjectList;
    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        //if the ingredient is not there deactivate the game object 
        foreach (kitchenObjectSO_GameObject kitchenObjectSO_GameObject in kitchenObjectSOGameObjectList)
        {
            kitchenObjectSO_GameObject.gameObject.SetActive(false);

        }

    }
    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        //set active the gameobject if there is an object 
        foreach (kitchenObjectSO_GameObject kitchenObjectSO_GameObject in kitchenObjectSOGameObjectList)
        {
            if (kitchenObjectSO_GameObject.kitchenObjectSO == e.kitchenObjectSO)
            {
                kitchenObjectSO_GameObject.gameObject.SetActive(true);
            }
        }
       //e.kitchenObjectSO 
    }

}
