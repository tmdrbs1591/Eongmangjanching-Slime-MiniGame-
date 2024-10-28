using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement; // ���� �����Ϸ��� �ʿ�

public class ArrowShootEvent : TimeManager
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
            // ��� ���� ���Ḧ ���� ó��
            if (!isScoreAdded) // ������ �߰����� �ʾҴٸ�
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

                // ���� ���� �� �� ��ȯ ó��
                StartCoroutine(FadeScene());
            }

            // Ÿ�̸Ӹ� 1�� �����ص� Ÿ�̸� ���Ҹ� ����, �� �̻��� ������Ʈ�� ���� ����
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
                foreach (var playerScore in GameManager.instance.playerScores)
                {
                    // �÷��̾ ���� �ʾ����� ���� �߰�
                    if (!playerScore.isDeath && PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("���� �߰�");
                    }
                }

                // ������ �߰��Ǿ����� ���
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
        Fadein.SetActive(true); // ��� Ŭ���̾�Ʈ���� FadeIn Ȱ��ȭ
    }

    protected IEnumerator FadeScene()
    {
        // FadeIn�� Ȱ��ȭ�ϴ� RPC ȣ��
        photonView.RPC("ActivateFadeIn", RpcTarget.All);
        yield return new WaitForSeconds(1.5f);

        // �� ��ȯ�� ���� RPC ȣ��
        photonView.RPC("LoadRandomScene", RpcTarget.All);
    }
}   