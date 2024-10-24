using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPoint : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();  // SpawnPoint ����Ʈ

    private bool hasMovedPlayers = false; // �÷��̾ �̵��ߴ��� ���θ� ����

    // Start is called before the first frame update
    void Start()
    {
        // PlayerScore ����Ʈ���� �� �÷��̾��� ��ġ�� spawnPoints ����Ʈ�� �߰�
        AddPlayerPositionsToSpawnPoints();

        // �÷��̾��� ��ġ�� spawnPoint�� �̵�
        MoveAllPlayersToSpawnPoints();
    }

    // PlayerScore ����Ʈ���� �÷��̾��� ��ġ�� SpawnPoint ����Ʈ�� �߰��ϴ� �Լ�
    void AddPlayerPositionsToSpawnPoints()
    {
        foreach (var playerScore in GameManager.instance.playerScores)
        {
            // PlayerScore���� GameObject�� �����ͼ� ��ġ�� SpawnPoint ����Ʈ�� �߰�
            spawnPoints.Add(playerScore.gameObject.transform);
        }
    }

    // ��� �÷��̾ ������ ��ġ�� �̵���Ű�� �Լ�
    void MoveAllPlayersToSpawnPoints()
    {
        if (hasMovedPlayers) return; // �̹� �̵��ߴٸ� �ƹ� �͵� ���� ����

        for (int i = 0; i < GameManager.instance.playerScores.Count; i++)
        {
            PlayerScore playerScore = GameManager.instance.playerScores[i];
            if (i < spawnPoints.Count) // spawnPoints�� �� ���� ��쿡�� �̵�
            {
                MovePlayerToSpawnPoint(playerScore, spawnPoints[i]);
            }
        }

        hasMovedPlayers = true; // �÷��̾� �̵� �Ϸ� �÷��� ����
    }

    // �÷��̾ ������ ��ġ�� �̵���Ű�� �Լ�
    void MovePlayerToSpawnPoint(PlayerScore playerScore, Transform spawnPoint)
    {
        // Photon�� ����Ͽ� ��ġ�� ����ȭ
        if (PhotonNetwork.IsMasterClient) // ������ Ŭ���̾�Ʈ�� �̵� ó��
        {
            playerScore.gameObject.transform.position = spawnPoint.position;
            // Photon���� ��ġ�� ����ȭ
            photonView.RPC("SyncPlayerPosition", RpcTarget.Others, playerScore.photonView.ViewID, spawnPoint.position);
        }
    }

    [PunRPC]
    void SyncPlayerPosition(int viewID, Vector3 newPosition)
    {
        PhotonView.Find(viewID).transform.position = newPosition;
    }
}
