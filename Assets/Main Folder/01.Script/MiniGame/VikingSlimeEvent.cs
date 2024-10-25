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
        TimeEnd(); // �� �����Ӹ��� Ÿ�̸� üũ
    }

    private Transform lastSpawnPoint = null; // ���������� ���õ� ��ġ�� ����

    void EventStart()
    {
        // �̺�Ʈ ���� ���� �߰�
    }

    void TimeEnd()
    {
        // ��� �÷��̾ �׾����� �˻�
        bool allPlayersDead = true; // ��� �÷��̾ �׾����� ��Ÿ���� ����
        List<PlayerScore> alivePlayers = new List<PlayerScore>(); // ����ִ� �÷��̾� ���

        foreach (var playerScore in GameManager.instance.playerScores)
        {
            if (!playerScore.isDeath)
            {
                allPlayersDead = false; // �� ���̶� ���� �ʾ����� false
                alivePlayers.Add(playerScore); // ����ִ� �÷��̾� ��Ͽ� �߰�
            }
        }

        // ��� �÷��̾ �׾��ٸ� ��� ���� ����
        if (allPlayersDead)
        {
            HandleGameEnd();
            return;
        }

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
                AddScoresToAlivePlayers(alivePlayers);
                StartCoroutine(FadeScene());
            }
        }
    }

    private void HandleGameEnd()
    {
        // ���� ���� �� ���� �߰� ó��
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
                Debug.Log("���� �߰�");
                playerScore.AddScore(1000); // ���� �߰�
            }
        }

        // ������ �߰��Ǿ����� ���
        isScoreAdded = true;
    }
}
