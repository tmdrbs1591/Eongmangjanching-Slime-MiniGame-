using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SongManager : MonoBehaviourPunCallbacks
{
    public static SongManager Instance;

    private AudioSource audioSource; // 오디오 소스
    public List<AudioClip> sceneMusicClips; // 씬에 맞는 음악 클립 리스트
    private float fadeDuration = 0.3f; // 페이드 시간 설정
    public float targetVolume = 0.8f; // 최종 볼륨값 설정 (1f 대신 원하는 값으로 변경)

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;

        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource 추가
        audioSource.volume = 1f; // 초기 볼륨 설정
        audioSource.loop = true; // 반복 재생 활성화
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
        // 다른 씬에 맞는 음악 추가
    }

    public IEnumerator ChangeMusic(int clipIndex)
    {
        // 현재 재생 중인 음악이 있을 경우 페이드 아웃
        if (audioSource.isPlaying)
        {
            yield return StartCoroutine(FadeOut(audioSource, fadeDuration));
        }

        // 새로운 음악 클립 설정
        audioSource.clip = sceneMusicClips[clipIndex];
        audioSource.Play();

        // 페이드 인
        yield return StartCoroutine(FadeIn(audioSource, fadeDuration));
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
            audioSource.volume = Mathf.Lerp(0, targetVolume, t / duration); // 최종 볼륨으로 설정
            yield return null;
        }

        audioSource.volume = targetVolume; // 최종 볼륨 설정
    }
}
