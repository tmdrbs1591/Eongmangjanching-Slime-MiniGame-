using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LogMiniGame : MonoBehaviourPunCallbacks
{
    // �� ��ġ�� Transform���� ����
    [SerializeField] Transform leftPos;
    [SerializeField] Transform rightPos;
    [SerializeField] Transform frontPos;
    [SerializeField] Transform backPos;

    [SerializeField] GameObject logPrefabs;
    [SerializeField] float moveSpeed = 5f; // �̵� �ӵ� ����

    // Start is called before the first frame update
    void Start()
    {
        // ������ Ŭ���̾�Ʈ�� �α׸� �����ϵ��� ����
        if (PhotonNetwork.IsMasterClient)
        {
            // 2�ʸ��� EventStart �Լ��� �ݺ� ����
            InvokeRepeating("EventStart", 2.0f, 2.0f);
        }
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
}
