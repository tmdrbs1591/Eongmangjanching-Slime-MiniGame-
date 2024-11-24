using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ResultSpawnPoint : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>(); // 순서에 따른 스폰 포인트 리스트
    private bool hasMovedPlayers = false; // 플레이어가 이동했는지 여부를 추적

    void Start()
    {
        // 점수에 따라 플레이어를 순서대로 스폰 포인트에 배치
        MovePlayersToSpawnPointsByScore();
    }

    void MovePlayersToSpawnPointsByScore()
    {
        if (hasMovedPlayers) return; // 이미 이동했다면 실행하지 않음

        // GameManager에서 정렬된 플레이어 리스트 가져오기
        List<PlayerScore> sortedPlayers = GetSortedPlayerScores();

        // 스폰 포인트에 플레이어 배치
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            if (i < spawnPoints.Count) // 스폰 포인트가 충분한 경우에만 처리
            {
                MovePlayerToSpawnPoint(sortedPlayers[i], spawnPoints[i]);
            }
        }

        hasMovedPlayers = true; // 이동 완료 플래그 설정
    }

    List<PlayerScore> GetSortedPlayerScores()
    {
        // GameManager에서 점수에 따라 정렬된 리스트 생성
        List<PlayerScore> sortedPlayers = new List<PlayerScore>(GameManager.instance.playerScores);
        sortedPlayers.Sort((a, b) => b.currentScore.CompareTo(a.currentScore)); // 점수 내림차순 정렬
        return sortedPlayers;
    }

    void MovePlayerToSpawnPoint(PlayerScore playerScore, Transform spawnPoint)
    {
        // 마스터 클라이언트가 위치 동기화
        if (PhotonNetwork.IsMasterClient)
        {
            playerScore.gameObject.transform.position = spawnPoint.position;

            // Photon을 통해 다른 클라이언트에 위치 동기화
            photonView.RPC("SyncPlayerPosition", RpcTarget.Others, playerScore.photonView.ViewID, spawnPoint.position);
        }
    }

    [PunRPC]
    void SyncPlayerPosition(int viewID, Vector3 newPosition)
    {
        PhotonView.Find(viewID).transform.position = newPosition;
    }
}
