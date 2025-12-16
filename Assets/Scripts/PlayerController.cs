using UnityEngine;
using System;
using Unity.Netcode;




public class PlayerController : NetworkBehaviour,IkitchenObjectParent {


    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomething;
    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }

    //implementing singleton pattern
    public static PlayerController LocalInstance { get; private set; }//managing accessiblity through properties were we can
    //get the property from any other class but cannot set it cause it is private
    //property and this can get or set the field through same property 
    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs>  OnSelectedCounterChanged;
                              //we pass in generics then pass in the event args
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    [SerializeField] private float moveSpeed = 5f;
    //[SerializeField] private Inputs gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;
    
    private void Start()
    {
        //listening the event
        Inputs.Instance.OnInteractAction += GameInput_OnInteractAction;
        Inputs.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;


    }
    //specific to netcode used instead of Awake
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);


        if (IsServer){
        NetworkManager.Singleton.OnClientDisconnectCallback+=NetworkManager_OnClientDisconnectCallback;
        }
    }
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId==OwnerClientId&& HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }
    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e)
    {   if (!GameManager.Instance.IsGamePlaying()) return;
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }

        
    }
    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {if (!GameManager.Instance.IsGamePlaying()) return;
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }


    }
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        HandleMovement();
        HandleInteractions();


    }
    public bool IsWalking()
    {
        return isWalking;
    }
    private void HandleInteractions()
    {
        float interactDistance = 2f;
        Vector2 inputVector = Inputs.Instance.GetMovementVectorNormalized();

        Vector3 movedir = new Vector3(inputVector.x, 0f, inputVector.y);
        //raycast hit is used to give output of what obj did raycast hit

        if (movedir != Vector3.zero)
        //if we stop moving then our last interact 
        //position gets saved in variable to use it so we can interact with obj when not moving
        {
            lastInteractDir = movedir;
        }
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))//<>->are c# generics it used to access the component built in or unity components
            {
                //use this instead of tags because tags are horrible way to intialize any go
                //has ClearCounter
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);

                }

            }
            else
            {
                SetSelectedCounter(null);
            }

        }
        else
        {
            SetSelectedCounter(null);
        }


    }
    //using Rpc -Remote Procedure call 
    //it is way to run code on server and the client from another place
    //serverauth vs clienauth 
    //serverauth-is made for pvp games were the server decide what client an dod and client can only request
    //it's movement and rotation speciality-secure but slow needs another function for prediction
    //clientauth-client can make changes and move anywere they donot ask permission from the server use full for
    //co-op games speciality-less secure but responds fast on client side
    
    private void HandleMovement()
    {
        Vector2 inputVector = Inputs.Instance.GetMovementVectorNormalized();

        Vector3 movedir = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHieght = 2f;
        //adding layer mask to make player not collide
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHieght, playerRadius, movedir, moveDistance,collisionLayerMask);
        //the capsule cast takes the player position from the bootom,the head of player,player size/,were we want to move player,and the distance from the player

        //if we cannot move in any direction the player movement to other direction
        //or diagonally don't stop if we move in a direction we hug the wall
        //and move into the direction diagonally
        if (!canMove)
        {//cannot move to the movedir


            //attempt only x movement
            //adding layermask
            Vector3 movedirx = new Vector3(movedir.x, 0, 0).normalized;
            canMove = (movedir.x < -0.5f || movedir.x > +0.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHieght, playerRadius, movedirx, moveDistance,collisionLayerMask);
            //if we are attempting to move in the x direction and there is nothing on there then we can move
            if (canMove)
            {
                //can move only on x axis
                movedir = movedirx;
            }
            else
            {
                //we cannot move only on the x
                //attempt only z movement
                Vector3 movedirZ = new Vector3(0, 0, movedir.z).normalized;
                canMove = (movedir.z<-0.5f || movedir.z>+0.5f )&&!Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHieght, playerRadius, movedirZ, moveDistance,collisionLayerMask);
                if (canMove)
                {
                    //can move only on the z
                    movedir = movedirZ;
                }
                else
                {
                    //cannot move in any direction
                }
            }
        }
        if (canMove)
        {
            transform.position += movedir * moveDistance;
        }
        //set active when vector3 changes from zero 
        isWalking = movedir != Vector3.zero;
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, movedir, Time.deltaTime * rotateSpeed);

        //the slerp help us to move our player smoothly in rotation 
        //were movement is not pixelated or junky

        //normalization is used when we press w with a or d it gives us normalized value
        //were our player will move same speed in diagonal

    }
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
         OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
                    {
                        selectedCounter = selectedCounter
                    });
    }
    //in interface the only thing that needs to match is function signature
    //you can implement logic based on the need
public Transform GetTheKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
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
        //return the object which is attached to this networkBehaviour script
        return NetworkObject;
    }
    
}


