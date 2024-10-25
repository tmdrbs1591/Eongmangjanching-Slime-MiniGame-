using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement; // ���� �����Ϸ��� �ʿ�

public class VikingSlimeEvent : TimeManager
{
    private bool isScoreAdded = false;  // ������ �̹� �߰��Ǿ����� Ȯ���ϴ� ����

    private Transform target; // ������ Ÿ��

    protected override void Start()
    {
        base.Start();
        // ������ Ŭ���̾�Ʈ�� �α׸� �����ϵ��� ����
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(InitialTargetSetup());
        }
    }

    void Update()
    {
        TimeEnd();
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

        foreach (var playerScore in GameManager.instance.playerScores)
        {
            // �� ���̶� ���� ���� �÷��̾ ������ false
            if (!playerScore.isDeath)
            {
                allPlayersDead = false;
                break; // �� �̻� �˻��� �ʿ� ����
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
                AddScoresToAlivePlayers();
                StartCoroutine(FadeScene());
            }
        }
    }

    private void HandleGameEnd()
    {
        // ���� ���� �� ���� �߰� ó��
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
                Debug.Log("���� �߰�");
                playerScore.AddScore(1000); // ���� �߰�
            }
        }

        // ������ �߰��Ǿ����� ���
        isScoreAdded = true;
    }

    private IEnumerator InitialTargetSetup()
    {
        yield return new WaitForSeconds(1f); // �ʱ�ȭ ���� �ð�
        SetTarget(); // Ÿ�� ����
    }

    private void SetTarget()
    {
        // Ÿ�� ���� ���� (���⼭ Ÿ���� �����մϴ�)
        // ��: target = ���õ� �÷��̾��� Transform;

        // Ÿ���� ������ �� �ٸ� Ŭ���̾�Ʈ�� ����ȭ
        photonView.RPC("SyncTarget", RpcTarget.All, target.position, target.rotation);
    }

    [PunRPC]
    private void SyncTarget(Vector3 targetPosition, Quaternion targetRotation)
    {
        target.position = targetPosition;
        target.rotation = targetRotation;
    }
}
