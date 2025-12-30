using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    private const int MAX_PLAYER_AMOUNT=4;
   public static KitchenGameMultiplayer Instance { get; private set; }
   public event EventHandler OnTryingToJoinGame;
   public event EventHandler OnFailedToJoinGame;
    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    private void Awake()
    {
        Instance = this;
        //this object 
        DontDestroyOnLoad(gameObject);
    }
    public void StartHost()
    {
         NetworkManager.Singleton.ConnectionApprovalCallback+=NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }
     private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name == Loader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved=false;
            connectionApprovalResponse.Reason="Game has already started";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved=false;
            connectionApprovalResponse.Reason="Game is full";
            return;
        }
        connectionApprovalResponse.Approved=true;
        //if we enable connection approval so that no one can join
        //mid game we have manually enable player creation and if the
        //game is not in waiting to start condition no other player can 
        //can join
        //if (GameManager.Instance.IsWaitingToStart()){
        
        //connectionApprovalResponse.CreatePlayerObject=true;
        //}
        //else
        //{
          //  connectionApprovalResponse.Approved=false;
       // }
    }
    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this,EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback+=NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this,EventArgs.Empty);
    }
    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IkitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());


    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectRefrence)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        //spawning the object on the network were the object is spawned on the host and Client
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);
        //try get gets the Network object attached and give is as and o/p
        kitchenObjectParentNetworkObjectRefrence.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IkitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IkitchenObjectParent>();
       
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);

    }
    //gets the index from the SO of kitchen object
    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }
    //returns the kitchenobject from the index
    public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
    {
        return kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];
    }

    



    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObjectReference);
        kitchenObject.DestroySelf();

    }
    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        kitchenObject.ClearKitchenObjectOnParent();
    }

}
