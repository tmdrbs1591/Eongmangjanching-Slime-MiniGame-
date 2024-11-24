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

    private bool isGameEnding = false; // 씬 전환 중인지 여부를 체크

    private void Awake()
    {
        // Singleton 패턴
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
        if (!isGameEnding && currentStage >= maxStage) // 한 번만 실행
        {
            isGameEnding = true; // 게임 종료 플래그 설정
            Debug.Log("게임끝!!!");
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
        SceneManager.sceneLoaded -= OnSceneLoaded; // 구독 해제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (SceneManager.GetActiveScene().name == "01.WaitRoom")
        {
            currentStage++;
            Debug.Log($"현재 스테이지: {currentStage}");
        }
    }
}
