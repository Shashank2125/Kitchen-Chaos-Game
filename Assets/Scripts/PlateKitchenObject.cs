using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    //inherting the kitchen object logic onto the plate
    //list of valid ingredient for using it to make items
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectList;
    private List<KitchenObjectSO> kitchenObjectSOList;
    //storing the list of kitchen objetc so
    protected override void Awake()
    {
        //so we run the Awake code in kitchenObject then this
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (!validKitchenObjectList.Contains(kitchenObjectSO))
        {
            //not a valid ingredient
            return false;
        }
        //add whatever is onto the plate
        if (kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            //already has the object we are assigning
            return false;

        }
        else
        {
            AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
            
            return true;
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientClientRpc(kitchenObjectSOIndex);

    }
    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO=KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        kitchenObjectSOList.Add(kitchenObjectSO);
            //if we do add the ingredient
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
        kitchenObjectSO = kitchenObjectSO
         });
        
    }
    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }
    
}
