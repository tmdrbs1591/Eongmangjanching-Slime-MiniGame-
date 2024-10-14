using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerScript : MonoBehaviourPunCallbacks   
{
    [SerializeField] private float moveSpeed = 5f;  // 이동 속도 설정
    [SerializeField] private float rotationSpeed = 10f;  // 회전 속도 설정
    [SerializeField] private float jumpForce = 5f; // 점프 힘



    [SerializeField] Vector3 interactionkBoxSize; //상호작용 범위
    [SerializeField] Transform interactionBoxPos; // 상호작용 위치
    private PlayerCustomizing playerCustomizing;

    [SerializeField] GameObject CustomPanel;

  


    private Rigidbody rb;
    private Animator anim; // Animator 컴포넌트

    private Vector3 moveDirection; // 이동 방향 저장



    [SerializeField] TMP_Text nickNameText;

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Rigidbody 컴포넌트 가져오기
        anim = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
        playerCustomizing = GetComponent<PlayerCustomizing>();


        if (photonView.IsMine)
        {
            nickNameText.text = PhotonNetwork.NickName;
            nickNameText.color = Color.green;

        }
        else
        {
            nickNameText.text = photonView.Owner.NickName;
            nickNameText.color = Color.white;
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleInput();
            Interaction();

        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Move();

           
        }
    }


    void Move()
    {
        // 입력을 받지만 이동은 FixedUpdate에서 처리
        float moveX = Input.GetAxisRaw("Horizontal"); // A, D
        float moveZ = Input.GetAxisRaw("Vertical");   // W, S

        // 이동 방향 계산
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // 애니메이션 상태 업데이트 (이동 중일 때 애니메이션 실행)
        anim.SetBool("isMove", moveDirection != Vector3.zero);

        // 물리적 이동 처리
        if (moveDirection != Vector3.zero)
        {
            // 이동
            Vector3 move = moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

            // 회전 처리
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
    void HandleInput()
    {
        // 입력을 받아 이동 방향 계산
        float moveX = Input.GetAxisRaw("Horizontal"); // A, D
        float moveZ = Input.GetAxisRaw("Vertical");   // W, S

        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // 애니메이션 상태 업데이트 (이동 중일 때 애니메이션 실행)
        anim.SetBool("isMove", moveDirection != Vector3.zero);
    }
    public void Jump()
    {
        // Rigidbody에 위 방향으로 힘을 추가하여 점프
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {


            Collider[] colliders = Physics.OverlapBox(interactionBoxPos.position, interactionkBoxSize / 2f);
            foreach (Collider collider in colliders)
            {
                if (collider != null && collider.CompareTag("Facility"))
                {
                    var facilityScript = collider.GetComponent<Facility>();

                    if (facilityScript.currentType == FacilityType.PlayerCustomizingBox)
                    {
                        UIManager.instance.PushUI(CustomPanel);

                        GameManager.instance.dumiSkinMeshRenderer.material = playerCustomizing.currentMaterial;
                    }
                }
            }


        }
     
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(interactionBoxPos.position, interactionkBoxSize);
    }
}
