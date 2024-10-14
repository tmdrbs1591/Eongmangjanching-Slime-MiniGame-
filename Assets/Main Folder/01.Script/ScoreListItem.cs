using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;  // �����̴��� ����ϱ� ���� ���ӽ����̽�

public class ScoreListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text nickNameText;        // �÷��̾� �̸��� ǥ���� �ؽ�Ʈ
    [SerializeField] TMP_Text scoreText;   // ������ ǥ���� �ؽ�Ʈ
    Player player;

    public void Setup(Player _player)
    {
        player = _player;
        nickNameText.text = _player.NickName;
        UpdateHPBar(); // �ʱ� HP �� ���� ������Ʈ
    }

    // HP �ٿ� ������ ������Ʈ�ϴ� �޼���
    void UpdateHPBar()
    {
        PlayerScore playerStats = GetPlayerStatsByNickName(nickNameText.text);
        if (playerStats != null)
        {

            // ���� �ؽ�Ʈ ����
            scoreText.text = playerStats.currentScore.ToString();
        }
    }

    // �г������� PlayerStats�� ã�� �޼���
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
        return null; // �÷��̾ ã�� ���� ���
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
        UpdateHPBar();  // �� ������ HP�� ������ ������Ʈ (�ֽ� ���� �ݿ�)
    }
}
