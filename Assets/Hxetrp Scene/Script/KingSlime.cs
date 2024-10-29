using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : MonoBehaviourPunCallbacks
{
    public float lookDuration = 1f;
    public float chargeSpeed = 5f;
    private PlayerScript targetPlayer;
    private List<PlayerScript> players = new List<PlayerScript>();
    private Vector3 chargeDirection;

    private enum State { Idle, Looking, Charging };
    private State currentState = State.Idle;

    private GameObject warningLine;
    private Rigidbody rb;

    private bool canCollide = false; // 충돌 가능 상태를 나타내는 변수

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        warningLine = transform.Find("Warning Line").gameObject;

        // players 리스트 초기화
        foreach (var playerScore in GameManager.instance.playerScores)
        {
            PlayerScript player = playerScore.GetComponent<PlayerScript>();
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

    void Update()
    {
        // 주기적으로 isDeath 상태를 확인하여 사망한 플레이어를 리스트에서 제거
        RemoveDeadPlayers();
    }

    void FixedUpdate()
    {
        if (currentState == State.Charging)
        {
            rb.MovePosition(transform.position + chargeDirection * chargeSpeed * Time.fixedDeltaTime);
        }
    }


    void RemoveDeadPlayers()
    {
        for (int i = players.Count - 1; i >= 0; i--)
        {
            PlayerScript player = players[i];
            PlayerScore playerScore = GameManager.instance.playerScores.Find(ps => ps.GetComponent<PlayerScript>() == player);

            if (playerScore != null && playerScore.isDeath) // 사망한 플레이어일 경우 리스트에서 제거
            {
                players.RemoveAt(i);
                Debug.Log("Removed player: " + player.name + " from target list due to death.");
            }
        }
    }

    IEnumerator InitialWaitAndChooseTarget()
    {
        yield return new WaitForSeconds(3f); // 3초 대기
        canCollide = true; // 충돌 가능 상태로 변경
        ChooseRandomTarget();
    }

    void ChooseRandomTarget()
    {
        if (players.Count == 0) return;

        int randomIndex = Random.Range(0, players.Count);
        targetPlayer = players[randomIndex];
        Debug.Log("Targeting: " + targetPlayer.name);

        StartCoroutine(LookAtTarget());
    }

    IEnumerator LookAtTarget()
    {
        currentState = State.Looking;
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ToggleWarningLine", RpcTarget.All, true);
        }

        float elapsedTime = 0f;
        Quaternion targetRotation = Quaternion.identity;
        while (elapsedTime < lookDuration)
        {
            if (targetPlayer != null)
            {
                Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
                targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

                if (PhotonNetwork.IsMasterClient && elapsedTime % 0.1f < Time.deltaTime) // 0.1초마다 동기화
                {
                    photonView.RPC("SyncRotation", RpcTarget.Others, transform.rotation);
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ToggleWarningLine", RpcTarget.All, false);
            StartCharging();
        }
    }


    [PunRPC]
    void SyncRotation(Quaternion rotation)
    {
        if (!PhotonNetwork.IsMasterClient) // 마스터 클라이언트는 로컬에서 로테이션을 설정하므로, 클라이언트만 RPC 값 사용
        {
            transform.rotation = rotation;
        }
    }

    [PunRPC]
    void ToggleWarningLine(bool isActive)
    {
        warningLine.SetActive(isActive);
    }

    void StartCharging()
    {
        if (targetPlayer != null)
        {
            chargeDirection = (targetPlayer.transform.position - transform.position).normalized;
            currentState = State.Charging;

            StartCoroutine(Charge());
        }
    }

    IEnumerator Charge()
    {
        while (currentState == State.Charging)
        {
            rb.MovePosition(transform.position + chargeDirection * chargeSpeed * Time.fixedDeltaTime);
            yield return null;
        }

        // 충돌 후 처리
        StopCharging();
    }

    void StopCharging()
    {
        Debug.Log("Charge complete.");
        currentState = State.Idle;
        ChooseRandomTarget();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canCollide) return; // 충돌 불가능 상태에서는 무시

        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Hit a wall, stopping charge.");
            StopCharging();
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            var playerScript = collision.gameObject.GetComponent<PlayerScript>();

            if (playerRb != null)
            {
                Vector3 forceDirection = collision.contacts[0].normal * -1;
                playerRb.AddForce(forceDirection * 35, ForceMode.Impulse);
                StartCoroutine(playerScript.StunCor(2));
            }

            PlayerScript hitPlayer = collision.gameObject.GetComponent<PlayerScript>();
            if (hitPlayer != null)
            {
                players.Remove(hitPlayer);
                Debug.Log("Removed player: " + hitPlayer.name + " from target list.");
            }

            Debug.Log("HIT " + collision.gameObject.name + "!!!");
        }
    }
}