public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public override void Interact(PlayerController player)
    {
        if (player.HasKitchenObject())
        //player has something
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                //getting the instance from the deliverymanager for passing plateKitchen obj before
                //Destroying it 
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);
                //only accepts plate and destroys the plate
                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                
            }
            
        }
    }
}
