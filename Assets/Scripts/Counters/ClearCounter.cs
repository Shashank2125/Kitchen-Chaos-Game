using UnityEngine;

public class ClearCounter : BaseCounter//adding the interface but we have only one base class but we can 
//implement as many interface we want 
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;



    public override void Interact(PlayerController player)
    {
        if (!HasKitchenObject())
        {
            //has no kitchen object
            if (player.HasKitchenObject())
            {
                ////player is carrying something
                //drop the kitchen object
                player.GetKitchenObject().SetKitchenObjectParent(this);
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



                    }
                    else 
                    {
                        //player is not carrying Plate but something else
                        if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                        {

                            //counter is holding plate
                            if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                            {
                                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                            }
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
    }
}

