using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    //this is generic script it doesnot have to do with
    //UI element it can be reused anywhere

    //enum are fixed set options we can have and define
    private enum Mode
    {
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted,
    }
    [SerializeField] private Mode mode;

    //it runs after the regular update
    private void LateUpdate()
    {
        //after the logic in update for cutting counter
        //late update run for this
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                //it gives us the inverted direction from player to the camera
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + dirFromCamera);
                break;
                //these cases help that Ui donot tilt with the camera and it remains straight
            case Mode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;

            
            
        }

        
    }
}
