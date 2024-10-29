using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI; // Add UI for slider control

public class AudioManager : MonoBehaviourPun
{
    public AudioClip[] clips;
    public AudioClip[] songs;
    public GameObject audioObjectPrefab;
    public static AudioManager instance;
    private AudioSource aud;
    private float initVolume;
    private int curSong;

    [Header("Volume Controls")]
    public Slider masterVolumeSlider; // Master volume slider
    public Slider sfxVolumeSlider;    // SFX volume slider

    private float masterVolume = 1f; // Master volume default
    private float sfxVolume = 1f;    // SFX volume default

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        aud = gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        initVolume = aud.volume;

        // Link sliders to methods
        if (masterVolumeSlider) masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        if (sfxVolumeSlider) sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        UpdateAudioVolume();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        UpdateAudioVolume();
    }

    private void UpdateAudioVolume()
    {
        aud.volume = initVolume * masterVolume;  // Update background music volume
    }

    public IEnumerator SwitchSong(int index)
    {
        if (curSong == index) yield break;

        for (float i = 0; i < 1; i += Time.unscaledDeltaTime)
        {
            aud.volume = Mathf.Lerp(initVolume, 0, i);
            yield return null;
        }

        aud.Stop();
        aud.clip = songs[index];
        aud.Play();

        for (float i = 0; i < 1; i += Time.unscaledDeltaTime)
        {
            aud.volume = Mathf.Lerp(0, initVolume * masterVolume, i);
            yield return null;
        }

        curSong = index;
        yield break;
    }

    public void PlaySound(Vector3 position, int index, float pitch = 1, float volume = 1, Transform follower = null)
    {
        photonView.RPC("RPC_PlaySound", RpcTarget.All, position, index, pitch, volume * sfxVolume * masterVolume, follower != null ? follower.GetComponent<PhotonView>().ViewID : -1);
    }

    [PunRPC]
    private void RPC_PlaySound(Vector3 position, int index, float pitch, float volume, int followerViewID)
    {
        Transform follower = followerViewID != -1 ? PhotonView.Find(followerViewID).transform : null;
        AudioObject audObj = Instantiate(audioObjectPrefab, new Vector3(position.x, position.y, -5), Quaternion.identity).GetComponent<AudioObject>();
        audObj.Initialize(clips[index], pitch, volume * sfxVolume * masterVolume, follower);
    }
}
