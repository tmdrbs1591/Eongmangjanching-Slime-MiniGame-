using Photon.Realtime;
using Photon.Pun; // Photon.Pun 네임스페이스 추가
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
        initialChargeSpeed = chargeSpeed;

        warningLine = transform.Find("Warning Line").gameObject;
        players = new List<PlayerScript>();

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playerObject in playerObjects)
        {
            PlayerScript player = playerObject.GetComponent<PlayerScript>();
            if (player != null)
            {
                players.Add(player);
            }
        }

        StartCoroutine(InitialWaitAndChooseTarget());
    }

    IEnumerator InitialWaitAndChooseTarget()
    {
        yield return new WaitForSeconds(3f);
        StartCoroutine(ChooseRandomTarget());
    }

    [PunRPC] // RPC로 설정
    IEnumerator ChooseRandomTarget()
    {
        if (players.Count == 0) yield break;

        int randomIndex = Random.Range(0, players.Count);
        targetPlayer = players[randomIndex];
        Debug.Log("Targeting: " + targetPlayer.name);

        currentState = State.Looking;
        warningLine.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < lookDuration)
        {
            Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        warningLine.SetActive(false);
        StartCharging();
    }

    void StartCharging()
    {
        chargeDirection = (targetPlayer.transform.position - transform.position).normalized;
        currentState = State.Charging;

        // 모든 클라이언트에서 StartChargingRPC 호출
        photonView.RPC("StartChargingRPC", RpcTarget.All);
    }

    [PunRPC] // RPC로 설정
    void StartChargingRPC()
    {
        // 여기서 필요한 로직을 추가 (예: 충전 시작 상태로 변경)
    }

    void Update()
    {
        if (currentState == State.Charging)
        {
            transform.position += chargeDirection * chargeSpeed * Time.deltaTime;
        }
    }

    void StopCharging()
    {
        Debug.Log("Charge complete.");
        currentState = State.Idle;
        chargeSpeed += 1f;
        StartCoroutine(ChooseRandomTarget());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Hit a wall, stopping charge.");
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
                Vector3 forceDirection = (other.transform.position - transform.position).normalized;
                playerRb.AddForce(forceDirection * 1000, ForceMode.Impulse);
                StartCoroutine(playerScript.StunCor());
            }

            PlayerScript hitPlayer = other.gameObject.GetComponent<PlayerScript>();
            if (hitPlayer != null)
            {
                players.Remove(hitPlayer);
                Debug.Log("Removed player: " + hitPlayer.name + " from target list.");
            }

            Debug.Log("HIT " + other.gameObject.name + "!!!");
        }
    }
}
