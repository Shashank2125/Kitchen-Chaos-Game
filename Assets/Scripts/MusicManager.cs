using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    public static MusicManager Instance{ get; private set; }
    private AudioSource audioSource;
    private float volume= 0.3f;
    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        //when scene restart set the music to player pref or default it to 0.3f
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .3f);
        audioSource.volume = volume;
    }
    public void ChangeVolume()
    {
        volume += 0.1f;//increasing volume by 10 percent
        if (volume > 1f)//if volume above 100% then reset to 0
        {
            volume = 0f;
        }
        //update the volume in audio source by itself
        audioSource.volume = volume;
        //save the player prefrences for music volume
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }
    public float GetVolume()
    {
        return volume;
    }
}
