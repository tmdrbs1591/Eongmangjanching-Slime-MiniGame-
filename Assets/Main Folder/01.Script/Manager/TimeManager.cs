using System.Collections;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class TimeManager : MonoBehaviourPunCallbacks
{
    [SerializeField] protected TMP_Text countdownText; // 텍스트 UI를 위한 변수
    [SerializeField] protected string[] sceneName; // 씬 이름 배열
    public int timeRemaining = 5; // 5초 카운트다운, int로 설정
    protected float countdownTimer = 1f; // 1초 카운트다운 타이머

    [SerializeField] protected GameObject Fadein;
    [SerializeField] protected GameObject FadeOut;

    protected virtual void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // 씬 자동 동기화 설정
    }

    protected virtual void Start()
    {
        // 처음 시작할 때 텍스트 초기화
        countdownText.text = timeRemaining.ToString();
        FadeOut.SetActive(true);
    }

    protected virtual void Update()
    {
        countdownTimer -= Time.deltaTime;

        if (countdownTimer <= 0)
        {
            timeRemaining -= 1;
            countdownText.text = timeRemaining.ToString();
            countdownTimer = 1f;

            if (timeRemaining <= 0)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(FadeScene());
                }
            }
        }
    }

    protected IEnumerator FadeScene()
    {
        Fadein.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        // 씬 전환을 위한 RPC 호출
        photonView.RPC("LoadRandomScene", RpcTarget.All);
    }

    [PunRPC]
    public void LoadRandomScene() // 접근성을 public으로 변경
    {
        // 랜덤으로 씬 이름 선택
        int randomIndex = Random.Range(0, sceneName.Length);
        string randomScene = sceneName[randomIndex];

        // 랜덤으로 선택된 씬으로 이동
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트만 LoadLevel 호출
        {
            PhotonNetwork.LoadLevel(randomScene);
        }
    }
}
