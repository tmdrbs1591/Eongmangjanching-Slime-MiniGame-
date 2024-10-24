using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance { get; private set; }

    public SkinnedMeshRenderer dumiSkinMeshRenderer;
    public List<PlayerScore> playerScores = new List<PlayerScore>();
    public List<ScoreListItem> scoreListItem = new List<ScoreListItem>();

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // ���� ����Ǿ �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject);  // �̹� �ν��Ͻ��� ������ �ڽ��� �ı�
        }
    }

    // ���� ��� ���� ������Ʈ �޼���
    public void UpdateScoreRanking()
    {
        // ���� �������� ���ĵ� ����Ʈ ����
        List<PlayerScore> sortedScores = new List<PlayerScore>(playerScores);
        sortedScores.Sort((a, b) => b.currentScore.CompareTo(a.currentScore)); // �������� ����

        // ������ ���� ScoreListItem ������Ʈ
        for (int i = 0; i < scoreListItem.Count; i++)
        {
            if (i < sortedScores.Count)
            {
                int rank = i + 1;
                scoreListItem[i].UpdateRankImage(rank); // ���� �̹��� ������Ʈ
            }
            else
            {
                scoreListItem[i].UpdateRankImage(0); // ���� ���� ��� ��Ȱ��ȭ
            }
        }
    }

    // �� �����Ӹ��� ���� ���Ͽ� �ְ� ���� �÷��̾�� �հ��� Ȱ��ȭ
    public void UpdateCrown()
    {
        // ���� ���� ���� ������ �ش� �÷��̾ ����
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

        // ��� �÷��̾��� �հ� ��Ȱ��ȭ
        foreach (var playerScore in playerScores)
        {
            if (playerScore.crown != null)
            {
                playerScore.crown.SetActive(false);
            }
        }

        // ���� ���� ������ ���� �÷��̾��� �հ� Ȱ��ȭ
        if (highestScorePlayer != null && highestScorePlayer.crown != null)
        {
            highestScorePlayer.crown.SetActive(true);
        }
    }

    #region �и�ġ �̺�Ʈ
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
        SceneManager.sceneLoaded -= OnSceneLoaded; // ���� ����
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (SceneManager.GetActiveScene().name == "01.WaitRoom")
        {
            foreach (var playerScore in playerScores)
            {
                playerScore.isDeath = false;

                var playerscript = playerScore.gameObject.GetComponent<PlayerScript>();
                playerscript.isStun = false;
            }
            UpdateScoreRanking(); // ���� �ε�� �� ���� ���� ������Ʈ
        }
    }
}
