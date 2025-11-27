using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public static Inputs Instance{ get; private set; }
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;
    public event EventHandler OnBindingRebind;
    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Pause,
        Interact,
        InteractAlternate,
        GamePad_Interact,
        GamePad_InteractAlternate,
        GamePad_Pause


    }

    private PlayerInputAction playerInputAction;

    private void Awake()
    {
        Instance = this;


        playerInputAction = new PlayerInputAction();
         //if the playerprefrence with string is saved then load playerpref
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            //loading player settings previously saved
           playerInputAction.LoadBindingOverridesFromJson( PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        playerInputAction.Player.Enable();
        playerInputAction.Player.Interact.performed += Interact_Performed;
        //listener
        //events has publisher and suscriber the publisher fires an event and suscriber listen to the event
        playerInputAction.Player.InteractAlternate.performed += InteractAlternate_Performed;
        playerInputAction.Player.Pause.performed += Pause_performed;
        // Debug.Log(GetBindingText(Binding.Move_Up));
       
    }
    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)

    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }
    //for suscribtion
    private void Interact_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //this function get all listener and suscriber to the event
        OnInteractAction?.Invoke(this, EventArgs.Empty);

    }
    private void OnDestroy() 
    {
        //unsuscribe to all the events when we destroy the game object or scene
        playerInputAction.Player.Interact.performed -= Interact_Performed;
        //listener
        //events has publisher and suscriber the publisher fires an event and suscriber listen to the event
        playerInputAction.Player.InteractAlternate.performed -= InteractAlternate_Performed;
        playerInputAction.Player.Pause.performed -= Pause_performed;
        //dispose this gameobject
        playerInputAction.Dispose();

    }
    public Vector2 GetMovementVectorNormalized()
    {

        Vector2 inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;
        return inputVector;

    }
    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            //because the Move has all the binding which is an array and has composite
            //binding for up down left right which is why we start from 1 index
            case Binding.Move_Up:

                return playerInputAction.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:

                return playerInputAction.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:

                return playerInputAction.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:

                return playerInputAction.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                //this will return array of bindings in input action map and we 
                //make it a string and rather than Input/e return e
                return playerInputAction.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:

                return playerInputAction.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:

                return playerInputAction.Player.Pause.bindings[0].ToDisplayString();
            case Binding.GamePad_Pause:

                return playerInputAction.Player.Pause.bindings[1].ToDisplayString();
            case Binding.GamePad_Interact:

                return playerInputAction.Player.Interact.bindings[1].ToDisplayString();
            case Binding.GamePad_InteractAlternate:

                return playerInputAction.Player.InteractAlternate.bindings[1].ToDisplayString();
        }
    }
    //action is a delegate which take no parameters and returns void
    public void RebindBinding(Binding binding , Action onActionRebound)
    {
        //disable action map
        playerInputAction.Player.Disable();
        InputAction inputAction;
        //take input action and binding index
        int bindingIndex;
        switch (binding)
        {
            //set them according to the case required
            default:
            case Binding.Move_Up:
                inputAction = playerInputAction.Player.Move;
                bindingIndex = 1;
                break;
             case Binding.Move_Down:
                inputAction = playerInputAction.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = playerInputAction.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = playerInputAction.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerInputAction.Player.Interact;
                bindingIndex = 0;
                break; 
            case Binding.InteractAlternate:
                inputAction = playerInputAction.Player.InteractAlternate;
                bindingIndex = 0;
                break;   
            case Binding.Pause:
                inputAction = playerInputAction.Player.Pause;
                bindingIndex = 0;
                break;  
            case Binding.GamePad_Interact:
                inputAction = playerInputAction.Player.Interact;
                bindingIndex = 1;
                break; 
            case Binding.GamePad_InteractAlternate:
                inputAction = playerInputAction.Player.InteractAlternate;
                bindingIndex = 1;
                break;   
            case Binding.GamePad_Pause:
                inputAction = playerInputAction.Player.Pause;
                bindingIndex = 1;
                break;  

        }
        //find the action
        //rebind using PerformInteractiveRebinding which takes the index of the object
        //in array of the object
        inputAction.PerformInteractiveRebinding(bindingIndex)
        //when we find the actionMap
        .OnComplete(callback =>
        {
           // Debug.Log(callback.action.bindings[1].path);
            //override the existing path
           // Debug.Log(callback.action.bindings[1].overridePath);
            callback.Dispose();
            //enable new mapping
            playerInputAction.Player.Enable();
            onActionRebound();

            //after we complete rebinding save the rebinding make it non-volatile
            //it stores it in json file which is simple text file
            PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputAction.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();

            OnBindingRebind?.Invoke(this, EventArgs.Empty);
        })
        //start the rebinding process
        .Start();
        
    }
}
