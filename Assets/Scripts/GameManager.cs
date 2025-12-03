using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    //state machine for countdown Timer
    public event EventHandler OnStateChange;
    public event EventHandler OnGamePause;
    public event EventHandler OnGameUnpause;
    public event EventHandler OnLocalPlayerReadyChanged;
    private enum State
    {
        
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private State state;
    //for checking if the player is ready to play or not
    private bool isLocalPlayerReady;
   
    //3sec to 1sec
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 300f;
    private bool isGamePaused = false;
    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStart;
    }
    private void Start()
    {
        Inputs.Instance.OnPauseAction += Inputs_OnPauseAction;
        Inputs.Instance.OnInteractAction += Inputs_OnInteractAction;


    }
    private void Inputs_OnInteractAction(object sender, EventArgs e)
    {
        if (state == State.WaitingToStart)
        {
           isLocalPlayerReady=true;
           OnLocalPlayerReadyChanged?.Invoke(this,EventArgs.Empty);
        }
    }
   
    private void Inputs_OnPauseAction(object sender,EventArgs e)
    {
        TogglePauseGame();
    }
    
    private void Update()
    {
        //switching between state based on State and timer we have 
        //made 
        switch (state)
        {
            case State.WaitingToStart:
               
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnStateChange?.Invoke(this,EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    state = State.GameOver;
                    OnStateChange?.Invoke(this,EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
        
    }
    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }
    public bool IsCountdownToStartActive()
    {
        return state == State.CountdownToStart;
    }
    public float GetCountDownToStartTimer()
    {
        return countdownToStartTimer;
    }
    public bool IsGameOver()
    {
        return state == State.GameOver;
    }
    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }
    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
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
