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

                GameManager.instance.HammerFalse();

                StartCoroutine(FadeScene());
            }
        }
    }
}