using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance { get; private set; }

    [Header("����")]
    public SkinnedMeshRenderer dumiSkinMeshRenderer;
    [SerializeField] public GameObject vikingHat;
    [SerializeField] public GameObject sproutHat;
    [SerializeField] public GameObject leafHat;
    [SerializeField] public GameObject metalHat;

    private List<GameObject> hats;

    public List<PlayerScore> playerScores = new List<PlayerScore>();
    public List<ScoreListItem> scoreListItem = new List<ScoreListItem>();


    [SerializeField] public GameObject grayScaleScreen;

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

        hats = new List<GameObject> { vikingHat, sproutHat, leafHat, metalHat };

    }
    private void Update()
    {
        // playerScores ����Ʈ���� Missing�� �׸� ����
        for (int i = playerScores.Count - 1; i >= 0; i--)
        {
            if (playerScores[i] == null)
            {
                playerScores.RemoveAt(i);
            }
        }

        // scoreListItem ����Ʈ���� Missing�� �׸� ����
        for (int i = scoreListItem.Count - 1; i >= 0; i--)
        {
            if (scoreListItem[i] == null)
            {
                scoreListItem.RemoveAt(i);
            }
        }
    }




    public void HatActive(GameObject selectedHat)
    {
        // ��� ���ڸ� ��Ȱ��ȭ�մϴ�.
        foreach (GameObject hat in hats)
        {
            hat.SetActive(false);
        }

        // ���õ� ���ڸ� Ȱ��ȭ�մϴ�.
        selectedHat.SetActive(true);
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
                scoreListItem[i].Setup(sortedScores[i].GetComponent<PhotonView>().Owner); // ���� ������Ʈ
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

        #region ȭ�� �̺�Ʈ
    public void ArrowShotTrue()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var playerScore in playerScores)
            {
                playerScore.arrowShot.SetActive(true);
            }
            photonView.RPC("SyncArrowShotTrue", RpcTarget.Others); // �ٸ� Ŭ���̾�Ʈ�� ����ȭ
        }
    }

    [PunRPC]
    public void SyncArrowShotTrue()
    {
        foreach (var playerScore in playerScores)
        {
            playerScore.arrowShot.SetActive(true);
        }
    }

    public void ArrowShotFalse()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var playerScore in playerScores)
            {
                playerScore.arrowShot.SetActive(false);
            }
            photonView.RPC("SyncArrowShotFalse", RpcTarget.Others); // �ٸ� Ŭ���̾�Ʈ�� ����ȭ
        }
    }

    [PunRPC]
    public void SyncArrowShotFalse()
    {
        foreach (var playerScore in playerScores)
        {
            playerScore.arrowShot.SetActive(false);
        }
    }
    #endregion
    #region ȭ�� �̺�Ʈ
    public void ArrowBrainTrue()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var playerScore in playerScores)
            {
                playerScore.arrowBrain.SetActive(true);
            }
            photonView.RPC("SyncArrowBrainTrue", RpcTarget.Others); // �ٸ� Ŭ���̾�Ʈ�� ����ȭ
        }
    }

    [PunRPC]
    public void SyncArrowBrainTrue()
    {
        foreach (var playerScore in playerScores)
        {
            playerScore.arrowBrain.SetActive(true);
        }
    }

    public void ArrowBrainFalse()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var playerScore in playerScores)
            {
                playerScore.arrowBrain.SetActive(false);
            }
            photonView.RPC("SyncArrowBrainFalse", RpcTarget.Others); // �ٸ� Ŭ���̾�Ʈ�� ����ȭ
        }
    }

    [PunRPC]
    public void SyncArrowBrainFalse()
    {
        foreach (var playerScore in playerScores)
        {
            playerScore.arrowBrain.SetActive(false);
        }
    }
    #endregion

    #region �и�ġ �̺�Ʈ
    public void HammerTrue()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var playerScore in playerScores)
            {
                if (playerScore.crown != null)
                {
                    playerScore.hammer.SetActive(true);
                }
            }
            photonView.RPC("SyncHammerTrue", RpcTarget.Others); // �ٸ� Ŭ���̾�Ʈ�� ����ȭ
        }
    }

    [PunRPC]
    public void SyncHammerTrue()
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
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var playerScore in playerScores)
            {
                if (playerScore.crown != null)
                {
                    playerScore.hammer.SetActive(false);
                }
            }
            photonView.RPC("SyncHammerFalse", RpcTarget.Others); // �ٸ� Ŭ���̾�Ʈ�� ����ȭ
        }
    }

    [PunRPC]
    public void SyncHammerFalse()
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
