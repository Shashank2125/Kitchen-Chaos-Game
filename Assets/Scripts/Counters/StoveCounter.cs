using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter,IProgress
{
//event for the interface 
public event EventHandler <IProgress.OnProgressChangedEventArgs> OnProgressChanged;
    //event system for visauls
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    //state machine
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;
     //for making the state network synchronized
    private NetworkVariable <State> state=new NetworkVariable<State>(State.Idle);//current state
    private NetworkVariable<float> fryingTimer=new NetworkVariable<float>(0f);
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;
    private NetworkVariable<float> burningTimer=new NetworkVariable<float>(0f);

    private void Start()
    {
        state.Value = State.Idle;
    }
    //network spawn for network variable
    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }
    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        //if frying frying rescipeSO is not null then only perform logic
        float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryingTimer.Value / fryingTimerMax
        });

    }
    private void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burningTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IProgress.OnProgressChangedEventArgs
        {
            progressNormalized = burningTimer.Value / burningTimerMax
        });

    }
    private void State_OnValueChanged(State prevoiusState,State newState)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state.Value
        });
        //Hiding the progress bar when either burned or idle 
        if (state.Value ==State.Burned || state.Value == State.Idle)
        {
            OnProgressChanged?.Invoke(this, new IProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });

        }
        
    }
    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        if (HasKitchenObject())
        {
            switch (state.Value)
            {
                case State.Idle:
                    break;

                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;
                    //increasing timer with actual time to cook
                    
                    if (fryingTimer.Value > fryingRecipeSO.fryingTimerMax)
                    //if timer is higher than maxTimer
                    {
                        //fried

                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        //destroy the uncooked mear
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                        //spawn cooked meat



                        state.Value = State.Fried;
                        //modify the state
                        burningTimer.Value = 0f;
                        SetBurningRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));
                       
                        
                    }
                    break;
                case State.Fried:
                    burningTimer.Value += Time.deltaTime;
                    //increasing timer with actual time to cook
                    OnProgressChanged?.Invoke(this, new IProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer.Value / burningRecipeSO.burningTimerMax
                    });

                    if (burningTimer.Value > burningRecipeSO.burningTimerMax)
                    //if timer is higher than maxTimer
                    {
                        //fried

                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        //destroy the uncooked mear
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);
                        //spawn cooked meat


                        state.Value = State.Burned;
                        //modify the state
                      

                    
                    }
                    break;
                case State.Burned:
               
                    break;
            }
        



        }
    }






    public override void Interact(PlayerController player)
    {
        if (!HasKitchenObject())
        {
            //has no kitchen object
            if (player.HasKitchenObject())
            {
                ////player is carrying something
                if (HasRecipeInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {   //if the player carrying kitchenobject can be fried
                    //drop the kitchen object
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);
                    //Getting the index of the kitchenObjectSO for this kitchen object and passing 
                    //it to the server rpc
                    InteractLogicPlaceObjectOnCounterServerRpc(
                        KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO())
                    );

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
                        // GetKitchenObject().DestroySelf();
                        SetStateIdleServerRpc();

                    }


                }
            }
            else
            {
                //player is not carrying anything
                //give it to player
                GetKitchenObject().SetKitchenObjectParent(player);
                //if player picks the obj state refreshes
                SetStateIdleServerRpc();

            }
        }

    }
    [ServerRpc(RequireOwnership =false)]
    private void SetStateIdleServerRpc()
    {
        //state can only be changed Through ServerRpc
        state.Value = State.Idle;
        
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
    {
        fryingTimer.Value = 0f;
        state.Value = State.Frying;
        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);

    }
    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
        //player drops something that can be fried





    }
    [ClientRpc]
     private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
         burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
                    //player drops something that can be fried
    }
    private bool HasRecipeInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
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
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
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
    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }


        }
        return null;

    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }


        }
        return null;

    }
    public bool IsFried()
    {
        return state.Value == State.Fried;
    }
}
 //implementations
    //if the player has object in hand with recipe
    //need to implement a timer for cooking the placed obj
    //if exceeds the timer the food gets burnt 
    //we need to heat the object
    //toggle the visual for the game object when the obj
    //is placed on the counter
