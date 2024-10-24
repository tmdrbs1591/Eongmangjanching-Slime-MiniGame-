using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement; // 씬을 변경하려면 필요

public class LogMiniGame : TimeManager
{
    [SerializeField] Transform leftPos;
    [SerializeField] Transform rightPos;
    [SerializeField] Transform frontPos;
    [SerializeField] Transform backPos;

    [SerializeField] GameObject logPrefabs;
    [SerializeField] float moveSpeed = 5f; // 이동 속도 설정

    private bool isScoreAdded = false;  // 점수가 이미 추가되었는지 확인하는 변수

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

    private Transform lastSpawnPoint = null; // 마지막으로 선택된 위치를 저장

    void EventStart()
    {
        // 이전에 선택된 위치에 따라 새로운 위치를 결정
        Transform spawnPoint = null;

        if (lastSpawnPoint == leftPos || lastSpawnPoint == rightPos)
        {
            // lastSpawnPoint가 leftPos 또는 rightPos인 경우 frontPos나 backPos 중에서 선택
            spawnPoint = Random.Range(0, 2) == 0 ? frontPos : backPos;
        }
        else
        {
            // lastSpawnPoint가 frontPos 또는 backPos인 경우 leftPos나 rightPos 중에서 선택
            spawnPoint = Random.Range(0, 2) == 0 ? leftPos : rightPos;
        }

        // 네트워크에서 로그 프리팹 생성 (PhotonView가 포함된 프리팹)
        PhotonNetwork.Instantiate(logPrefabs.name, spawnPoint.position, spawnPoint.rotation);

        // 마지막 선택된 위치 갱신
        lastSpawnPoint = spawnPoint;
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

            if (timeRemaining <= 0 && !isScoreAdded)
            {
                // 점수를 한 번만 추가하도록 체크
                foreach (var playerScore in GameManager.instance.playerScores)
                {
                    // 플레이어가 죽지 않았으면 점수 추가
                    if (!playerScore.isDeath && PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("점수 추가");
                        playerScore.AddScore(1000);  // 점수 추가
                    }
                }

                // 점수가 추가되었음을 기록
                isScoreAdded = true;

                StartCoroutine(FadeScene());
            }
        }
    }
}