using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : MonoBehaviourPunCallbacks
{
    public float lookDuration = 1f; // 플레이어를 바라보는 시간
    public float chargeSpeed = 5f; // 돌진 속도
    private PlayerScript targetPlayer; // 타겟 플레이어
    private List<PlayerScript> players = new List<PlayerScript>();
    private Vector3 chargeDirection; // 돌진 방향

    private enum State { Idle, Looking, Charging }; // 상태 정의
    private State currentState = State.Idle; // 초기 상태는 Idle

    private GameObject warningLine;
    private Rigidbody rb; // Rigidbody 추가

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        warningLine = transform.Find("Warning Line").gameObject;

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObject in playerObjects)
        {
            PlayerScript player = playerObject.GetComponent<PlayerScript>();
            if (player != null)
            {
                players.Add(player);
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(InitialWaitAndChooseTarget());
        }
    }

    IEnumerator InitialWaitAndChooseTarget()
    {
        yield return new WaitForSeconds(3f);
        photonView.RPC("ChooseRandomTarget", RpcTarget.All); // RPC 호출
    }

    [PunRPC] // RPC로 설정
    void ChooseRandomTarget()
    {
        if (players.Count == 0) return;

        int randomIndex = Random.Range(0, players.Count);
        targetPlayer = players[randomIndex];
        Debug.Log("Targeting: " + targetPlayer.name);

        // 랜덤 인덱스를 모든 클라이언트에 전송
        photonView.RPC("SetTargetIndex", RpcTarget.All, randomIndex);
    }

    [PunRPC]
    void SetTargetIndex(int index)
    {
        if (index >= 0 && index < players.Count)
        {
            targetPlayer = players[index]; // 동기화된 타겟 플레이어 설정
            StartCoroutine(LookAtTarget());
        }
    }

    IEnumerator LookAtTarget()
    {
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
        photonView.RPC("StartChargingRPC", RpcTarget.All, chargeDirection);
    }

    [PunRPC] // RPC로 설정
    void StartChargingRPC(Vector3 direction) // 방향을 인자로 받도록 수정
    {
        chargeDirection = direction; // 모든 클라이언트에서 충전 방향 설정
        currentState = State.Charging; // 모든 클라이언트에서 충전 상태로 변경
    }

    void FixedUpdate() // FixedUpdate로 변경
    {
        if (currentState == State.Charging)
        {
            rb.MovePosition(transform.position + chargeDirection * chargeSpeed * Time.fixedDeltaTime);
        }
    }

    void StopCharging() // 충전 정지 메서드 추가
    {
        Debug.Log("Charge complete.");
        currentState = State.Idle;
        chargeSpeed += 1f; // 충전 속도 증가
        photonView.RPC("ChooseRandomTarget", RpcTarget.All); // 새로운 타겟 선택
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Hit a wall, stopping charge.");
            StopCharging();
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            // 충돌한 객체의 Rigidbody 가져오기
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            var playerScript = collision.gameObject.GetComponent<PlayerScript>();

            if (playerRb != null)
            {
                // Log 객체가 충돌한 방향으로 힘을 가함
                Vector3 forceDirection = collision.contacts[0].normal * -1;  // 충돌면의 반대 방향
                playerRb.AddForce(forceDirection * 15, ForceMode.Impulse);  // 힘을 즉시 가함
                StartCoroutine(playerScript.StunCor());
            }

            PlayerScript hitPlayer = collision.gameObject.GetComponent<PlayerScript>();
            if (hitPlayer != null)
            {
                players.Remove(hitPlayer);
                Debug.Log("Removed player: " + hitPlayer.name + " from target list.");
                StopCharging(); // 플레이어가 제거되면 충전 정지
            }

            Debug.Log("HIT " + collision.gameObject.name + "!!!");
        }
    }
}
