// # System
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;

// # Unity
using UnityEngine;

public class ArrowEventManager : TimeManager
{
    public static ArrowEventManager instance;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI arrowText;
    [SerializeField] private TextMeshProUGUI readyText;

    [Header("ArrowList")]
    [SerializeField] private List<string> arrowList = new List<string>();
    public int arrowCount = 0;

    [Header("CurrentState")]
    public bool isTypingTime = false;

    private bool isScoreAdded = false;  // 점수가 이미 추가되었는지 확인하는 변수


    private void Awake()
    {
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(ArrowEvent());
    }
    void Update()
    {
        TimeEnd();
    }

    IEnumerator ArrowEvent()
    {
        GameManager.instance.ArrowBrainTrue();

        readyText.text = "Ready..";
        yield return new WaitForSeconds(1f);
        readyText.text = "Go!!";
        yield return new WaitForSeconds(1f);
        readyText.text = "";

        ResetBord();
        yield return StartCoroutine(StartPhase(4, 2));

        readyText.text = "입력하세요!";
        isTypingTime = true;
        yield return new WaitForSeconds(6f);

        ResetBord();
        yield return StartCoroutine(StartPhase(6, 3));

        readyText.text = "입력하세요!";
        isTypingTime = true;
        yield return new WaitForSeconds(6f);

        ResetBord();
        yield return StartCoroutine(StartPhase(10, 5));

        readyText.text = "입력하세요!";
        isTypingTime = true;
        yield return new WaitForSeconds(6f);
    }

    IEnumerator StartPhase(float _arrowCount, float time)
    {
        float delayTime = time / _arrowCount;

        Debug.Log(delayTime);

        for (int i = 0; i < _arrowCount; i++)
        {
            string arrow = RandomArrow();
            arrowText.text += arrow;
            arrowList.Add(arrow);
            yield return new WaitForSeconds(delayTime);
        }

        arrowText.text = "";
        arrowCount = arrowList.Count;
    }

    private string RandomArrow()
    {
        int i = Random.Range(0, 4);
        switch (i)
        {
            case 0:
                return "→";
            case 1:
                return "←";
            case 2:
                return "↑";
            case 3:
                return "↓";
        }
        return null;
    }

    public bool CheckArrow(string arrow, int index)
    {
        // 인덱스 범위 검사 추가
        if (index >= arrowList.Count)
        {
            Debug.LogError($"Index out of range: {index}. arrowList.Count is {arrowList.Count}");
            return false;
        }

        if (arrowList[index] == arrow)
        {
            Debug.Log("맞췄어요! 정답 : " + arrow);
            return true;
        }
        else
        {
            Debug.Log("틀렸어요ㅜㅜ 입력한 답 : " + arrow + " 정답 : " + arrowList[index]);
            return false;
        }
    }



    private void ResetBord()
    {
        readyText.text = "";
        arrowList.Clear();
        isTypingTime = false;
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
