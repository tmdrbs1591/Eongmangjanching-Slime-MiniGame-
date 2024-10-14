using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement; // ���� �����Ϸ��� �ʿ�

public class LogMiniGame : TimeManager
{
    // �� ��ġ�� Transform���� ����
    [SerializeField] Transform leftPos;
    [SerializeField] Transform rightPos;
    [SerializeField] Transform frontPos;
    [SerializeField] Transform backPos;

    [SerializeField] GameObject logPrefabs;
    [SerializeField] float moveSpeed = 5f; // �̵� �ӵ� ����

    // Start is called before the first frame update
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
    void EventStart()
    {
        // 4���� ��ġ �� �������� ����
        int randomIndex = Random.Range(0, 4);
        Transform spawnPoint = null;

        switch (randomIndex)
        {
            case 0:
                spawnPoint = leftPos;
                break;
            case 1:
                spawnPoint = rightPos;
                break;
            case 2:
                spawnPoint = frontPos;
                break;
            case 3:
                spawnPoint = backPos;
                break;
        }

        // ��Ʈ��ũ���� �α� ������ ���� (PhotonView�� ���Ե� ������)
        PhotonNetwork.Instantiate(logPrefabs.name, spawnPoint.position, spawnPoint.rotation);
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

            if (timeRemaining <= 0)
            {
                foreach (var playerScore in GameManager.instance.playerScores)
                {
                    // �÷��̾ ���� �ʾ����� ���� �߰�
                    if (!playerScore.isDeath && PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("���� �߰�");
                        playerScore.AddScore(1000);  // ���� �߰�
                        playerScore.isDeath = false;
                    }
                }
                StartCoroutine(FadeScene());
            }
        }
    }
}
