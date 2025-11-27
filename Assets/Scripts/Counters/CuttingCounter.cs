using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter,IProgress
{
    //synchronize the cuttting counter logic while cutting on both server and client 
    public static event EventHandler OnAnyCut;
    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }
    public event EventHandler <IProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }
    public event EventHandler OnCut;
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    



    private int cuttingProgress;
    public override void Interact(PlayerController player)
    {
        if (!HasKitchenObject())
        {
            //has no kitchen object
            if (player.HasKitchenObject())
            {
                ////player is carrying something
                if (HasRecipeInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {   //if the player carrying kitchenobject can be cut
                    //drop the kitchen object
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);
                    InteractLogicPlaceObjectOnCounterServerRpc();
                    
                }
            }
            else
            {
                //player not carrying anything
            }
        }
        else
        {
            //does have kitchen object
            if (player.HasKitchenObject())
            {
                //player is carrying something
                //donot give it to player
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //player is holding a plate
                    //we want to add whatever it is onto the kitchen object to plate player is holding

                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        //if the object already not exist in tryingredient 
                        //adding the ingredient to the player 
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        //GetKitchenObject().DestroySelf();
                    }


                }
            }
            else
            {
                //player is not carrying anything
                //give it to player
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }
    [ClientRpc]
      private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;
                    //firing the event for progress bar 
                   
                    OnProgressChanged?.Invoke(this, new IProgress.OnProgressChangedEventArgs
                    {
                        //we are casting this one as a float because if int/int=returns int
                        //were 1/2=0 not 0.5 so change one element to get result
                        progressNormalized = 0f
                    });
        
    }
    public override void InteractAlternate(PlayerController player)
    {
        //something placed on here and it has the recipe which means it can be cut 
        if (HasKitchenObject() && HasRecipeInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            CutObjectServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void CutObjectServerRpc()
    {
        CutObjectClientRpc();
    }
    [ClientRpc]
    private void CutObjectClientRpc()
    {
        cuttingProgress++;
        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());


        //firing the event for progress bar 

        OnProgressChanged?.Invoke(this, new IProgress.OnProgressChangedEventArgs
        {
            //we are casting this one as a float because if int/int=returns int
            //were 1/2=0 not 0.5 so change one element to get result
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
        });
        

    }
    [ServerRpc(RequireOwnership =false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
        if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
        {

            KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            //there is kitchen object
            //destroy the kitchen object(kitchen object script) and spawn the sliced object
            KitchenObject.DestroyKitchenObject(GetKitchenObject());

            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);



        }
        
    }
    private bool HasRecipeInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
        //checking the object player is carrying has recipe as in it if not
        //then dont drop the object on the table
       // foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        //{
          //  if (cuttingRecipeSO.input == inputKitchenObjectSO)
          //  {
               // return true;
        // }


       // }
       // return false;

    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        else
        {
            return null;
        }
        //we are cycling through all the objects to get which object is placed on the cutting counter
        //in cuttingrecipeSO array
      //  foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        //{
          //  if (cuttingRecipeSO.input == inputKitchenObjectSO)
          //  {
               // return cuttingRecipeSO.output;
          //  }


      //  }
       // return null;

    }
    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }


        }
        return null;
        
    }
}
