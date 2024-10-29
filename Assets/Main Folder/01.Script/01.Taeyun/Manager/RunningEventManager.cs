// # System
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

// # Unity
using UnityEngine;

public class RunningEventManager : TimeManager
{
    [Header("Props_Log")]
    [SerializeField] private GameObject log;
    [SerializeField] private Transform logSpawnPos;
    [SerializeField] private float logSpawnTime;

    [Header("Props_ball")]
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform[] ballSpawnPos;
    [SerializeField] private float ballSpawnTime;

    private bool isScoreAdded = false;  // ������ �̹� �߰��Ǿ����� Ȯ���ϴ� ����

    protected override void Start()
    {
        base.Start();
        StartCoroutine(Co_SpawnLog());
        StartCoroutine(Co_SpawnBall());
    }
    void Update()
    {
        TimeEnd();
    }
    void EventStart()
    {
    }


    IEnumerator Co_SpawnLog()
    {
        //PhotonNetwork.Instantiate(log.name, logSpawnPos.position, logSpawnPos.rotation);
        Instantiate(log, logSpawnPos.position, logSpawnPos.rotation);

        yield return new WaitForSeconds(logSpawnTime);

        yield return StartCoroutine(Co_SpawnLog()); 
    }

    IEnumerator Co_SpawnBall()
    {
        int index = Random.Range(0, ballSpawnPos.Length);
        
        Instantiate(ball, ballSpawnPos[index].position, ballSpawnPos[index].rotation);

        yield return new WaitForSeconds(ballSpawnTime);

        yield return StartCoroutine(Co_SpawnBall());
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
                        playerScore.AddScore(1000);  // ���� �߰�
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