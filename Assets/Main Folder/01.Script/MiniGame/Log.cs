using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Log : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float forcePower = 10f;  // 플레이어에게 가할 힘의 크기
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate는 물리 계산과 함께 실행됩니다.
    void FixedUpdate()
    {
        // 객체의 로컬 좌표를 기준으로 왼쪽으로 이동
        Vector3 moveDirection = transform.TransformDirection(Vector3.left);
        Vector3 newPosition = transform.position + moveDirection * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    // 플레이어와 충돌 시 해당 방향으로 플레이어에게 힘을 가함
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 객체가 "Player" 태그를 가지고 있는지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            // 충돌한 객체의 Rigidbody 가져오기
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            var playerScript = collision.gameObject.GetComponent<PlayerScript>();

            if (playerRb != null)
            {
                // Log 객체가 충돌한 방향으로 힘을 가함
                Vector3 forceDirection = collision.contacts[0].normal * -1;  // 충돌면의 반대 방향
                playerRb.AddForce(forceDirection * forcePower, ForceMode.Impulse);  // 힘을 즉시 가함
                StartCoroutine(playerScript.StunCor(1));
            } 
        }
    }
}
