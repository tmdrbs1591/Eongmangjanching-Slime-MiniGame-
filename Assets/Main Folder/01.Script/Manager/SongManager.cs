using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SongManager : MonoBehaviourPunCallbacks
{
    public static SongManager Instance;

    private AudioSource audioSource; // ����� �ҽ�
    public List<AudioClip> sceneMusicClips; // ���� �´� ���� Ŭ�� ����Ʈ
    private float fadeDuration = 0.3f; // ���̵� �ð� ����
    public float targetVolume = 0.8f; // ���� ������ ���� (1f ��� ���ϴ� ������ ����)

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;

        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource �߰�
        audioSource.volume = 1f; // �ʱ� ���� ����
        audioSource.loop = true; // �ݺ� ��� Ȱ��ȭ
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
        // �ٸ� ���� �´� ���� �߰�
    }

    public IEnumerator ChangeMusic(int clipIndex)
    {
        // ���� ��� ���� ������ ���� ��� ���̵� �ƿ�
        if (audioSource.isPlaying)
        {
            yield return StartCoroutine(FadeOut(audioSource, fadeDuration));
        }

        // ���ο� ���� Ŭ�� ����
        audioSource.clip = sceneMusicClips[clipIndex];
        audioSource.Play();

        // ���̵� ��
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
        audioSource.Stop(); // ���� ����
    }

    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        audioSource.volume = 0; // ������ 0���� ����
        audioSource.Play(); // ���� ��� ����

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, targetVolume, t / duration); // ���� �������� ����
            yield return null;
        }

        audioSource.volume = targetVolume; // ���� ���� ����
    }
}
