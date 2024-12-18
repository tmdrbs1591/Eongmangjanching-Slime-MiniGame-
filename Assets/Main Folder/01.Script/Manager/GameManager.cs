using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance { get; private set; }

    [Header("더미")]
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

        hats = new List<GameObject> { vikingHat, sproutHat, leafHat, metalHat };

    }
    private void Update()
    {
        // playerScores 리스트에서 Missing된 항목 제거
        for (int i = playerScores.Count - 1; i >= 0; i--)
        {
            if (playerScores[i] == null)
            {
                playerScores.RemoveAt(i);
            }
        }

        // scoreListItem 리스트에서 Missing된 항목 제거
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
        // 모든 모자를 비활성화합니다.
        foreach (GameObject hat in hats)
        {
            hat.SetActive(false);
        }

        // 선택된 모자만 활성화합니다.
        selectedHat.SetActive(true);
    }





    // 점수 기반 순위 업데이트 메서드
    public void UpdateScoreRanking()
    {
        // 점수 기준으로 정렬된 리스트 생성
        List<PlayerScore> sortedScores = new List<PlayerScore>(playerScores);
        sortedScores.Sort((a, b) => b.currentScore.CompareTo(a.currentScore)); // 내림차순 정렬

        // 순위에 따라 ScoreListItem 업데이트
        for (int i = 0; i < scoreListItem.Count; i++)
        {
            if (i < sortedScores.Count)
            {
                int rank = i + 1;
                scoreListItem[i].Setup(sortedScores[i].GetComponent<PhotonView>().Owner); // 순위 업데이트
                scoreListItem[i].UpdateRankImage(rank); // 순위 이미지 업데이트
            }
            else
            {
                scoreListItem[i].UpdateRankImage(0); // 순위 밖인 경우 비활성화
            }
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

        #region 화살 이벤트
    public void ArrowShotTrue()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var playerScore in playerScores)
            {
                playerScore.arrowShot.SetActive(true);
            }
            photonView.RPC("SyncArrowShotTrue", RpcTarget.Others); // 다른 클라이언트에 동기화
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
            photonView.RPC("SyncArrowShotFalse", RpcTarget.Others); // 다른 클라이언트에 동기화
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
    #region 화살 이벤트
    public void ArrowBrainTrue()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var playerScore in playerScores)
            {
                playerScore.arrowBrain.SetActive(true);
            }
            photonView.RPC("SyncArrowBrainTrue", RpcTarget.Others); // 다른 클라이언트에 동기화
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
            photonView.RPC("SyncArrowBrainFalse", RpcTarget.Others); // 다른 클라이언트에 동기화
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

    #region 뿅망치 이벤트
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
            photonView.RPC("SyncHammerTrue", RpcTarget.Others); // 다른 클라이언트에 동기화
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
            photonView.RPC("SyncHammerFalse", RpcTarget.Others); // 다른 클라이언트에 동기화
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
        SceneManager.sceneLoaded -= OnSceneLoaded; // 구독 해제
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
            UpdateScoreRanking(); // 씬이 로드될 때 점수 순위 업데이트
        }
    }
}
