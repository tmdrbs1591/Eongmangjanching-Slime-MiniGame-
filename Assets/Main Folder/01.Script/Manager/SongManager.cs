using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class SongManager : MonoBehaviourPunCallbacks
{
    public static SongManager Instance;

    [SerializeField] private AudioSource musicSource; // 음악용 오디오 소스
    public List<AudioClip> sceneMusicClips; // 씬에 맞는 음악 클립 리스트
    private float fadeDuration = 0.3f; // 페이드 시간 설정
    public float targetVolume = 0.8f; // 최종 음악 볼륨값 설정

    [Header("Volume Sliders")]
    public Slider masterVolumeSlider; // 마스터 볼륨 슬라이더
    public Slider musicVolumeSlider; // 음악 볼륨 슬라이더

    private float masterVolume = 1f; // 마스터 볼륨 초기값
    private float musicVolume = 1f; // 음악 볼륨 초기값

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;

        musicSource.loop = true; // 음악 반복 재생 활성화
        UpdateMusicVolume();
    }

    private void Start()
    {
        // 슬라이더 값 변경 시 호출될 메소드 연결
        if (masterVolumeSlider) masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicVolumeSlider) musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded; // 구독 해제
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (SceneManager.GetActiveScene().name == "00.Room")
        {
            StartCoroutine(ChangeMusic(0)); // 첫 번째 음악 클립 재생
        }
        else if (SceneManager.GetActiveScene().name == "01.StartWaitRoom")
        {
            StartCoroutine(ChangeMusic(1)); // 두 번째 음악 클립 재생
        }
        else if (SceneManager.GetActiveScene().name == "01.WaitRoom")
        {
            StartCoroutine(ChangeMusic(1)); // 두 번째 음악 클립 재생
        }
        else if (SceneManager.GetActiveScene().name == "EV.Arrow Event")
        {
            StartCoroutine(ChangeMusic(2)); // 세 번째 음악 클립 재생
        }
        else if (SceneManager.GetActiveScene().name == "EV.LogEvent")
        {
            StartCoroutine(ChangeMusic(3)); // 네 번째 음악 클립 재생
        }
        else if (SceneManager.GetActiveScene().name == "Ev.KingSlime")
        {
            StartCoroutine(ChangeMusic(4));
        }
    }

    public IEnumerator ChangeMusic(int clipIndex)
    {
        // 현재 재생 중인 음악이 있을 경우 페이드 아웃
        if (musicSource.isPlaying)
        {
            yield return StartCoroutine(FadeOut(musicSource, fadeDuration));
        }

        // 새로운 음악 클립 설정
        musicSource.clip = sceneMusicClips[clipIndex];

        // 페이드 인
        yield return StartCoroutine(FadeIn(musicSource, fadeDuration));
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop(); // 음악 정지
    }

    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        audioSource.volume = 0; // 볼륨을 0으로 설정
        audioSource.Play(); // 음악 재생 시작

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, targetVolume * masterVolume * musicVolume, t / duration); // 최종 볼륨으로 설정
            yield return null;
        }

        audioSource.volume = targetVolume * masterVolume * musicVolume; // 최종 볼륨 설정
    }

    // 마스터 볼륨 설정 메소드
    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        UpdateMusicVolume();
    }

    // 음악 볼륨 설정 메소드
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        UpdateMusicVolume();
    }

    // 음악 볼륨 업데이트 메소드
    private void UpdateMusicVolume()
    {
        musicSource.volume = targetVolume * masterVolume * musicVolume;
    }
}
