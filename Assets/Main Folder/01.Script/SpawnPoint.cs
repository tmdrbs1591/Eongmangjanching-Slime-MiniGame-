using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnPoint : MonoBehaviourPunCallbacks
{
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();  // SpawnPoint ����Ʈ

    private int currentSpawnIndex = 0; // ���� �̵��� �ε��� ����

    // Start is called before the first frame update
    void Start()
    {
        // PlayerScore ����Ʈ���� �� �÷��̾��� ��ġ�� spawnPoints ����Ʈ�� �߰�
        AddPlayerPositionsToSpawnPoints();
    }

    // PlayerScore ����Ʈ���� �÷��̾��� ��ġ�� SpawnPoint ����Ʈ�� �߰��ϴ� �Լ�
    void AddPlayerPositionsToSpawnPoints()
    {
        foreach (var playerScore in GameManager.instance.playerScores)
        {
            // PlayerScore���� GameObject�� �����ͼ� ��ġ�� SpawnPoint ����Ʈ�� �߰�
            spawnPoints.Add(playerScore.gameObject.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // spawnPoints�� ������� �ʰ�, ���� �ε����� playerScores���� ���� ���
        if (spawnPoints.Count > 0 && currentSpawnIndex < GameManager.instance.playerScores.Count)
        {
            // ���� �÷��̾�� �ش� ��ġ�� �̵�
            MovePlayerToSpawnPoint(GameManager.instance.playerScores[currentSpawnIndex], spawnPoints[currentSpawnIndex]);

            // �ε��� ���� (���� �÷��̾ ���� ��ġ�� �̵�)
            currentSpawnIndex++;
        }
    }

    // �÷��̾ ������ ��ġ�� �̵���Ű�� �Լ�
    void MovePlayerToSpawnPoint(PlayerScore playerScore, Transform spawnPoint)
    {
        playerScore.gameObject.transform.position = spawnPoint.position;
    }
}
