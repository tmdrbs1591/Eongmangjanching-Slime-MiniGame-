using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public CustomizingManager customizingManager;

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // ���� ����Ǿ �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject);  // �̹� �ν��Ͻ��� ������ �ڽ��� �ı�
        }
    }
}
