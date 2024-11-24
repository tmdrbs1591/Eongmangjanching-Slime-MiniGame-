using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.IO;
public class StageManager : MonoBehaviourPunCallbacks
{
    public static StageManager instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    [SerializeField] float currentStage;
    [SerializeField] float maxStage;


    private void Update()
    {
        if (currentStage >= maxStage)
        {
            Debug.Log("게임끝!!!");
            SceneManager.LoadScene("ResultStage");
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

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (SceneManager.GetActiveScene().name == "01.WaitRoom")
        {
            currentStage++;
        }
    }
}
