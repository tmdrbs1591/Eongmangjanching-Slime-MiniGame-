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



    protected override void Start()
    {
        base.Start();
        StartCoroutine(Co_SpawnLog());
        StartCoroutine(Co_SpawnBall());
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
}