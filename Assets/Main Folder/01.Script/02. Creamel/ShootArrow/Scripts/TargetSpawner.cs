using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TargetSpawner : MonoBehaviourPunCallbacks
{
    public GameObject targetPrefab;
    public int maxTargets;
    public float XspawnRange;
    public float YspawnRange;
    public float checkCooltime;
    public int findTargetScore;

    private Vector3 spawnPosition;

    public List<GameObject> targets = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnTargetsRoutine());
    }

    IEnumerator SpawnTargetsRoutine()
    {
        while (true)
        {
            targets.Clear();
            Target[] targetScripts = FindObjectsOfType<Target>();

            foreach (Target targetSc in targetScripts)
            {
                if (targetSc.Point == findTargetScore)
                {
                    targets.Add(targetSc.gameObject);
                }
            }
            yield return new WaitForSeconds(checkCooltime);

            // ������ Ŭ���̾�Ʈ�� Ÿ���� ����
            if (PhotonNetwork.IsMasterClient && targets.Count < maxTargets)
            {
                // ���� ���� ��ġ ����
                spawnPosition = new Vector3(transform.position.x + Random.Range(-XspawnRange, XspawnRange),
                                             transform.position.y + Random.Range(-YspawnRange, YspawnRange),
                                             transform.position.z);
                // RPC�� ���� ��� Ŭ���̾�Ʈ�� Ÿ�� ���� ��û
                photonView.RPC("SpawnTargetRPC", RpcTarget.All, spawnPosition);
            }
        }
    }

    [PunRPC]
    void SpawnTargetRPC(Vector3 spawnPosition)
    {
        GameObject newTarget = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
        // �߰������� ������ Ÿ�ٿ� ���� �ʱ�ȭ�� �ʿ��ϸ� ���⼭ ������ �� ����
    }
}
