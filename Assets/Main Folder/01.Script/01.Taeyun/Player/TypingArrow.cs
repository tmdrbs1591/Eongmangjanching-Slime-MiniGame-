using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingArrow : MonoBehaviourPunCallbacks
{
    public int currentArrowCount = 0;
    private bool isFailed = false;  // 틀렸는지 여부를 추적할 변수

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
                    if (!isCorrect) isFailed = true;  // 틀렸으면 isFailed를 true로 설정
                    currentArrowCount++;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    bool isCorrect = ArrowEventManager.instance.CheckArrow("↓", currentArrowCount);
                    if (!isCorrect) isFailed = true;
                    currentArrowCount++;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    bool isCorrect = ArrowEventManager.instance.CheckArrow("←", currentArrowCount);
                    if (!isCorrect) isFailed = true;
                    currentArrowCount++;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    bool isCorrect = ArrowEventManager.instance.CheckArrow("→", currentArrowCount);
                    if (!isCorrect) isFailed = true;
                    currentArrowCount++;
                }
            }
        }
    }
}
