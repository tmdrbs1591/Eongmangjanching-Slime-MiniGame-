using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class StageManager : MonoBehaviourPunCallbacks
{
    public static StageManager instance;

    [SerializeField] float currentStage;
    [SerializeField] float maxStage;

    private bool isGameEnding = false; // �� ��ȯ ������ ���θ� üũ

    private void Awake()
    {
        // Singleton ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!isGameEnding && currentStage >= maxStage) // �� ���� ����
        {
            isGameEnding = true; // ���� ���� �÷��� ����
            Debug.Log("���ӳ�!!!");
            SceneManager.LoadScene("02.ResultRoom");
        }
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (SceneManager.GetActiveScene().name == "01.WaitRoom")
        {
            currentStage++;
            Debug.Log($"���� ��������: {currentStage}");
        }
    }
}
