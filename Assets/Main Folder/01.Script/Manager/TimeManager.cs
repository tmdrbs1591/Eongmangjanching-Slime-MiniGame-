using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬을 변경하려면 필요
using Photon.Pun;

public class TimeManager : MonoBehaviourPunCallbacks
{
    [SerializeField] protected TMP_Text countdownText; // 텍스트 UI를 위한 변수
    [SerializeField] protected string[] sceneName; // 씬 이름 배열
    public int timeRemaining = 5; // 5초 카운트다운, int로 설정
    protected float countdownTimer = 1f; // 1초 카운트다운 타이머

    [SerializeField] protected GameObject Fadein;
    [SerializeField] protected GameObject FadeOut;

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
                int randomIndex = Random.Range(0, sceneName.Length);
                string randomScene = sceneName[randomIndex];

                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(FadeScene(randomScene));
                    photonView.RPC("SyncFadeScene", RpcTarget.All, randomScene); // 씬 이름 전달
                }
                else
                {
                    photonView.RPC("SyncFadeScene", RpcTarget.All, randomScene); // 다른 클라이언트에게 씬 전환을 동기화
                }
            }
        }
    }

    protected IEnumerator FadeScene(string sceneToLoad)
    {
        Fadein.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        // 랜덤으로 선택된 씬으로 이동
        PhotonNetwork.LoadLevel(sceneToLoad);
    }

    [PunRPC]
    private void SyncFadeScene(string sceneToLoad)
    {
        StartCoroutine(FadeScene(sceneToLoad)); // 마스터 클라이언트의 씬 전환에 맞춰 실행
    }
}
