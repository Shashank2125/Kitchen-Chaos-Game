using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    //to support as many player as we want we can use dictionary
    //also the player id will not be sequential so to support that
    //The dictonary is best data structure

    public static GameManager Instance { get; private set; }
    //state machine for countdown Timer
    public event EventHandler OnStateChange;
    public event EventHandler OnGamePause;
    public event EventHandler OnGameUnpause;
    //Local player ready is changed event
    public event EventHandler OnLocalPlayerReadyChanged;
    private enum State
    {
        
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private NetworkVariable<State> state=new NetworkVariable<State>(State.WaitingToStart);
    //for checking if the player is ready to play or not
    private bool isLocalPlayerReady;
   
    //3sec to 1sec
    private NetworkVariable <float> countdownToStartTimer = new NetworkVariable<float> (3f);
    private NetworkVariable <float> gamePlayingTimer= new NetworkVariable<float>(0f);
    private float gamePlayingTimerMax = 300f;
    private bool isGamePaused = false;
    //ulong for player id ulong only stores positive numbers size=64 unsigned bits
    private Dictionary<ulong,bool> playerReadyDictonary;
    private void Awake()
    {
        Instance = this;

        playerReadyDictonary=new Dictionary<ulong, bool>();
    }
    private void Start()
    {
        Inputs.Instance.OnPauseAction += Inputs_OnPauseAction;
        Inputs.Instance.OnInteractAction += Inputs_OnInteractAction;


    }
    public override void OnNetworkSpawn()
    {
        state.OnValueChanged+=State_OnValueChange;
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
           SetPlayerReadyServerRpc();
           //listening to the event
           OnLocalPlayerReadyChanged?.Invoke(this,EventArgs.Empty);
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams=default)
    {
        playerReadyDictonary[serverRpcParams.Receive.SenderClientId]=true;
        bool allClientsReady=true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictonary.ContainsKey(clientId)|| !playerReadyDictonary[clientId])
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
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            //pause the game 
            Time.timeScale = 0f;
            OnGamePause?.Invoke(this, EventArgs.Empty);
            //timescale already has multiplier it pauses all the action
            //of time.deltatime
        }
        else
        {

            //unpause the game 
            Time.timeScale = 1f;
            
             OnGameUnpause?.Invoke(this, EventArgs.Empty);
        }
    }
}
