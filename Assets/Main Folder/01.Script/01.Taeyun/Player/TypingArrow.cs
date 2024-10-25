using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingArrow : MonoBehaviourPunCallbacks
{
    public int currentArrowCount = 0;
    private bool isFailed = false;  // Ʋ�ȴ��� ���θ� ������ ����
    private PlayerScript player;
    [SerializeField] private float forceAmount;

    [SerializeField] private GameObject failEffect;

    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject upArrow;
    [SerializeField] private GameObject downArrow;

    private void OnEnable()
    {
        player = GetComponentInParent<PlayerScript>();
        player.rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        if (ArrowEventManager.instance != null)
        {
            // �ߺ� ���� ����
            ArrowEventManager.instance.OnTypingTimeEnd -= OnTypingTimeEnd;
            ArrowEventManager.instance.OnTypingTimeEnd += OnTypingTimeEnd;
        }

        ResetState(); // �̺�Ʈ ���� �� ���� �ʱ�ȭ
    }

    private void OnDisable()
    {
        player.rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (ArrowEventManager.instance != null)
        {
            ArrowEventManager.instance.OnTypingTimeEnd -= OnTypingTimeEnd;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Ʋ�Ȱų� �Է� �ð��� �ƴ� ��� ó������ ����
            if (isFailed || ArrowEventManager.instance.isTypingTime == false)
            {
                currentArrowCount = 0;
                return;
            }

            if (currentArrowCount < ArrowEventManager.instance.arrowCount)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    HandleArrowInput("��", "Up");
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    HandleArrowInput("��", "Down");
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    HandleArrowInput("��", "Left");
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    HandleArrowInput("��", "Right");
                }
            }
        }
    }

    private void HandleArrowInput(string arrow, string arrowDirection)
    {
        bool isCorrect = ArrowEventManager.instance.CheckArrow(arrow, currentArrowCount);
        photonView.RPC("SetArrowActive", RpcTarget.All, isCorrect, arrowDirection);

        if (isCorrect)
        {
            currentArrowCount++; // �¾��� ���� ����
        }
        else
        {
            FailedArrow(); // Ʋ������ ���� ó��
        }
    }

    [PunRPC]
    private void SetArrowActive(bool isCorrect, string arrowDirection)
    {
        // ��� ȭ��ǥ�� ��Ȱ��ȭ�ϰ� ���� ȭ��ǥ�� Ȱ��ȭ
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        upArrow.SetActive(false);
        downArrow.SetActive(false);

        if (isCorrect)
        {
            switch (arrowDirection)
            {
                case "Up":
                    upArrow.SetActive(true);
                    break;
                case "Down":
                    downArrow.SetActive(true);
                    break;
                case "Left":
                    leftArrow.SetActive(true);
                    break;
                case "Right":
                    rightArrow.SetActive(true);
                    break;
            }
        }
    }

    private void FailedArrow()
    {
        if (photonView.IsMine)
        {
            isFailed = true;
            player.rb.constraints = RigidbodyConstraints.None;
            PhotonNetwork.Instantiate(failEffect.name, transform.position, Quaternion.identity);
            // ĳ���Ͱ� �ڷ� ���ư��� �ϴ� ����
            player.rb.AddForce((-transform.forward + Vector3.up) * forceAmount, ForceMode.Impulse);
        }
    }

    private void OnTypingTimeEnd()
    {
        // Ÿ���� �ð��� ������ �� ���� ��� ȭ��ǥ�� �Է����� ���� ��� ���� ó��
        if (currentArrowCount < ArrowEventManager.instance.arrowCount)
        {
            FailedArrow();
        }
        else
        {
            ResetState(); // �̺�Ʈ�� ���� �� ���� �ʱ�ȭ
        }
    }

    private void ResetState()
    {
        // �̺�Ʈ�� ���۵� ���� ���� �� ���¸� �ʱ�ȭ
        isFailed = false;
        currentArrowCount = 0;
        player.rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }
}
