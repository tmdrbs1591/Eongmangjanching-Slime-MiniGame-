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

    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject upArrow;
    [SerializeField] private GameObject downArrow;

    private void OnEnable()
    {
        player = GetComponentInParent<PlayerScript>();
        player.rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    private void OnDisable()
    {
        player.rb.constraints = RigidbodyConstraints.None;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // 틀렸거나 입력 시간이 아닐 경우 처리하지 않음
            if (isFailed || ArrowEventManager.instance.isTypingTime == false)
            {
                return;
            }

            if (currentArrowCount < ArrowEventManager.instance.arrowCount)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    bool isCorrect = ArrowEventManager.instance.CheckArrow("↑", currentArrowCount);
                    photonView.RPC("SetArrowActive", RpcTarget.All, isCorrect, "Up");
                    if (!isCorrect) FailedArrow();
                    currentArrowCount++;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    bool isCorrect = ArrowEventManager.instance.CheckArrow("↓", currentArrowCount);
                    photonView.RPC("SetArrowActive", RpcTarget.All, isCorrect, "Down");
                    if (!isCorrect) FailedArrow();
                    currentArrowCount++;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    bool isCorrect = ArrowEventManager.instance.CheckArrow("←", currentArrowCount);
                    photonView.RPC("SetArrowActive", RpcTarget.All, isCorrect, "Left");
                    if (!isCorrect) FailedArrow();
                    currentArrowCount++;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    bool isCorrect = ArrowEventManager.instance.CheckArrow("→", currentArrowCount);
                    photonView.RPC("SetArrowActive", RpcTarget.All, isCorrect, "Right");
                    if (!isCorrect) FailedArrow();
                    currentArrowCount++;
                }
            }
        }
    }

    [PunRPC]
    private void SetArrowActive(bool isCorrect, string arrowDirection)
    {
        // 맞춘 화살표만 true로, 나머지는 false로 설정
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
        isFailed = true;
        player.rb.constraints = RigidbodyConstraints.None;

        player.rb.AddForce((-transform.forward + Vector3.up) * forceAmount, ForceMode.Impulse);
    }
}
