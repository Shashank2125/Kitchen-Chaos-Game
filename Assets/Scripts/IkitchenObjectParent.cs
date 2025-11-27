using Unity.Netcode;
using UnityEngine;

public interface IkitchenObjectParent
{
    //just the function definition with its signature 
    public Transform GetTheKitchenObjectFollowTransform();


    public void SetKitchenObject(KitchenObject kitchenObject);

    public KitchenObject GetKitchenObject();

    public void ClearKitchenObject();

    public bool HasKitchenObject();
    public NetworkObject GetNetworkObject();
    
   
}
