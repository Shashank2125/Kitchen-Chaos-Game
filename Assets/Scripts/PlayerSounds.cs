using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private PlayerController player;
    private float footStepsTimer;
    private float footStepsTimerMax=0.1f;
    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }
    private void Update()
    {
        footStepsTimer -= Time.deltaTime;
        if (footStepsTimer < 0f)
        {
            footStepsTimer = footStepsTimerMax;
            //player is wlaking play sound else don't
            if (player.IsWalking())
            {
                float volumeFootstep = 1f;

                SoundManager.Instance.PlayFootstepsSound(player.transform.position, volumeFootstep);
            }
        }
    }

}
