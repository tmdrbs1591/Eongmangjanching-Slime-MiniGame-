using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LogMiniGame : MonoBehaviourPunCallbacks
{
    // 각 위치를 Transform으로 선언
    [SerializeField] Transform leftPos;
    [SerializeField] Transform rightPos;
    [SerializeField] Transform frontPos;
    [SerializeField] Transform backPos;

    [SerializeField] GameObject logPrefabs;
    [SerializeField] float moveSpeed = 5f; // 이동 속도 설정

    // Start is called before the first frame update
    void Start()
    {
        // 마스터 클라이언트만 로그를 생성하도록 설정
        if (PhotonNetwork.IsMasterClient)
        {
            // 2초마다 EventStart 함수를 반복 실행
            InvokeRepeating("EventStart", 2.0f, 2.0f);
        }
    }

    void EventStart()
    {
        // 4개의 위치 중 랜덤으로 선택
        int randomIndex = Random.Range(0, 4);
        Transform spawnPoint = null;

        switch (randomIndex)
        {
            case 0:
                spawnPoint = leftPos;
                break;
            case 1:
                spawnPoint = rightPos;
                break;
            case 2:
                spawnPoint = frontPos;
                break;
            case 3:
                spawnPoint = backPos;
                break;
        }

        // 네트워크에서 로그 프리팹 생성 (PhotonView가 포함된 프리팹)
        PhotonNetwork.Instantiate(logPrefabs.name, spawnPoint.position, spawnPoint.rotation);
    }
}
