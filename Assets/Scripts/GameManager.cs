using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    //to support as many player as we want we can use dictionary
    //also the player id will not be sequential so to support that
    //The dictonary is best data structure

    public static GameManager Instance { get; private set; }
    //state machine for countdown Timer
    public event EventHandler OnStateChange;
    public event EventHandler OnLocalGamePause;
    public event EventHandler OnLocalGameUnpause;
    //Local player ready is changed event
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;
    private enum State
    {
        
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }
    [SerializeField] private Transform playerPrefab;

    private NetworkVariable<State> state=new NetworkVariable<State>(State.WaitingToStart);
    //for checking if the player is ready to play or not
    private bool isLocalPlayerReady;
    private bool autoTestGamePausedState;
   
    //3sec to 1sec
    private NetworkVariable <float> countdownToStartTimer = new NetworkVariable<float> (3f);
    private NetworkVariable <float> gamePlayingTimer= new NetworkVariable<float>(0f);
    private float gamePlayingTimerMax = 300f;
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused=new NetworkVariable<bool>(false);
    //ulong for player id ulong only stores positive numbers size=64 unsigned bits
    private Dictionary<ulong,bool> playerReadyDictionary;
    private Dictionary<ulong,bool> playerPausedDictionary;
    private void Awake()
    {
        Instance = this;

        playerReadyDictionary=new Dictionary<ulong, bool>();
        playerPausedDictionary=new Dictionary<ulong, bool>();
    }
    private void Start()
    {
        Inputs.Instance.OnPauseAction += Inputs_OnPauseAction;
        Inputs.Instance.OnInteractAction += Inputs_OnInteractAction;


    }
    public override void OnNetworkSpawn()
    {
        state.OnValueChanged+=State_OnValueChange;
        isGamePaused.OnValueChanged+=IsGamePaused_OnValueChanged;
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback+=NetworkManager_OnClientDisconnectCallBack;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted+=SceneManager_OnLoadEventCompleted;
        }

    }
    //spawning player prefab on the game 
    private void SceneManager_OnLoadEventCompleted(string sceneName,UnityEngine.SceneManagement.LoadSceneMode loadSceneMode,List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform= Instantiate(playerPrefab);
            //player get spawned and destroyed with scene
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId,true);
        }
    }
    private void NetworkManager_OnClientDisconnectCallBack(ulong clientId)
    {
        autoTestGamePausedState=true;
    }
    private void IsGamePaused_OnValueChanged(bool previousValue,bool newValue)
    {
        if (isGamePaused.Value)
        {
            Time.timeScale=0f;
            OnMultiplayerGamePaused?.Invoke(this,EventArgs.Empty);
        }
        else
        {
            Time.timeScale=1f;
            OnMultiplayerGameUnpaused?.Invoke(this,EventArgs.Empty);
        }
    }
    private void State_OnValueChange( State previousValue, State newValue)
    {
        OnStateChange?.Invoke(this,EventArgs.Empty);
    }
    private void Inputs_OnInteractAction(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
           isLocalPlayerReady=true;
           //we can pass owner client id but the hacker can fake the client id 
           //and break server for avoiding that we remove it
           OnLocalPlayerReadyChanged?.Invoke(this,EventArgs.Empty);
           SetPlayerReadyServerRpc();
           //listening to the event
           
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams=default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId]=true;
        bool allClientsReady=true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId)|| !playerReadyDictionary[clientId])
            {
                //This player not ready
                allClientsReady=false;
                break;
            }
            if (allClientsReady)
            {
                state.Value=State.CountdownToStart;
            }
        }
        Debug.Log("allclientReady" + allClientsReady);
    }
    private void Inputs_OnPauseAction(object sender,EventArgs e)
    {
        TogglePauseGame();
    }
    
    private void Update()
    {
            if (!IsServer)
        {
            return;
        }
        //switching between state based on State and timer we have 
        //made 
        switch (state.Value)
        {
        
            case State.WaitingToStart:
               
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f)
                {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                    
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f)
                {
                    state.Value = State.GameOver;
                   
                }
                break;
            case State.GameOver:
                break;
        }
        
    }
    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }
    public bool IsCountdownToStartActive()
    {
        return state.Value == State.CountdownToStart;
    }
    public float GetCountDownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }
    public bool IsWaitingToStart()
    {
        return state.Value==State.WaitingToStart;
    }
    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }
    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }
    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }
    public void TogglePauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused)
        {
            PauseGameServerRpc();
            //pause the game 
            
            OnLocalGamePause?.Invoke(this, EventArgs.Empty);
            //timescale already has multiplier it pauses all the action
            //of time.deltatime
        }
        else
        {
            UnpauseGameServerRpc();

            //unpause the game 
            
            
             OnLocalGameUnpause?.Invoke(this, EventArgs.Empty);
        }
    }
    //through these serverrpc server knows which player are paused or
    //unpaused
    [ServerRpc(RequireOwnership =false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams=default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId]=true;
        TestGamePausedState();
    }
    [ServerRpc(RequireOwnership =false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams=default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId]=false;
        TestGamePausedState();
    }
    private void TestGamePausedState()
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientId)&& playerPausedDictionary[clientId])
            {
                //THis player is paused
                isGamePaused.Value=true;
                return;
            }
        }
        isGamePaused.Value=false;
        //if we reach here then all player are unpaused

    }
}
