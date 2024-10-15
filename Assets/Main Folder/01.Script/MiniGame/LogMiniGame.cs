using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement; // ���� �����Ϸ��� �ʿ�

public class LogMiniGame : TimeManager
{
    [SerializeField] Transform leftPos;
    [SerializeField] Transform rightPos;
    [SerializeField] Transform frontPos;
    [SerializeField] Transform backPos;

    [SerializeField] GameObject logPrefabs;
    [SerializeField] float moveSpeed = 5f; // �̵� �ӵ� ����

    private bool isScoreAdded = false;  // ������ �̹� �߰��Ǿ����� Ȯ���ϴ� ����

    protected override void Start()
    {
        base.Start();
        // ������ Ŭ���̾�Ʈ�� �α׸� �����ϵ��� ����
        if (PhotonNetwork.IsMasterClient)
        {
            // 2�ʸ��� EventStart �Լ��� �ݺ� ����
            InvokeRepeating("EventStart", 2.0f, 2.0f);
        }
    }

    void Update()
    {
        TimeEnd();
    }

    private Transform lastSpawnPoint = null; // ���������� ���õ� ��ġ�� ����

    void EventStart()
    {
        // ������ ���õ� ��ġ�� ���� ���ο� ��ġ�� ����
        Transform spawnPoint = null;

        if (lastSpawnPoint == leftPos || lastSpawnPoint == rightPos)
        {
            // lastSpawnPoint�� leftPos �Ǵ� rightPos�� ��� frontPos�� backPos �߿��� ����
            spawnPoint = Random.Range(0, 2) == 0 ? frontPos : backPos;
        }
        else
        {
            // lastSpawnPoint�� frontPos �Ǵ� backPos�� ��� leftPos�� rightPos �߿��� ����
            spawnPoint = Random.Range(0, 2) == 0 ? leftPos : rightPos;
        }

        // ��Ʈ��ũ���� �α� ������ ���� (PhotonView�� ���Ե� ������)
        PhotonNetwork.Instantiate(logPrefabs.name, spawnPoint.position, spawnPoint.rotation);

        // ������ ���õ� ��ġ ����
        lastSpawnPoint = spawnPoint;
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

                StartCoroutine(FadeScene());
            }
        }
    }
}
