using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement; // 씬을 변경하려면 필요

public class VikingSlimeEvent : TimeManager
{
    private bool isScoreAdded = false;  // 점수가 이미 추가되었는지 확인하는 변수

    protected override void Start()
    {
        base.Start();
        // 마스터 클라이언트만 로그를 생성하도록 설정
    }

    void Update()
    {
        TimeEnd(); // 매 프레임마다 타이머 체크
    }

    private Transform lastSpawnPoint = null; // 마지막으로 선택된 위치를 저장

    void EventStart()
    {
        // 이벤트 시작 로직 추가
    }

    void TimeEnd()
    {
        // 모든 플레이어가 죽었는지 검사
        bool allPlayersDead = true; // 모든 플레이어가 죽었음을 나타내는 변수
        List<PlayerScore> alivePlayers = new List<PlayerScore>(); // 살아있는 플레이어 목록

        foreach (var playerScore in GameManager.instance.playerScores)
        {
            if (!playerScore.isDeath)
            {
                allPlayersDead = false; // 한 명이라도 죽지 않았으면 false
                alivePlayers.Add(playerScore); // 살아있는 플레이어 목록에 추가
            }
        }

        // 모든 플레이어가 죽었다면 즉시 게임 종료
        if (allPlayersDead)
        {
            HandleGameEnd();
            return;
        }

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
                AddScoresToAlivePlayers(alivePlayers);
                StartCoroutine(FadeScene());
            }
        }
    }

    private void HandleGameEnd()
    {
        // 게임 종료 및 점수 추가 처리
        if (!isScoreAdded)
        {
            AddScoresToAlivePlayers(GameManager.instance.playerScores);
            StartCoroutine(FadeScene());
        }
    }

    private void AddScoresToAlivePlayers(List<PlayerScore> alivePlayers)
    {
        foreach (var playerScore in alivePlayers)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("점수 추가");
                playerScore.AddScore(1000); // 점수 추가
            }
        }

        // 점수가 추가되었음을 기록
        isScoreAdded = true;
    }
}
