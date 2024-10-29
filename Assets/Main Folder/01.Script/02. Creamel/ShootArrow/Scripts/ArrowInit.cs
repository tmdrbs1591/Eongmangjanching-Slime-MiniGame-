using UnityEngine;
using Photon.Pun;

public class ArrowInit : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed = 30f; // 화살 속도 설정
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Rigidbody가 null인지 확인하고 null일 경우 경고 로그 출력
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody가 존재하지 않습니다. arrowPrefab에 Rigidbody를 추가하세요.");
            return;
        }

        rb.useGravity = false; // 중력 비활성화
    }

    public void initArrow(Vector3 startPosition, Vector3 targetPosition, int power, string player)
    {
        this.targetPosition = targetPosition;

        // Rigidbody가 null인지 확인하여 오류 방지
        if (rb != null)
        {
            Vector3 direction = (targetPosition - startPosition).normalized;
            rb.velocity = direction * speed;
        }
        else
        {
            Debug.LogWarning("Rigidbody가 설정되지 않았으므로 화살 속도가 설정되지 않습니다.");
        }
    }

    void Update()
    {
        transform.LookAt(targetPosition); // 목표 위치로 회전
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌 시 화살을 제거
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Target"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
