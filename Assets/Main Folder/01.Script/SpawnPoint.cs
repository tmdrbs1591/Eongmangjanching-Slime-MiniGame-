using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnPoint : MonoBehaviourPunCallbacks
{
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();  // SpawnPoint 리스트

    private int currentSpawnIndex = 0; // 현재 이동할 인덱스 추적

    // Start is called before the first frame update
    void Start()
    {
        // PlayerScore 리스트에서 각 플레이어의 위치를 spawnPoints 리스트에 추가
        AddPlayerPositionsToSpawnPoints();
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

    // Update is called once per frame
    void Update()
    {
        // spawnPoints가 비어있지 않고, 현재 인덱스가 playerScores보다 작은 경우
        if (spawnPoints.Count > 0 && currentSpawnIndex < GameManager.instance.playerScores.Count)
        {
            // 현재 플레이어와 해당 위치로 이동
            MovePlayerToSpawnPoint(GameManager.instance.playerScores[currentSpawnIndex], spawnPoints[currentSpawnIndex]);

            // 인덱스 증가 (다음 플레이어를 다음 위치로 이동)
            currentSpawnIndex++;
        }
    }

    // 플레이어를 지정된 위치로 이동시키는 함수
    void MovePlayerToSpawnPoint(PlayerScore playerScore, Transform spawnPoint)
    {
        playerScore.gameObject.transform.position = spawnPoint.position;
    }
}
