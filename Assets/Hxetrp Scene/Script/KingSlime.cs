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

    private enum State { Idle, Targeting, Charging, Attacking };
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
        RemoveDeadPlayers();
    }

    void RemoveDeadPlayers()
    {
        for (int i = players.Count - 1; i >= 0; i--)
        {
            PlayerScript player = players[i];
            PlayerScore playerScore = GameManager.instance.playerScores.Find(ps => ps.GetComponent<PlayerScript>() == player);

            if (playerScore != null && playerScore.isDeath)
            {
                players.RemoveAt(i);
                Debug.Log("Removed player: " + player.name + " from target list due to death.");
            }
        }
    }

    IEnumerator InitialWaitAndChooseTarget()
    {
        yield return new WaitForSeconds(3f);
        photonView.RPC("TargetAndPrepare", RpcTarget.All);
    }

    [PunRPC]
    void TargetAndPrepare()
    {
        if (currentState != State.Idle || players.Count == 0) return;

        int playerIndex = Random.Range(0, players.Count);
        targetPlayer = players[playerIndex];
        chargeDirection = (targetPlayer.transform.position - transform.position).normalized;

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(LookAndChargeCoroutine());
        }

        currentState = State.Targeting;
    }

    IEnumerator LookAndChargeCoroutine()
    {
        photonView.RPC("ToggleWarningLine", RpcTarget.All, true); // 경고 라인 활성화
        float elapsedTime = 0f;

        while (elapsedTime < lookDuration)
        {
            Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            photonView.RPC("SyncRotationAndDirection", RpcTarget.Others, targetRotation, direction);
            transform.rotation = targetRotation;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        photonView.RPC("ToggleWarningLine", RpcTarget.All, false); // 경고 라인 비활성화
        photonView.RPC("StartChargingRPC", RpcTarget.All, chargeDirection);
    }

    [PunRPC]
    void SyncRotationAndDirection(Quaternion rotation, Vector3 direction)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            transform.rotation = rotation;
            chargeDirection = direction;
        }
    }

    [PunRPC]
    void ToggleWarningLine(bool isActive)
    {
        warningLine.SetActive(isActive);
    }

    [PunRPC]
    void StartChargingRPC(Vector3 direction)
    {
        chargeDirection = direction;
        currentState = State.Charging;
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
        currentState = State.Idle;
        chargeSpeed += 1f;
        photonView.RPC("TargetAndPrepare", RpcTarget.All);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            StopCharging();
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            var playerScript = collision.gameObject.GetComponent<PlayerScript>();

            if (playerRb != null)
            {
                Vector3 forceDirection = (collision.contacts[0].point - transform.position).normalized;
                playerRb.AddForce(forceDirection * 35, ForceMode.Impulse);

                StartCoroutine(playerScript.StunCor(2));
            }

            PlayerScript hitPlayer = collision.gameObject.GetComponent<PlayerScript>();
            if (hitPlayer != null)
            {
                players.Remove(hitPlayer);
                StopCharging();
            }
        }
    }
}
