using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬을 변경하려면 필요
using Photon.Pun;

public class TimeManager : MonoBehaviourPunCallbacks
{
    [SerializeField] protected TMP_Text countdownText; // 텍스트 UI를 위한 변수
    [SerializeField] protected string sceneName;
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
                StartCoroutine(FadeScene());
            }
        }
    }

    protected IEnumerator FadeScene()
    {
        Fadein.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        PhotonNetwork.LoadLevel(sceneName);


    }
}
