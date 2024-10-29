using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGoal : MonoBehaviour
{

    GameObject player;
    private List<int> scores = new List<int> { 1500, 1000, 500, 200 };
    private int playerCount = 0; // ��¼��� ������ �÷��̾� ��
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        // �÷��̾ �±׷� ã�� �Ҵ��մϴ�.
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
        }

     
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // �̹� ������ ���� �÷��̾�� �ٽ� ������ ���� �ʵ��� ����
            if (playerCount < scores.Count)
            {
                player = other.gameObject; // �浹�� �÷��̾� ��ü�� ����
                var playerScoreScript = player.GetComponent<PlayerScore>();
                playerScoreScript.AddScore(scores[playerCount]); // ������ �°� ������ �߰�

                playerCount++; // �÷��̾� �� ����
            }
        }
    }

}
