using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ToyHammer : MonoBehaviourPunCallbacks
{
    [Header("Interaction")]
    [SerializeField] Vector3 interactionkBoxSize; // 상호작용 범위
    [SerializeField] Transform interactionBoxPos; // 상호작용 위치

    private Animator anim;

    [SerializeField] GameObject starPtc;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    void Attack()
    {
        if (photonView.IsMine)
        {
            // 로컬에서 공격 처리
            if (Input.GetMouseButtonDown(0))
            {
                anim.SetTrigger("isAttack");
                photonView.RPC("PunAttack", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void PunAttack()
    {
        // 모든 클라이언트에서 밀치는 행동을 수행
        Collider[] colliders = Physics.OverlapBox(interactionBoxPos.position, interactionkBoxSize / 2f);
        foreach (Collider collider in colliders)
        {
            if (collider != null && collider.CompareTag("Player"))
            {
                // 플레이어의 리지드바디를 가져오기
                Rigidbody playerRigidbody = collider.GetComponent<Rigidbody>();

                if (playerRigidbody != null)
                {
                    // 밀칠 힘의 방향을 설정 (예: 공격 방향)
                    Vector3 pushDirection = (collider.transform.position - transform.position).normalized; // 플레이어에서 공격자 방향
                    float pushStrength = 18f; // 힘의 세기

                    Instantiate(starPtc, collider.transform.position , Quaternion.identity);
                    // AddForce로 밀쳐주기
                    playerRigidbody.AddForce(pushDirection * pushStrength, ForceMode.Impulse);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(interactionBoxPos.position, interactionkBoxSize);
    }
}
