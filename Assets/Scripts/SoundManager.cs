using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour

{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClipRefSO audioClipRefSO;
    private float volume=1f;
    private void Awake()
    {
        Instance = this;
        //save the player prefrences as the volume 
        volume=PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        PlayerController.OnAnyPickedSomething += Player_OnPickedSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }
    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipRefSO.trash, trashCounter.transform.position);
    }
    private void BaseCounter_OnAnyObjectPlacedHere(object sender,System.EventArgs e)
    {
        //when the object is dropped on the basecounter which means on any counter
        //the sound should be played at pos of the counter
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(audioClipRefSO.objectDrop, baseCounter.transform.position);
    }
    private void Player_OnPickedSomething(object sender,System.EventArgs e)
    {
        PlayerController player = sender as PlayerController;
        PlaySound(audioClipRefSO.objectPickup,player.transform.position);
    }
    private void CuttingCounter_OnAnyCut(object sender,System.EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefSO.chop, cuttingCounter.transform.position);
    }
    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        //playing clip when delivery is success at pos of counter
         DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefSO.deliverySuccess,deliveryCounter.transform.position);

    }
    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
    {
        //playing clip when delivery success at pos of delivery counter
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefSO.deliveryFail,deliveryCounter.transform.position);
    }
     private void PlaySound(AudioClip[] audioClipArray,Vector3 position,float volume=1f)
    {
       
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
   
    }
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier*volume);
    }
    //we made public field for specific footstep sound for ease of access to the sound property
    //without coupling the array with the player here the position will be passed
    //when player access this script through instance 
    public void PlayFootstepsSound(Vector3 position, float volume)
    {
        PlaySound(audioClipRefSO.footSteps, position, volume);
    }
    public void PlayCountDownSound()
    {
        PlaySound(audioClipRefSO.warning, Vector3.zero);
    }
    public void PlayWarningSound(Vector3 position)
    {
        PlaySound(audioClipRefSO.warning,position);
    }
    public void ChangeVolume()
    {
        volume += 0.1f;//increasing volume by 10 percent
        if (volume > 1f)//if volume above 100% then reset to 0
        {
            volume = 0f;
        }
        //save the volume which is player prefrences and when we reset the scene it does not changes
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
    }
    public float GetVolume()
    {
        return volume;
    }
}
