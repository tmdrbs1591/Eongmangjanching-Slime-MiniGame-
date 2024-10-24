using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance { get; private set; }

    public SkinnedMeshRenderer dumiSkinMeshRenderer;
    public List<PlayerScore> playerScores = new List<PlayerScore>();

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 씬이 변경되어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);  // 이미 인스턴스가 있으면 자신을 파괴
        }
    }

    // 매 프레임마다 점수 비교하여 최고 점수 플레이어에게 왕관을 활성화
    public void UpdateCrown()
    {
        // 현재 가장 높은 점수와 해당 플레이어를 추적
        PlayerScore highestScorePlayer = null;
        float highestScore = float.MinValue;

        foreach (var playerScore in playerScores)
        {
            if (playerScore.currentScore > highestScore)
            {
                highestScore = playerScore.currentScore;
                highestScorePlayer = playerScore;
            }
        }

        // 모든 플레이어의 왕관 비활성화
        foreach (var playerScore in playerScores)
        {
            if (playerScore.crown != null)
            {
                playerScore.crown.SetActive(false);
            }
        }

        // 가장 높은 점수를 가진 플레이어의 왕관 활성화
        if (highestScorePlayer != null && highestScorePlayer.crown != null)
        {
            highestScorePlayer.crown.SetActive(true);
        }
    }

    #region 뿅망치 이벤트
    public void HammerTrue()
    {
        foreach (var playerScore in playerScores)
        {
            if (playerScore.crown != null)
            {
                playerScore.hammer.SetActive(true);
            }
        }
    }

    public void HammerFalse()
    {
        foreach (var playerScore in playerScores)
        {
            if (playerScore.crown != null)
            {
                playerScore.hammer.SetActive(false);
            }
        }
    }
    #endregion

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
            foreach (var playerScore in playerScores)
            {
                    playerScore.isDeath =false;

                var playerscript = playerScore.gameObject.GetComponent<PlayerScript>();
                playerscript.isStun = false;
            }
        }
    }
}
