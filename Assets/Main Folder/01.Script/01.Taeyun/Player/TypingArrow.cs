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
            // Ʋ�Ȱų� �Է� �ð��� �ƴ� ��� ó������ ����
            if (isFailed || ArrowEventManager.instance.isTypingTime == false)
            {
                return;
            }

            if (currentArrowCount < ArrowEventManager.instance.arrowCount)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    bool isCorrect = ArrowEventManager.instance.CheckArrow("��", currentArrowCount);
                    if (!isCorrect) FailedArrow();  // Ʋ������ isFailed�� true�� ����
                    currentArrowCount++;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    bool isCorrect = ArrowEventManager.instance.CheckArrow("��", currentArrowCount);
                    if (!isCorrect) FailedArrow();
                    currentArrowCount++;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    bool isCorrect = ArrowEventManager.instance.CheckArrow("��", currentArrowCount);
                    if (!isCorrect) FailedArrow();
                    currentArrowCount++;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    bool isCorrect = ArrowEventManager.instance.CheckArrow("��", currentArrowCount);
                    if (!isCorrect) FailedArrow();
                    currentArrowCount++;
                }
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
