using System;
using Unity.Netcode;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectTrashed;
    new public static void ResetStaticData()
    {
        OnAnyObjectTrashed = null;
    }

    public override void Interact(PlayerController player)
    {
        if (player.HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            //player.GetKitchenObject().DestroySelf();
            InteractLogicServerRpc();

        }
    }
    /// <summary>
    /// properly destroy a network object were we are using trash bin on the 
    /// object which are placed and trashCounter get destroyed so through this logic
    /// we can destroy the gameobject
    /// </summary>
    [ServerRpc(RequireOwnership =false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
       OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);  
    }
}
