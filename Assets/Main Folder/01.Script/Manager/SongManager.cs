using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class SongManager : MonoBehaviourPunCallbacks
{
    public static SongManager Instance;

    [SerializeField] private AudioSource musicSource; // ���ǿ� ����� �ҽ�
    public List<AudioClip> sceneMusicClips; // ���� �´� ���� Ŭ�� ����Ʈ
    private float fadeDuration = 0.3f; // ���̵� �ð� ����
    public float targetVolume = 0.8f; // ���� ���� ������ ����

    [Header("Volume Sliders")]
    public Slider masterVolumeSlider; // ������ ���� �����̴�
    public Slider musicVolumeSlider; // ���� ���� �����̴�

    private float masterVolume = 1f; // ������ ���� �ʱⰪ
    private float musicVolume = 1f; // ���� ���� �ʱⰪ

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;

        musicSource.loop = true; // ���� �ݺ� ��� Ȱ��ȭ
        UpdateMusicVolume();
    }

    private void Start()
    {
        // �����̴� �� ���� �� ȣ��� �޼ҵ� ����
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
        SceneManager.sceneLoaded -= OnSceneLoaded; // ���� ����
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (SceneManager.GetActiveScene().name == "00.Room")
        {
            StartCoroutine(ChangeMusic(0)); // ù ��° ���� Ŭ�� ���
        }
        else if (SceneManager.GetActiveScene().name == "01.StartWaitRoom")
        {
            StartCoroutine(ChangeMusic(1)); // �� ��° ���� Ŭ�� ���
        }
        else if (SceneManager.GetActiveScene().name == "01.WaitRoom")
        {
            StartCoroutine(ChangeMusic(1)); // �� ��° ���� Ŭ�� ���
        }
        else if (SceneManager.GetActiveScene().name == "EV.Arrow Event")
        {
            StartCoroutine(ChangeMusic(2)); // �� ��° ���� Ŭ�� ���
        }
        else if (SceneManager.GetActiveScene().name == "EV.LogEvent")
        {
            StartCoroutine(ChangeMusic(3)); // �� ��° ���� Ŭ�� ���
        }
        else if (SceneManager.GetActiveScene().name == "Ev.KingSlime")
        {
            StartCoroutine(ChangeMusic(4));
        }
    }

    public IEnumerator ChangeMusic(int clipIndex)
    {
        // ���� ��� ���� ������ ���� ��� ���̵� �ƿ�
        if (musicSource.isPlaying)
        {
            yield return StartCoroutine(FadeOut(musicSource, fadeDuration));
        }

        // ���ο� ���� Ŭ�� ����
        musicSource.clip = sceneMusicClips[clipIndex];

        // ���̵� ��
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
        audioSource.Stop(); // ���� ����
    }

    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        audioSource.volume = 0; // ������ 0���� ����
        audioSource.Play(); // ���� ��� ����

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, targetVolume * masterVolume * musicVolume, t / duration); // ���� �������� ����
            yield return null;
        }

        audioSource.volume = targetVolume * masterVolume * musicVolume; // ���� ���� ����
    }

    // ������ ���� ���� �޼ҵ�
    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        UpdateMusicVolume();
    }

    // ���� ���� ���� �޼ҵ�
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        UpdateMusicVolume();
    }

    // ���� ���� ������Ʈ �޼ҵ�
    private void UpdateMusicVolume()
    {
        musicSource.volume = targetVolume * masterVolume * musicVolume;
    }
}
