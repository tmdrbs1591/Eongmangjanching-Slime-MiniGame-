using UnityEngine;
using TMPro;
using System.Xml;

public class BalloonController : MonoBehaviour
{
    // 크기 증가량 변수

     private int nowCount = 0;            // 현재까지 누른 Space 횟수
    [SerializeField] private int maxCount = 5;              // 풍선 최대 증가 횟수
    [SerializeField] private float addSize = 0.05f;      // 증가할 크기
    private Transform balloon;               // 각 플레이어의 풍선 프리팹 참조
    private float currentSize;        // 풍선 현재 크기
    private TMP_Text scaleText;

    void Start()
    {
        balloon = this.transform;
        currentSize = balloon.localScale.x;

        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            scaleText = canvas.GetComponentInChildren<TMP_Text>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && nowCount < maxCount)
        {
            nowCount++;
        }
        else if(nowCount >= maxCount)
        {
            AddScaleBalloon();
        }
    }

    // 풍선을 키우고 텍스트를 갱신하는 함수
    public void AddScaleBalloon()
    {
        currentSize += addSize;
        balloon.localScale = Vector3.one * currentSize;
        UpdateScaeleText();
        nowCount = 0;
    }

    private void UpdateScaeleText()
    {
        scaleText.text = currentSize.ToString("F2");
    }

    // 풍선의 현재 크기 반환
    public float GetCurrentSize()
    {
        return currentSize;
    }
}
