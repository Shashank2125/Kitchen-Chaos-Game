using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image TimerImage;
    private void Update()
    {
        //the normalized time will fill the image we have created 
       TimerImage.fillAmount= GameManager.Instance.GetGamePlayingTimerNormalized();
    }
}
