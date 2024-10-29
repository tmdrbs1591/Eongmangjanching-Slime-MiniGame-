using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGoal : MonoBehaviour
{

    GameObject player;
    private List<int> scores = new List<int> { 1500, 1000, 500, 200 };
    private int playerCount = 0; // 결승선에 도착한 플레이어 수
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        // 플레이어를 태그로 찾아 할당합니다.
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
        }

     
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 이미 점수를 받은 플레이어는 다시 점수를 받지 않도록 설정
            if (playerCount < scores.Count)
            {
                player = other.gameObject; // 충돌한 플레이어 객체를 저장
                var playerScoreScript = player.GetComponent<PlayerScore>();
                playerScoreScript.AddScore(scores[playerCount]); // 순서에 맞게 점수를 추가

                playerCount++; // 플레이어 수 증가
            }
        }
    }

}
