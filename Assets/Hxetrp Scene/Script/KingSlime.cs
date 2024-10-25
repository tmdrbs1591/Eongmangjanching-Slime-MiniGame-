using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : MonoBehaviourPunCallbacks
{
    public float lookDuration = 1f; // 플레이어를 바라보는 시간
    public float chargeSpeed = 5f; // 돌진 속도
    public float initialChargeSpeed; // 돌진 속도의 초기 값

    private PlayerScript targetPlayer; // 타겟 플레이어
    private List<PlayerScript> players;
    private Vector3 chargeDirection; // 돌진 방향
    private enum State { Idle, Looking, Charging }; // 상태 정의
    private State currentState = State.Idle; // 초기 상태는 Idle

    private GameObject warningLine;

    void Start()
    {
        // 시작 시 초기 속도를 저장
        initialChargeSpeed = chargeSpeed;

        warningLine = transform.Find("Warning Line").gameObject;
        // 플레이어 목록 초기화
        players = new List<PlayerScript>();

        // "Test Player" 태그를 가진 모든 오브젝트를 찾음
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        // 각 플레이어 오브젝트에서 TestPlayer 스크립트를 가져와 리스트에 추가
        foreach (GameObject playerObject in playerObjects)
        {
            PlayerScript player = playerObject.GetComponent<PlayerScript>();
            if (player != null)
            {
                players.Add(player);
            }
        }

        // 게임 시작 3초 후 첫 타겟 선택
        StartCoroutine(InitialWaitAndChooseTarget());
    }

    IEnumerator InitialWaitAndChooseTarget()
    {
        // 게임 시작 후 3초 대기
        yield return new WaitForSeconds(3f);
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(ChooseRandomTarget());
        }
    }

    IEnumerator ChooseRandomTarget()
    {
        // 랜덤한 타겟 선택
        if (players.Count == 0) yield break; // 플레이어가 없으면 종료

        int randomIndex = Random.Range(0, players.Count);
        targetPlayer = players[randomIndex];
        Debug.Log("Targeting: " + targetPlayer.name);

        // 타겟을 바라보는 상태로 전환
        currentState = State.Looking;
        warningLine.SetActive(true);

        // 타겟을 바라보기
        float elapsedTime = 0f;
        while (elapsedTime < lookDuration)
        {
            Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction); // 타겟 방향으로 회전
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 타겟을 향해 돌진 시작
        warningLine.SetActive(false);
        photonView.RPC("StartChargingRPC", RpcTarget.All);
    }

    [PunRPC]
    void StartChargingRPC()
    {
        // 돌진 방향 설정 및 돌진 상태로 전환
        chargeDirection = (targetPlayer.transform.position - transform.position).normalized;
        currentState = State.Charging;
    }

    void Update()
    {
        // 현재 상태에 따라 행동 결정
        if (currentState == State.Charging)
        {
            // 한 번 설정한 돌진 방향을 고정하여 움직임
            transform.position += chargeDirection * chargeSpeed * Time.deltaTime;
        }
    }

    void StopCharging()
    {
        Debug.Log("Charge complete.");

        // 돌진 종료 후 새로운 타겟을 찾기 시작
        currentState = State.Idle;
        chargeSpeed = initialChargeSpeed; // 속도 초기화
        StartCoroutine(ChooseRandomTarget()); // 새로운 타겟 선택 시작
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 벽에 충돌했을 경우
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Hit a wall, stopping charge.");

            // 벽에 충돌하면 돌진 멈춤
            StopCharging();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();
            var playerScript = other.gameObject.GetComponent<PlayerScript>();

            if (playerRb != null)
            {
                // 간단한 충돌 방향 계산
                Vector3 forceDirection = (other.transform.position - transform.position).normalized;
                playerRb.AddForce(forceDirection * 1000, ForceMode.Impulse);  // 방향에 힘 가하기
                StartCoroutine(playerScript.StunCor());
            }

            // 충돌한 플레이어를 타겟 리스트에서 제거
            PlayerScript hitPlayer = other.gameObject.GetComponent<PlayerScript>();
            if (hitPlayer != null)
            {
                players.Remove(hitPlayer);  // 플레이어 목록에서 제거
                Debug.Log("Removed player: " + hitPlayer.name + " from target list.");
            }

            Debug.Log("HIT " + other.gameObject.name + "!!!");
        }
    }
}
