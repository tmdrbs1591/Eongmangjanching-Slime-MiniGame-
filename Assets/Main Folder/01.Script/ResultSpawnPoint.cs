using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ResultSpawnPoint : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>(); // ������ ���� ���� ����Ʈ ����Ʈ
    private bool hasMovedPlayers = false; // �÷��̾ �̵��ߴ��� ���θ� ����

    void Start()
    {
        // ������ ���� �÷��̾ ������� ���� ����Ʈ�� ��ġ
        MovePlayersToSpawnPointsByScore();
    }

    void MovePlayersToSpawnPointsByScore()
    {
        if (hasMovedPlayers) return; // �̹� �̵��ߴٸ� �������� ����

        // GameManager���� ���ĵ� �÷��̾� ����Ʈ ��������
        List<PlayerScore> sortedPlayers = GetSortedPlayerScores();

        // ���� ����Ʈ�� �÷��̾� ��ġ
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            if (i < spawnPoints.Count) // ���� ����Ʈ�� ����� ��쿡�� ó��
            {
                MovePlayerToSpawnPoint(sortedPlayers[i], spawnPoints[i]);
            }
        }

        hasMovedPlayers = true; // �̵� �Ϸ� �÷��� ����
    }

    List<PlayerScore> GetSortedPlayerScores()
    {
        // GameManager���� ������ ���� ���ĵ� ����Ʈ ����
        List<PlayerScore> sortedPlayers = new List<PlayerScore>(GameManager.instance.playerScores);
        sortedPlayers.Sort((a, b) => b.currentScore.CompareTo(a.currentScore)); // ���� �������� ����
        return sortedPlayers;
    }

    void MovePlayerToSpawnPoint(PlayerScore playerScore, Transform spawnPoint)
    {
        // ������ Ŭ���̾�Ʈ�� ��ġ ����ȭ
        if (PhotonNetwork.IsMasterClient)
        {
            playerScore.gameObject.transform.position = spawnPoint.position;

            // Photon�� ���� �ٸ� Ŭ���̾�Ʈ�� ��ġ ����ȭ
            photonView.RPC("SyncPlayerPosition", RpcTarget.Others, playerScore.photonView.ViewID, spawnPoint.position);
        }
    }

    [PunRPC]
    void SyncPlayerPosition(int viewID, Vector3 newPosition)
    {
        PhotonView.Find(viewID).transform.position = newPosition;
    }
}
