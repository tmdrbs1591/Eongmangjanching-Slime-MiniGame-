using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement; // 씬을 변경하려면 필요

public class VikingSlimeEvent : TimeManager
{
    private bool isScoreAdded = false;  // 점수가 이미 추가되었는지 확인하는 변수

    private Transform target; // 설정된 타겟

    protected override void Start()
    {
        base.Start();
        // 마스터 클라이언트만 로그를 생성하도록 설정
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(InitialTargetSetup());
        }
    }

    void Update()
    {
        TimeEnd();
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

        foreach (var playerScore in GameManager.instance.playerScores)
        {
            // 한 명이라도 죽지 않은 플레이어가 있으면 false
            if (!playerScore.isDeath)
            {
                allPlayersDead = false;
                break; // 더 이상 검사할 필요 없음
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
                AddScoresToAlivePlayers();
                StartCoroutine(FadeScene());
            }
        }
    }

    private void HandleGameEnd()
    {
        // 게임 종료 및 점수 추가 처리
        if (!isScoreAdded)
        {
            AddScoresToAlivePlayers();
            StartCoroutine(FadeScene());
        }
    }

    private void AddScoresToAlivePlayers()
    {
        foreach (var playerScore in GameManager.instance.playerScores)
        {
            if (!playerScore.isDeath && PhotonNetwork.IsMasterClient)
            {
                Debug.Log("점수 추가");
                playerScore.AddScore(1000); // 점수 추가
            }
        }

        // 점수가 추가되었음을 기록
        isScoreAdded = true;
    }

    private IEnumerator InitialTargetSetup()
    {
        yield return new WaitForSeconds(1f); // 초기화 지연 시간
        SetTarget(); // 타겟 설정
    }

    private void SetTarget()
    {
        // 타겟 설정 로직 (여기서 타겟을 선택합니다)
        // 예: target = 선택된 플레이어의 Transform;

        // 타겟을 설정한 후 다른 클라이언트와 동기화
        photonView.RPC("SyncTarget", RpcTarget.All, target.position, target.rotation);
    }

    [PunRPC]
    private void SyncTarget(Vector3 targetPosition, Quaternion targetRotation)
    {
        target.position = targetPosition;
        target.rotation = targetRotation;
    }
}
