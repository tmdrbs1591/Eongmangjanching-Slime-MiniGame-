using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerScript : MonoBehaviourPunCallbacks   
{
    [SerializeField] private float moveSpeed = 5f;  // �̵� �ӵ� ����
    [SerializeField] private float rotationSpeed = 10f;  // ȸ�� �ӵ� ����
    [SerializeField] private float jumpForce = 5f; // ���� ��



    [SerializeField] Vector3 interactionkBoxSize; //��ȣ�ۿ� ����
    [SerializeField] Transform interactionBoxPos; // ��ȣ�ۿ� ��ġ

    [SerializeField] GameObject CustomPanel;

    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] public Material currentMaterial;


    private Rigidbody rb;
    private Animator anim; // Animator ������Ʈ

    private Vector3 moveDirection; // �̵� ���� ����



    [SerializeField] TMP_Text nickNameText;

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Rigidbody ������Ʈ ��������
        anim = GetComponent<Animator>(); // Animator ������Ʈ ��������

 
        if (photonView.IsMine)
        {
            nickNameText.text = PhotonNetwork.NickName;
            nickNameText.color = Color.green;

            Select();
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

    [PunRPC]
    public void RPC_Select()
    {
        skinnedMeshRenderer.material = currentMaterial;
    }

    public void Select()
    {
        photonView.RPC("RPC_Select", RpcTarget.AllBuffered);
    }


    [PunRPC]
    public void RPC_ColorChange(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "red":
                CustomizingManager.instance.dumiSkinMeshRenderer.material = CustomizingManager.instance.redColor;
                currentMaterial = CustomizingManager.instance.redColor;
                break;

            case "yellow":
                CustomizingManager.instance.dumiSkinMeshRenderer.material = CustomizingManager.instance.yellowColor;
                currentMaterial = CustomizingManager.instance.yellowColor;
                break;

            case "blue":
                CustomizingManager.instance.dumiSkinMeshRenderer.material = CustomizingManager.instance.blueColor;
                currentMaterial = CustomizingManager.instance.blueColor;
                break;

            default:
                Debug.LogWarning("Unknown color name: " + colorName);
                break;
        }
    }

    public void ColorChange(string colorName)
    {
        photonView.RPC("RPC_ColorChange", RpcTarget.AllBuffered, colorName);
        Select();
    }
    void Move()
    {
        // �Է��� ������ �̵��� FixedUpdate���� ó��
        float moveX = Input.GetAxisRaw("Horizontal"); // A, D
        float moveZ = Input.GetAxisRaw("Vertical");   // W, S

        // �̵� ���� ���
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // �ִϸ��̼� ���� ������Ʈ (�̵� ���� �� �ִϸ��̼� ����)
        anim.SetBool("isMove", moveDirection != Vector3.zero);

        // ������ �̵� ó��
        if (moveDirection != Vector3.zero)
        {
            // �̵�
            Vector3 move = moveDirection * moveSpeed * Time.fixedDeltaTime;
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
