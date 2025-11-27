using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class PlayerAnimator : NetworkBehaviour
{
    private const string IS_Walking = "Iswalk";
    private Animator animator;
    [SerializeField] private PlayerController player;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        
        //string can cause error becuase the strings are 
        //case sensitive and can cause error in the game but it 
        //won't show in console

    }
    private void Update()
    {   if (!IsOwner)
        {
            return;
        }
        animator.SetBool(IS_Walking, player.IsWalking());
        
    }
}
