using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // ���� �����Ϸ��� �ʿ�
using Photon.Pun;

public class TimeManager : MonoBehaviourPunCallbacks
{
    [SerializeField] protected TMP_Text countdownText; // �ؽ�Ʈ UI�� ���� ����
    [SerializeField] protected string sceneName;
    public int timeRemaining = 5; // 5�� ī��Ʈ�ٿ�, int�� ����
    protected float countdownTimer = 1f; // 1�� ī��Ʈ�ٿ� Ÿ�̸�


    void Start()
    {
        // ó�� ������ �� �ؽ�Ʈ �ʱ�ȭ
        countdownText.text = timeRemaining.ToString();
    }

    protected virtual void Update()
    {
        // ī��Ʈ�ٿ� Ÿ�̸Ӱ� 0 ���Ϸ� �������� ���� ����
        countdownTimer -= Time.deltaTime;

        if (countdownTimer <= 0)
        {
            // 1�ʰ� ������ ī��Ʈ�ٿ� ���� ����
            timeRemaining -= 1;

            // �ؽ�Ʈ ����
            countdownText.text = timeRemaining.ToString();

            // Ÿ�̸� �ʱ�ȭ
            countdownTimer = 1f;

            if (timeRemaining <= 0)
            {
                // 0�� �Ǹ� �� ��ȯ
                SceneManager.LoadScene(sceneName); // "NextScene"�� ��ȯ�� ���� �̸����� �ٲ��ּ���.
            }
        }
    }
}
