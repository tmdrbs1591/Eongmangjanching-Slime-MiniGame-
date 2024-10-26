using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;
    public int maxTargets;
    public float XspawnRange;
    public float YspawnRange;
    public float checkCooltime;
    public float Distence;

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
            Target[] targetScripts;
            targetScripts = FindObjectsOfType<Target>();

            foreach (Target targetSc in targetScripts)
            {
                if (targetSc.Point == findTargetScore)
                {
                    targets.Add(targetSc.gameObject);
                }
            }
            yield return new WaitForSeconds(checkCooltime);
            if (targets.Count < maxTargets)
            {
                SpawnTarget();
            }
        }
    }

    void SpawnTarget()
    {
        spawnPosition = new Vector3(transform.position.x + Random.Range(-XspawnRange, XspawnRange), transform.position.y + Random.Range(-YspawnRange, YspawnRange), transform.position.z);
        GameObject newTarget = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);

    }
}
