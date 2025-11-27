using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private IkitchenObjectParent kitchenObjectParent;
    private FollowTransform followTransform;
    protected virtual void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }
    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    //The refactoring was done for syncronizing both client and server setting of parents
    public void SetKitchenObjectParent(IkitchenObjectParent kitchenObjectParent)
    {
        //in local function we call the server Rpc to get the output
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        //server allows the client to set the kitchenObjectParent
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
      
    }
    [ClientRpc]
    //client sets the transform for all the kitchenobject
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) 
    {
            kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IkitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IkitchenObjectParent>();
        //here this.clearCounter to old counter
        //and clear counter refers to new counter in assignment
        if (this.kitchenObjectParent != null)
        //if the current parent is not null we go into the parent and set
        //the parent to null
        {
            this.kitchenObjectParent.ClearKitchenObject();
        }
        this.kitchenObjectParent = kitchenObjectParent;

        if (kitchenObjectParent.HasKitchenObject())
        //if new clear counter is empty then only perform logic
        {
            Debug.LogError("kitchenObjectParent already has an object");
        }
        //new counter parent is set to the kitchen obj
        kitchenObjectParent.SetKitchenObject(this);
        //below logic is when we transfer the obj the obj is transfered to the 
        //other gameobj Visually it is a visual logic to set transform



        //instead of modifying the parent we are using another script to store
        //transform we need for the user to store spawned object
        followTransform.SetTargetTransform(kitchenObjectParent.GetTheKitchenObjectFollowTransform());
    }
    public IkitchenObjectParent GetIkitchenObjectParent()
    {
        return kitchenObjectParent;

        //we refactor all the code to work with kitchenobject parent rather than working with only
        //counter to implement the interface were we implement the kicthenObjectParent for counter as
        //well as player 
    }
    public void DestroySelf()
    {
       
        Destroy(gameObject);
    }
    public void ClearKitchenObjectOnParent()
    {
        kitchenObjectParent.ClearKitchenObject();
        
    }
    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
        
    }



    //a static function which we can call to get set kitchen object parent
    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IkitchenObjectParent kitchenObjectParent)
    {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);


    }
    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        KitchenGameMultiplayer.Instance.DestroyKitchenObject(kitchenObject); 
    }
   
}
