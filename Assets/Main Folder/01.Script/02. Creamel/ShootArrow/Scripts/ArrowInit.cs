using UnityEngine;
using Photon.Pun;

public class ArrowInit : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed = 30f; // ȭ�� �ӵ� ����
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Rigidbody�� null���� Ȯ���ϰ� null�� ��� ��� �α� ���
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody�� �������� �ʽ��ϴ�. arrowPrefab�� Rigidbody�� �߰��ϼ���.");
            return;
        }

        rb.useGravity = false; // �߷� ��Ȱ��ȭ
    }

    public void initArrow(Vector3 startPosition, Vector3 targetPosition, int power, string player)
    {
        this.targetPosition = targetPosition;

        // Rigidbody�� null���� Ȯ���Ͽ� ���� ����
        if (rb != null)
        {
            Vector3 direction = (targetPosition - startPosition).normalized;
            rb.velocity = direction * speed;
        }
        else
        {
            Debug.LogWarning("Rigidbody�� �������� �ʾ����Ƿ� ȭ�� �ӵ��� �������� �ʽ��ϴ�.");
        }
    }

    void Update()
    {
        transform.LookAt(targetPosition); // ��ǥ ��ġ�� ȸ��
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �浹 �� ȭ���� ����
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Target"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
