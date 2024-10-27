using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Log : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float forcePower = 10f;  // �÷��̾�� ���� ���� ũ��
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate�� ���� ���� �Բ� ����˴ϴ�.
    void FixedUpdate()
    {
        // ��ü�� ���� ��ǥ�� �������� �������� �̵�
        Vector3 moveDirection = transform.TransformDirection(Vector3.left);
        Vector3 newPosition = transform.position + moveDirection * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    // �÷��̾�� �浹 �� �ش� �������� �÷��̾�� ���� ����
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ��ü�� "Player" �±׸� ������ �ִ��� Ȯ��
        if (collision.gameObject.CompareTag("Player"))
        {
            // �浹�� ��ü�� Rigidbody ��������
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            var playerScript = collision.gameObject.GetComponent<PlayerScript>();

            if (playerRb != null)
            {
                // Log ��ü�� �浹�� �������� ���� ����
                Vector3 forceDirection = collision.contacts[0].normal * -1;  // �浹���� �ݴ� ����
                playerRb.AddForce(forceDirection * forcePower, ForceMode.Impulse);  // ���� ��� ����
                StartCoroutine(playerScript.StunCor(1));
            } 
        }
    }
}
