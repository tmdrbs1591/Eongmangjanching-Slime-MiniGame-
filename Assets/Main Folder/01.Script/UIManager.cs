using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UIManager : MonoBehaviourPunCallbacks
{
    // 싱글톤 인스턴스
    public static UIManager instance { get; private set; }

    [SerializeField] public GameObject customizingPanel;


    // UI 요소를 담을 스택
    private Stack<GameObject> uiStack = new Stack<GameObject>();

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 씬이 변경되어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);  // 이미 인스턴스가 있으면 자신을 파괴
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PopUI();
        }
    }

    // 스택에 UI 오브젝트를 추가하는 함수
    public void PushUI(GameObject uiElement)
    {
        // UI 요소를 스택에 추가
        uiStack.Push(uiElement);
        // UI 활성화
        uiElement.SetActive(true);
    }

    // 스택에서 UI 오브젝트를 제거하는 함수
    public void PopUI()
    {
            // 스택에 요소가 있는지 확인
            if (uiStack.Count > 0)
            {
                // 가장 최근에 추가된 UI 요소를 가져와 비활성화
                GameObject topUI = uiStack.Pop();
                topUI.SetActive(false);
            }
            else
            {
                Debug.Log("UI 스택이 비어 있습니다.");
            }
    }
}
