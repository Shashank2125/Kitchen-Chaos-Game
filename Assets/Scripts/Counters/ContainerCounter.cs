using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    public override void Interact(PlayerController player)
    {
        //this condition is we only assign the kitchen object ones not multiple times
        //and the clear counter knows when something is on top it

        if (!player.HasKitchenObject())
        {//if player doesnot have kitchen object player picks up the obj
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            //kitchenObjectTransform.localPosition = Vector3.zero;
            //modifying the localpositon of tomato prefab taht it spawns on the counter top on the scene
            //Debug.Log(kitchenObjectTransform.GetComponent<KitchenObject>().GetKitchenObjectSO());
            //we get the kitchen object and what type of kitchen object SO we have to spawn the correct scriptable object 
            //kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
            //then we spawn and assign the kitchen object to the kitchenobjecttransform
            //kitchenObject.SetClearCounter(this);

            InteractLogicServerRpc();


        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        
    }
}
