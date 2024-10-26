using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement; // ���� �����Ϸ��� �ʿ�

public class VikingSlimeEvent : TimeManager
{
    private bool isScoreAdded = false;  // ������ �̹� �߰��Ǿ����� Ȯ���ϴ� ����

    protected override void Start()
    {
        base.Start();
        // ������ Ŭ���̾�Ʈ�� �α׸� �����ϵ��� ����
    }

    void Update()
    {
        TimeEnd();
    }

    private Transform lastSpawnPoint = null; // ���������� ���õ� ��ġ�� ����

    void EventStart()
    {
    }

    void TimeEnd()
    {
        // ���� �÷��̾ �����ϱ� ���� ����Ʈ ������Ʈ
        GameManager.instance.playerScores.RemoveAll(playerScore => playerScore.isDeath);

        // ��� �÷��̾ �׾����� �˻�
        bool allPlayersDead = GameManager.instance.playerScores.Count == 0; // ��� �÷��̾ �׾����� ��Ÿ���� ����

        // ��� �÷��̾ �׾��ٸ� ��� ���� ����
        if (allPlayersDead)
        {
            if (!isScoreAdded) // ������ �߰����� �ʾҴٸ�
            {
                foreach (var playerScore in GameManager.instance.playerScores)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("���� �߰�");
                        playerScore.AddScore(1000);  // ���� �߰�
                    }
                }

                isScoreAdded = true;
                StartCoroutine(FadeScene());
            }

            return;
        }

        // ī��Ʈ�ٿ� Ÿ�̸Ӱ� 0 ���Ϸ� �������� ���� ����
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
                        Debug.Log("���� �߰�");
                        playerScore.AddScore(1000);  // ���� �߰�
                    }
                }

                isScoreAdded = true;
                StartCoroutine(FadeScene());
            }
        }
    }
}
