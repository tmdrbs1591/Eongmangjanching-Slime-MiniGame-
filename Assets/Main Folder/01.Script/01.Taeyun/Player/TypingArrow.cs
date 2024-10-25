using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingArrow : MonoBehaviourPunCallbacks
{
    public int currentArrowCount = 0;
    private bool isFailed = false;  // 틀렸는지 여부를 추적할 변수
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
            // 중복 구독 방지
            ArrowEventManager.instance.OnTypingTimeEnd -= OnTypingTimeEnd;
            ArrowEventManager.instance.OnTypingTimeEnd += OnTypingTimeEnd;
        }

        ResetState(); // 이벤트 시작 시 상태 초기화
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
            // 틀렸거나 입력 시간이 아닐 경우 처리하지 않음
            if (isFailed || ArrowEventManager.instance.isTypingTime == false)
            {
                currentArrowCount = 0;
                return;
            }

            if (currentArrowCount < ArrowEventManager.instance.arrowCount)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    HandleArrowInput("↑", "Up");
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    HandleArrowInput("↓", "Down");
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    HandleArrowInput("←", "Left");
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    HandleArrowInput("→", "Right");
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
            currentArrowCount++; // 맞았을 때만 증가
        }
        else
        {
            FailedArrow(); // 틀렸으면 실패 처리
        }
    }

    [PunRPC]
    private void SetArrowActive(bool isCorrect, string arrowDirection)
    {
        // 모든 화살표를 비활성화하고 맞춘 화살표만 활성화
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
            // 캐릭터가 뒤로 날아가게 하는 로직
            player.rb.AddForce((-transform.forward + Vector3.up) * forceAmount, ForceMode.Impulse);
        }
    }

    private void OnTypingTimeEnd()
    {
        // 타이핑 시간이 끝났을 때 아직 모든 화살표를 입력하지 못한 경우 실패 처리
        if (currentArrowCount < ArrowEventManager.instance.arrowCount)
        {
            FailedArrow();
        }
        else
        {
            ResetState(); // 이벤트가 끝날 때 상태 초기화
        }
    }

    private void ResetState()
    {
        // 이벤트가 시작될 때나 끝날 때 상태를 초기화
        isFailed = false;
        currentArrowCount = 0;
        player.rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }
}
