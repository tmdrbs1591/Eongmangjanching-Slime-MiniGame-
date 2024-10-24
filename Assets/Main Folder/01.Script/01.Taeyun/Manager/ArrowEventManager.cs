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

    private bool isScoreAdded = false;  // ������ �̹� �߰��Ǿ����� Ȯ���ϴ� ����


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

        readyText.text = "�Է��ϼ���!";
        isTypingTime = true;
        yield return new WaitForSeconds(6f);

        ResetBord();
        yield return StartCoroutine(StartPhase(6, 3));

        readyText.text = "�Է��ϼ���!";
        isTypingTime = true;
        yield return new WaitForSeconds(6f);

        ResetBord();
        yield return StartCoroutine(StartPhase(10, 5));

        readyText.text = "�Է��ϼ���!";
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
                return "��";
            case 1:
                return "��";
            case 2:
                return "��";
            case 3:
                return "��";
        }
        return null;
    }

    public bool CheckArrow(string arrow, int index)
    {
        // �ε��� ���� �˻� �߰�
        if (index >= arrowList.Count)
        {
            Debug.LogError($"Index out of range: {index}. arrowList.Count is {arrowList.Count}");
            return false;
        }

        if (arrowList[index] == arrow)
        {
            Debug.Log("������! ���� : " + arrow);
            return true;
        }
        else
        {
            Debug.Log("Ʋ�Ⱦ��̤� �Է��� �� : " + arrow + " ���� : " + arrowList[index]);
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
        // ī��Ʈ�ٿ� Ÿ�̸Ӱ� 0 ���Ϸ� �������� ���� ����
        countdownTimer -= Time.deltaTime;

        if (countdownTimer <= 0)
        {
            // 1�ʰ� ������ ī��Ʈ�ٿ� ���� ����
            timeRemaining -= 1;

            // �ؽ�Ʈ ����
            countdownText.text = timeRemaining.ToString();

            // Ÿ�̸� �ʱ�ȭ
            countdownTimer = 1f;

            if (timeRemaining <= 0 && !isScoreAdded)
            {
                // ������ �� ���� �߰��ϵ��� üũ
                foreach (var playerScore in GameManager.instance.playerScores)
                {
                    // �÷��̾ ���� �ʾ����� ���� �߰�
                    if (!playerScore.isDeath && PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("���� �߰�");
                        playerScore.AddScore(1000);  // ���� �߰�
                    }
                }

                // ������ �߰��Ǿ����� ���
                isScoreAdded = true;

                GameManager.instance.HammerFalse();

                StartCoroutine(FadeScene());
            }
        }
    }
}
