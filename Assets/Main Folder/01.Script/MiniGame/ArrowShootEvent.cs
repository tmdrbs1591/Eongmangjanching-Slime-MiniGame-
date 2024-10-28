using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement; // 씬을 변경하려면 필요

public class ArrowShootEvent : TimeManager
{


    private bool isScoreAdded = false;  // 점수가 이미 추가되었는지 확인하는 변수

    protected override void Start()
    {
        base.Start();
        // 마스터 클라이언트만 로그를 생성하도록 설정
    }

    void Update()
    {
        TimeEnd();
    }

    private Transform lastSpawnPoint = null; // 마지막으로 선택된 위치를 저장

    void EventStart()
    {
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
            // 즉시 게임 종료를 위한 처리
            if (!isScoreAdded) // 점수를 추가하지 않았다면
            {
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

                // 게임 종료 및 씬 전환 처리
                StartCoroutine(FadeScene());
            }

            // 타이머를 1로 설정해도 타이머 감소를 막고, 더 이상의 업데이트는 하지 않음
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
                foreach (var playerScore in GameManager.instance.playerScores)
                {
                    // 플레이어가 죽지 않았으면 점수 추가
                    if (!playerScore.isDeath && PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("점수 추가");
                    }
                }

                // 점수가 추가되었음을 기록
                isScoreAdded = true;
                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(FadeScene());
                }
            }
        }
    }
    [PunRPC]
    public void ActivateFadeIn()
    {
        Fadein.SetActive(true); // 모든 클라이언트에서 FadeIn 활성화
    }

    protected IEnumerator FadeScene()
    {
        // FadeIn을 활성화하는 RPC 호출
        photonView.RPC("ActivateFadeIn", RpcTarget.All);
        yield return new WaitForSeconds(1.5f);

        // 씬 전환을 위한 RPC 호출
        photonView.RPC("LoadRandomScene", RpcTarget.All);
    }
}   