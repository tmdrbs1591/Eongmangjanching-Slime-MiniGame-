// # System
using System.Collections;
using System.Collections.Generic;
using TMPro;

// # Unity
using UnityEngine;

public class ArrowEventManager : TimeManager
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI arrowText;
    [SerializeField] private TextMeshProUGUI readyText;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(ArrowEvent());
    }

    IEnumerator ArrowEvent()
    {
        readyText.text = "Ready..";
        yield return new WaitForSeconds(1f);
        readyText.text = "Go!!";
        yield return new WaitForSeconds(1f);
        readyText.text = "";

        readyText.text = "";
        yield return StartCoroutine(StartPhase(4, 2));

        readyText.text = "입력하세요!";
        yield return new WaitForSeconds(6f);

        readyText.text = "";
        yield return StartCoroutine(StartPhase(6, 3));

        readyText.text = "입력하세요!";
        yield return new WaitForSeconds(6f);

        readyText.text = "";
        yield return StartCoroutine(StartPhase(10, 5));

        readyText.text = "입력하세요!";
        yield return new WaitForSeconds(6f);
    }

    IEnumerator StartPhase(float arrowCount, float time)
    {
        float delayTime = time / arrowCount;

        Debug.Log(delayTime);
        
        for (int i = 0; i < arrowCount; i++)
        {
            arrowText.text += RandomArrow();
            yield return new WaitForSeconds(delayTime);
        }

        arrowText.text = "";
    }

    private string RandomArrow()
    {
        int i = Random.Range(0, 3);
        switch (i)
        {
            case 0:
                return "→";
            case 1:
                return "←";
            case 2:
                return "↑";
            case 3:
                return "↓";
        }

        return null;
    }
}
