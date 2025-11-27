using System;
using Unity.Netcode;
using UnityEngine;

public class BaseCounter : NetworkBehaviour, IkitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlacedHere;
    public static void ResetStaticData()
    {
        OnAnyObjectPlacedHere = null;
    }

    [SerializeField] private Transform counterTopPoint;
    
    private KitchenObject kitchenObject;

    //we made this virtual cause the ClearCounter and ContainerCounter can interact 
    //with it
    public virtual void Interact(PlayerController player)
    {
        Debug.LogError("BaseCounter.Interact();");
    }
       public virtual void InteractAlternate(PlayerController player)
    {
        //Debug.LogError("BaseCounter.InteractAlternate();");
    }
     public Transform GetTheKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }
    public void SetKitchenObject(KitchenObject kitchenObject)
    {

        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

}
