using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement; // 씬을 변경하려면 필요

public class LogMiniGame : TimeManager
{
    // 각 위치를 Transform으로 선언
    [SerializeField] Transform leftPos;
    [SerializeField] Transform rightPos;
    [SerializeField] Transform frontPos;
    [SerializeField] Transform backPos;

    [SerializeField] GameObject logPrefabs;
    [SerializeField] float moveSpeed = 5f; // 이동 속도 설정

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // 마스터 클라이언트만 로그를 생성하도록 설정
        if (PhotonNetwork.IsMasterClient)
        {
            // 2초마다 EventStart 함수를 반복 실행
            InvokeRepeating("EventStart", 2.0f, 2.0f);
        }
    }
    void Update()
    {
        TimeEnd();
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

    void TimeEnd()
    {
        // 카운트다운 타이머가 0 이하로 내려가면 숫자 감소
        countdownTimer -= Time.deltaTime;

        if (countdownTimer <= 0)
        {
            // 1초가 지나면 카운트다운 숫자 감소
            timeRemaining -= 1;

            // 텍스트 갱신
            countdownText.text = timeRemaining.ToString();

            // 타이머 초기화
            countdownTimer = 1f;

            if (timeRemaining <= 0)
            {
                foreach (var playerScore in GameManager.instance.playerScores)
                {
                    // 플레이어가 죽지 않았으면 점수 추가
                    if (!playerScore.isDeath && PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("점수 추가");
                        playerScore.AddScore(1000);  // 점수 추가
                        playerScore.isDeath = false;
                    }
                }
                StartCoroutine(FadeScene());
            }
        }
    }
}
