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
        TimeEnd();
    }

    private Transform lastSpawnPoint = null; // 마지막으로 선택된 위치를 저장

    void EventStart()
    {
    }

    void TimeEnd()
    {
        // 죽은 플레이어를 제거하기 위한 리스트 업데이트
        GameManager.instance.playerScores.RemoveAll(playerScore => playerScore.isDeath);

        // 모든 플레이어가 죽었는지 검사
        bool allPlayersDead = GameManager.instance.playerScores.Count == 0; // 모든 플레이어가 죽었음을 나타내는 변수

        // 모든 플레이어가 죽었다면 즉시 게임 종료
        if (allPlayersDead)
        {
            if (!isScoreAdded) // 점수를 추가하지 않았다면
            {
                foreach (var playerScore in GameManager.instance.playerScores)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("점수 추가");
                        playerScore.AddScore(1000);  // 점수 추가
                    }
                }

                isScoreAdded = true;
                StartCoroutine(FadeScene());
            }

            return;
        }

        // 카운트다운 타이머가 0 이하로 내려가면 숫자 감소
        countdownTimer -= Time.deltaTime;

        if (countdownTimer <= 0)
        {
            timeRemaining -= 1;
            countdownText.text = timeRemaining.ToString();
            countdownTimer = 1f;

            if (timeRemaining <= 0 && !isScoreAdded)
            {
                foreach (var playerScore in GameManager.instance.playerScores)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("점수 추가");
                        playerScore.AddScore(1000);  // 점수 추가
                    }
                }

                isScoreAdded = true;
                StartCoroutine(FadeScene());
            }
        }
    }
}
