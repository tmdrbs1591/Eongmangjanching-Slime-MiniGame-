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

    private enum State { Idle, Looking, Charging, Attacking };
    private State currentState = State.Idle;

    private GameObject warningLine;
    private Rigidbody rb;

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
        yield return new WaitForSeconds(3f);

        // ChooseRandomTarget을 호출할 때 매개변수를 전달하지 않도록 수정
        photonView.RPC("ChooseRandomTarget", RpcTarget.All);
    }

    [PunRPC]
    void ChooseRandomTarget()
    {
        // Idle 상태가 아니면 타겟을 변경하지 않음
        if (currentState != State.Idle || players.Count == 0) return;

        int playerIndex = Random.Range(0, players.Count);
        photonView.RPC("SetTargetIndex", RpcTarget.All, playerIndex);
    }

    [PunRPC]
    void SetTargetIndex(int index)
    {
        if (index >= 0 && index < players.Count)
        {
            targetPlayer = players[index];
            StartCoroutine(LookAtTarget());
        }
    }

    IEnumerator LookAtTarget()
    {
        currentState = State.Looking;

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ToggleWarningLine", RpcTarget.All, true); // warningLine 활성화 동기화
        }

        float elapsedTime = 0f;
        while (elapsedTime < lookDuration)
        {
            if (PhotonNetwork.IsMasterClient && targetPlayer != null)
            {
                Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // 마스터 클라이언트에서만 로테이션을 동기화
                photonView.RPC("SyncRotation", RpcTarget.Others, targetRotation);
                transform.rotation = targetRotation;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ToggleWarningLine", RpcTarget.All, false); // warningLine 비활성화 동기화
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
        if (PhotonNetwork.IsMasterClient && targetPlayer != null)
        {
            chargeDirection = (targetPlayer.transform.position - transform.position).normalized;
            currentState = State.Charging;

            photonView.RPC("StartChargingRPC", RpcTarget.All, chargeDirection);
        }
    }

    [PunRPC]
    void StartChargingRPC(Vector3 direction)
    {
        chargeDirection = direction;
        currentState = State.Attacking; // 공격 시작 상태로 변경
    }

    void FixedUpdate()
    {
        if (currentState == State.Charging || currentState == State.Attacking)
        {
            rb.MovePosition(transform.position + chargeDirection * chargeSpeed * Time.fixedDeltaTime);
        }
    }

    void StopCharging()
    {
        Debug.Log("Charge complete.");
        currentState = State.Idle;
        chargeSpeed += 1f;
        photonView.RPC("ChooseRandomTarget", RpcTarget.All);
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
                StopCharging();
            }

            Debug.Log("HIT " + collision.gameObject.name + "!!!");
        }
    }
}
