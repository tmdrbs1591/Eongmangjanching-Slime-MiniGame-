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

            // 마스터 클라이언트만 타겟을 생성
            if (PhotonNetwork.IsMasterClient && targets.Count < maxTargets)
            {
                // 랜덤 스폰 위치 생성
                spawnPosition = new Vector3(transform.position.x + Random.Range(-XspawnRange, XspawnRange),
                                             transform.position.y + Random.Range(-YspawnRange, YspawnRange),
                                             transform.position.z);
                // RPC를 통해 모든 클라이언트에 타겟 생성 요청
                photonView.RPC("SpawnTargetRPC", RpcTarget.All, spawnPosition);
            }
        }
    }

    [PunRPC]
    void SpawnTargetRPC(Vector3 spawnPosition)
    {
        GameObject newTarget = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
        // 추가적으로 생성된 타겟에 대한 초기화가 필요하면 여기서 수행할 수 있음
    }
}
