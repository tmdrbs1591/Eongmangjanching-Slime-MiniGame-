using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class ScoreListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text nickNameText;        // 플레이어 이름을 표시할 텍스트
    [SerializeField] TMP_Text scoreText;            // 점수를 표시할 텍스트
    Player player;

    [SerializeField] public GameObject firstImage;        // 1등 이미지
    [SerializeField] public GameObject secondImage;       // 2등 이미지
    [SerializeField] public GameObject thirdImage;        // 3등 이미지
    [SerializeField] public GameObject fourthImage;       // 4등 이미지

    private void Awake()
    {
        GameManager.instance.scoreListItem.Add(this);
    }

    public void Setup(Player _player)
    {
        player = _player;
        nickNameText.text = _player.NickName;
        UpdateHPBar(); // 초기 HP 및 레벨 업데이트
    }

    void UpdateHPBar()
    {
        PlayerScore playerStats = GetPlayerStatsByNickName(nickNameText.text);
        if (playerStats != null)
        {
            // 점수 텍스트 설정
            scoreText.text = playerStats.currentScore.ToString();
        }
    }

    // 닉네임으로 PlayerStats를 찾는 메서드
    PlayerScore GetPlayerStatsByNickName(string nickName)
    {
        foreach (GameObject playerObject in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView photonView = playerObject.GetComponent<PhotonView>();
            if (photonView != null && photonView.Owner.NickName == nickName)
            {
                return playerObject.GetComponent<PlayerScore>();
            }
        }
        return null; // 플레이어를 찾지 못한 경우
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        UpdateHPBar();  // 매 프레임 HP와 레벨을 업데이트 (최신 상태 반영)
    }

    // 순위 이미지 업데이트 메서드
    public void UpdateRankImage(int rank)
    {
        firstImage.SetActive(rank == 1);
        secondImage.SetActive(rank == 2);
        thirdImage.SetActive(rank == 3);
        fourthImage.SetActive(rank == 4);
    }
}
