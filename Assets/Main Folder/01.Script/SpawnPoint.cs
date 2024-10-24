using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPoint : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();  // SpawnPoint 리스트

    private bool hasMovedPlayers = false; // 플레이어가 이동했는지 여부를 추적

    // Start is called before the first frame update
    void Start()
    {
        // PlayerScore 리스트에서 각 플레이어의 위치를 spawnPoints 리스트에 추가
        AddPlayerPositionsToSpawnPoints();

        // 플레이어의 위치를 spawnPoint로 이동
        MoveAllPlayersToSpawnPoints();
    }

    // PlayerScore 리스트에서 플레이어의 위치를 SpawnPoint 리스트에 추가하는 함수
    void AddPlayerPositionsToSpawnPoints()
    {
        foreach (var playerScore in GameManager.instance.playerScores)
        {
            // PlayerScore에서 GameObject를 가져와서 위치를 SpawnPoint 리스트에 추가
            spawnPoints.Add(playerScore.gameObject.transform);
        }
    }

    // 모든 플레이어를 지정된 위치로 이동시키는 함수
    void MoveAllPlayersToSpawnPoints()
    {
        if (hasMovedPlayers) return; // 이미 이동했다면 아무 것도 하지 않음

        for (int i = 0; i < GameManager.instance.playerScores.Count; i++)
        {
            PlayerScore playerScore = GameManager.instance.playerScores[i];
            if (i < spawnPoints.Count) // spawnPoints가 더 많은 경우에만 이동
            {
                MovePlayerToSpawnPoint(playerScore, spawnPoints[i]);
            }
        }

        hasMovedPlayers = true; // 플레이어 이동 완료 플래그 설정
    }

    // 플레이어를 지정된 위치로 이동시키는 함수
    void MovePlayerToSpawnPoint(PlayerScore playerScore, Transform spawnPoint)
    {
        // Photon을 사용하여 위치를 동기화
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트가 이동 처리
        {
            playerScore.gameObject.transform.position = spawnPoint.position;
            // Photon으로 위치를 동기화
            photonView.RPC("SyncPlayerPosition", RpcTarget.Others, playerScore.photonView.ViewID, spawnPoint.position);
        }
    }

    [PunRPC]
    void SyncPlayerPosition(int viewID, Vector3 newPosition)
    {
        PhotonView.Find(viewID).transform.position = newPosition;
    }
}
