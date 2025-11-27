using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticEvent : MonoBehaviour
{
    //reset all the event listener so that if we reload 
    //the scene we donot have more than one event which 
    //can create issues in event listener so we clear all the event
    //clearing all static data which do not automatically clean up after scene loading
    private void Awake()
    {
        CuttingCounter.ResetStaticData();
        BaseCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
        PlayerController.ResetStaticData();

    }
}
