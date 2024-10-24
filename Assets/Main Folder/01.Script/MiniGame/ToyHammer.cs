using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ToyHammer : MonoBehaviourPunCallbacks
{
    [Header("Interaction")]
    [SerializeField] Vector3 interactionkBoxSize; // ��ȣ�ۿ� ����
    [SerializeField] Transform interactionBoxPos; // ��ȣ�ۿ� ��ġ

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
            // ���ÿ��� ���� ó��
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
        // ��� Ŭ���̾�Ʈ���� ��ġ�� �ൿ�� ����
        Collider[] colliders = Physics.OverlapBox(interactionBoxPos.position, interactionkBoxSize / 2f);
        foreach (Collider collider in colliders)
        {
            if (collider != null && collider.CompareTag("Player"))
            {
                // �÷��̾��� ������ٵ� ��������
                Rigidbody playerRigidbody = collider.GetComponent<Rigidbody>();

                if (playerRigidbody != null)
                {
                    // ��ĥ ���� ������ ���� (��: ���� ����)
                    Vector3 pushDirection = (collider.transform.position - transform.position).normalized; // �÷��̾�� ������ ����
                    float pushStrength = 18f; // ���� ����

                    Instantiate(starPtc, collider.transform.position , Quaternion.identity);
                    // AddForce�� �����ֱ�
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
