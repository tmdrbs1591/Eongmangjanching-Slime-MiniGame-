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

    private string selectedScene; // 선택된 씬 이름 저장 변수

    protected virtual void Start()
    {
        // 처음 시작할 때 텍스트 초기화
        countdownText.text = timeRemaining.ToString();
        FadeOut.SetActive(true);
    }

    protected virtual void Update()
    {
        // 카운트다운 타이머가 0 이하로 내려가면 숫자 감소
        countdownTimer -= Time.deltaTime;

        if (countdownTimer <= 0)
        {
            // 1초가 지나면 카운트다운 숫자 감소
            timeRemaining -= 1;

            // 텍스트 갱신
            countdownText.text = timeRemaining.ToString();

            // 타이머 초기화
            countdownTimer = 1f;

            if (timeRemaining <= 0)
            {
                // 서버에서 랜덤 씬을 선택하여 모든 클라이언트에게 전송
                photonView.RPC("ChooseRandomScene", RpcTarget.MasterClient);
            }
        }
    }

    [PunRPC]
    private void ChooseRandomScene()
    {
        if (sceneName.Length > 0)
        {
            // sceneName 배열에서 랜덤으로 하나의 씬 이름 선택
            int randomIndex = Random.Range(0, sceneName.Length);
            selectedScene = sceneName[randomIndex]; // 선택된 씬 이름 저장

            // 모든 클라이언트에게 씬 전환 요청
            photonView.RPC("LoadSelectedScene", RpcTarget.All);
        }
    }

    [PunRPC]
    private void LoadSelectedScene()
    {
        StartCoroutine(FadeScene());
    }

    protected IEnumerator FadeScene()
    {
        Fadein.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        // 저장된 랜덤 씬으로 이동
        PhotonNetwork.LoadLevel(selectedScene);
    }
}
