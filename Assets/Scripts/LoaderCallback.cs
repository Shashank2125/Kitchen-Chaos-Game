using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private bool isFirstUpdate = true;
    private void Update()
    {
        //if it is first update then call the function on the loader
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            Loader.LoaderCallback();

        }
    }
}
//this script ensure proper loading of the game scene 
//and main menu scene through adding Loading scene in the middle 
//which negates the lag in between game and main menu
