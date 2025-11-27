using System;
using Unity.Netcode;
using UnityEngine;

public class PlateCounter : BaseCounter
{
    public event EventHandler OnPlateRemoved;
    public event EventHandler OnPlateSpawned;
    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnAmount;
    private int plateSpawnAmountMax = 4;

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;
            if (GameManager.Instance.IsGamePlaying() && platesSpawnAmount < plateSpawnAmountMax)
            {
                SpawnPlateServerRpc();
            }

        }
    }
    [ServerRpc]
private void SpawnPlateServerRpc()
{
        SpawnPlateClientRpc();
}

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        platesSpawnAmount++;
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
        
    }
    public override void Interact(PlayerController player)
    {
        if (!player.HasKitchenObject())
        {
            //player is empty handed

            if (platesSpawnAmount > 0)
            {
                //we have plate to assign to plates
                //atleast 1 plate

                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                InteractLogicServerRpc();
                
            }
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
        platesSpawnAmount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
       


        
      
        
    }
}
//for this we need to spawn mulitple dummy object becuase of refactoring cost
//then a player needs a plate then we spawn the real object to change its parent 
