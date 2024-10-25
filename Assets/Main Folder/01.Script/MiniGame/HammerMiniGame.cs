using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HammerMiniGame : TimeManager
{
    private bool isScoreAdded = false;  // ������ �̹� �߰��Ǿ����� Ȯ���ϴ� ����

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
        // ī��Ʈ�ٿ� Ÿ�̸Ӱ� 0 ���Ϸ� �������� ���� ����
        countdownTimer -= Time.deltaTime;

        // ����ִ� �÷��̾� ���� Ȯ��
        int alivePlayerCount = 0;

        foreach (var playerScore in GameManager.instance.playerScores)
        {
            // ����ִ� �÷��̾� �� ����
            if (!playerScore.isDeath)
            {
                alivePlayerCount++;
            }
        }

        // ����ִ� �÷��̾ 1���̶�� ���� ����
        if (alivePlayerCount == 1 && !isScoreAdded)
        {
            foreach (var playerScore in GameManager.instance.playerScores)
            {
                // ���� �߰� (������ Ŭ���̾�Ʈ�� ���)
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

            // Ÿ�̸Ӹ� ���߱� ���� �Լ� ����
            return;
        }

        if (countdownTimer <= 0)
        {
            // 1�ʰ� ������ ī��Ʈ�ٿ� ���� ����
            timeRemaining -= 1;

            // �ؽ�Ʈ ����
            countdownText.text = timeRemaining.ToString();

            // Ÿ�̸� �ʱ�ȭ
            countdownTimer = 1f;

            // timeRemaining�� 0 ������ �� ������ �߰�
            if (timeRemaining <= 0 && !isScoreAdded)
            {
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