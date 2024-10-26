using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HammerMiniGame : TimeManager
{
    private bool isScoreAdded = false;  // 점수가 이미 추가되었는지 확인하는 변수

    protected override void Start()
    {
        base.Start();
        EventStart();
    }
    void Update()
    {
        TimeEnd();
    }

    void EventStart()
    {
        GameManager.instance.HammerTrue();
    }
    void TimeEnd()
    {
        // 카운트다운 타이머가 0 이하로 내려가면 숫자 감소
        countdownTimer -= Time.deltaTime;

        // 살아있는 플레이어 수를 확인
        int alivePlayerCount = 0;

        foreach (var playerScore in GameManager.instance.playerScores)
        {
            // 살아있는 플레이어 수 증가
            if (!playerScore.isDeath)
            {
                alivePlayerCount++;
            }
        }

        // 살아있는 플레이어가 1명이라면 게임 종료
        if (alivePlayerCount == 1 && !isScoreAdded)
        {
            foreach (var playerScore in GameManager.instance.playerScores)
            {
                // 점수 추가 (마스터 클라이언트인 경우)
                if (!playerScore.isDeath && PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("점수 추가");
                    playerScore.AddScore(1000);  // 점수 추가
                }
            }

            // 점수가 추가되었음을 기록
            isScoreAdded = true;

            GameManager.instance.HammerFalse();

            StartCoroutine(FadeScene());

            // 타이머를 멈추기 위해 함수 종료
            return;
        }

        if (countdownTimer <= 0)
        {
            // 1초가 지나면 카운트다운 숫자 감소
            timeRemaining -= 1;

            // 텍스트 갱신
            countdownText.text = timeRemaining.ToString();

            // 타이머 초기화
            countdownTimer = 1f;

            // timeRemaining이 0 이하일 때 점수를 추가
            if (timeRemaining <= 0 && !isScoreAdded)
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

                GameManager.instance.HammerFalse();

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