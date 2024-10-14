using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UIManager : MonoBehaviourPunCallbacks
{
    // �̱��� �ν��Ͻ�
    public static UIManager instance { get; private set; }

    [SerializeField] public GameObject customizingPanel;


    // UI ��Ҹ� ���� ����
    private Stack<GameObject> uiStack = new Stack<GameObject>();

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

    // Update is called once per frame
    void Update()
    {
        // ESC Ű �Է� ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PopUI();
        }
    }

    // ���ÿ� UI ������Ʈ�� �߰��ϴ� �Լ�
    public void PushUI(GameObject uiElement)
    {
        // UI ��Ҹ� ���ÿ� �߰�
        uiStack.Push(uiElement);
        // UI Ȱ��ȭ
        uiElement.SetActive(true);
    }

    // ���ÿ��� UI ������Ʈ�� �����ϴ� �Լ�
    public void PopUI()
    {
            // ���ÿ� ��Ұ� �ִ��� Ȯ��
            if (uiStack.Count > 0)
            {
                // ���� �ֱٿ� �߰��� UI ��Ҹ� ������ ��Ȱ��ȭ
                GameObject topUI = uiStack.Pop();
                topUI.SetActive(false);
            }
            else
            {
                Debug.Log("UI ������ ��� �ֽ��ϴ�.");
            }
    }
}
