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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        warningLine = transform.Find("Warning Line").gameObject;

        // GameManager의 playerScores 리스트를 통해 players 리스트 구성
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

    IEnumerator InitialWaitAndChooseTarget()
    {
        yield return new WaitForSeconds(3f);
        photonView.RPC("ChooseRandomTarget", RpcTarget.All);
    }

    [PunRPC]
    void ChooseRandomTarget()
    {
        if (players.Count == 0) return;

        int randomIndex = Random.Range(0, players.Count);
        targetPlayer = players[randomIndex];
        Debug.Log("Targeting: " + targetPlayer.name);

        photonView.RPC("SetTargetIndex", RpcTarget.All, randomIndex);
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
        currentState = State.Charging;
    }

    void FixedUpdate()
    {
        if (currentState == State.Charging)
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
                playerRb.AddForce(forceDirection * 15, ForceMode.Impulse);
                StartCoroutine(playerScript.StunCor());
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
