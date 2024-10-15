using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerScript : MonoBehaviourPunCallbacks
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;  // �̵� �ӵ� ����
    [SerializeField] private float sprintSpeed = 10f;  // �޸��� �ӵ�
    [SerializeField] private float rotationSpeed = 10f;  // ȸ�� �ӵ� ����
    [SerializeField] private float jumpForce = 5f; // ���� ��

    [Header("Interaction")]
    [SerializeField] Vector3 interactionkBoxSize; //��ȣ�ۿ� ����
    [SerializeField] Transform interactionBoxPos; // ��ȣ�ۿ� ��ġ
    private PlayerCustomizing playerCustomizing;

    [Header("UI")]
    [SerializeField] GameObject CustomPanel;

    [Header("Bool")]
    [SerializeField] public bool isStun;

    [Header("Effects")]
    [SerializeField] ParticleSystem runPtc; // ��ƼŬ �ý���

    private Rigidbody rb;
    private Animator anim; // Animator ������Ʈ

    private Vector3 moveDirection; // �̵� ���� ����

    [SerializeField] TMP_Text nickNameText;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Rigidbody ������Ʈ ��������
        anim = GetComponent<Animator>(); // Animator ������Ʈ ��������
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
        if (isStun)
            return;

        // �Է��� ������ �̵��� FixedUpdate���� ó��
        float moveX = Input.GetAxisRaw("Horizontal"); // A, D
        float moveZ = Input.GetAxisRaw("Vertical");   // W, S

        // �̵� ���� ���
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // �޸��� ���� üũ
        bool isSprinting = Input.GetKey(KeyCode.LeftShift); // Shift Ű�� �޸��� ���� Ȯ��

        // �ִϸ��̼� ���� ������Ʈ (�̵� ���� �� �ִϸ��̼� ����)
        anim.SetBool("isMove", moveDirection != Vector3.zero);

        // ������ �̵� ó��
        if (moveDirection != Vector3.zero)
        {
            // �̵� �ӵ� ���� (�޸��� ������ ��� �ӵ� ����)
            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

            // �޸��� �� ��ƼŬ ȿ�� ����/����
            if (isSprinting && !runPtc.isPlaying)
                photonView.RPC("PlayRunParticle", RpcTarget.AllBuffered);
            else if (!isSprinting && runPtc.isPlaying)
                photonView.RPC("StopRunParticle", RpcTarget.AllBuffered);

            Vector3 move = moveDirection * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

            // ȸ�� ó��
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void HandleInput()
    {
        // �Է��� �޾� �̵� ���� ���
        float moveX = Input.GetAxisRaw("Horizontal"); // A, D
        float moveZ = Input.GetAxisRaw("Vertical");   // W, S

        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // �ִϸ��̼� ���� ������Ʈ (�̵� ���� �� �ִϸ��̼� ����)
        anim.SetBool("isMove", moveDirection != Vector3.zero);
    }

    public void Jump()
    {
        // Rigidbody�� �� �������� ���� �߰��Ͽ� ����
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

    public IEnumerator StunCor()
    {
        isStun = true;
        yield return new WaitForSeconds(1f);
        isStun = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(interactionBoxPos.position, interactionkBoxSize);
    }

    // �޸��� ��ƼŬ ȿ�� �����ϴ� RPC
    [PunRPC]
    void PlayRunParticle()
    {
        if (!runPtc.isPlaying)
            runPtc.Play();
    }

    // �޸��� ��ƼŬ ȿ�� ���ߴ� RPC
    [PunRPC]
    void StopRunParticle()
    {
        if (runPtc.isPlaying)
            runPtc.Stop();
    }
}
