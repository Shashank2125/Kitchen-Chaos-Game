using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DeliveryManager : NetworkBehaviour 
    

{
    public event EventHandler OnRecipeSpawn;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    public static DeliveryManager Instance { get; private set; }
    //creating the object as an instance for accessing it get component through other objects
    [SerializeField] private RecipeListSO recipeListSO;
    private List<RecipeSO> waitingrecipeSOList;
    //4f so we have time to connect host and the client
    private float spawnRecipeTimer=4f;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeMax = 4;
    private int successfulDelivery;
    private void Awake()
    {
        Instance = this;
        //set the instance to this game object
        waitingrecipeSOList = new List<RecipeSO>();
    }
    private void Update()
    {
        //we want only one delivery manager which is on server to spawn
        //delivery for both client and host
        if (!IsServer)
        {
            return;
        }
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;
            if (GameManager.Instance.IsGamePlaying() && waitingrecipeSOList.Count < waitingRecipeMax)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);

                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);

            }
        }



    }
    //we are not making the game for late joiner because it is a small game 
    //300sec thus we use these logic ClientRpc rather than we use Network Variable
    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
            RecipeSO waitingRecipeSo = recipeListSO.recipeSOList[waitingRecipeSOIndex];
              waitingrecipeSOList.Add(waitingRecipeSo);
                //triggering event when recipe is spawned
                OnRecipeSpawn?.Invoke(this, EventArgs.Empty);
        
    }
    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingrecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingrecipeSOList[i];
            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                //has same number of ingredients
                bool plateContentMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    //cycling through all ingredients in the recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        //cycling through all ingredients in the Plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            //ingredient match
                            ingredientFound = true;
                            break;
                        }

                    }
                    if (!ingredientFound)
                    {
                        //this Recipe was not found on the plate
                        plateContentMatchesRecipe = false;
                    }
                }
                if (plateContentMatchesRecipe)
                {
                    DeliverCorrectRecipeServerRpc(i);
                    return;
                }

            }

        }

        //no matches found 
        //player did not deliver a correct recipe
        DeliverIncorrectRecipeServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    //client can be able to trigger this serverRpc
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }
    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }
    [ServerRpc(RequireOwnership = false)]
    //were we can see when client deliver correct or incorrect recipe
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }
    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
         //player delivered the correct recipe 
                    successfulDelivery++;

                    waitingrecipeSOList.RemoveAt(waitingRecipeSOListIndex);
                    //when correct recipe is delivered remove the recipe from the list through this
                    //event 
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
        
    }
    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingrecipeSOList;
    }
    public int GetSuccessfulDdelivery()
    {
        return successfulDelivery;
    }
}
//recipe which are delivered and which needs to deliver debug it console
//by accessing diffrent type of recipe and handling the recipe put on the plate 
