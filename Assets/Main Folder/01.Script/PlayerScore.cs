using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerScore : MonoBehaviourPunCallbacks
{
    public float currentScore;

    [SerializeField] TMP_Text scoreText;
    [SerializeField] public GameObject crcown;

    [SerializeField] public bool isDeath;

    [SerializeField] public GameObject crown;
    [SerializeField] public GameObject hammer;

    private void Start()
    {
        GameManager.instance.playerScores.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = currentScore.ToString();

        // 점수 추가
        if (Input.GetKeyDown(KeyCode.H) && photonView.IsMine)
        {
            AddScore(1000);
        }
    }

    // 점수 추가 함수
    [PunRPC] // PunRPC 어트리뷰트로 네트워크에서 호출 가능한 함수로 만듬
    void AddScoreRPC(float score)
    {
        currentScore += score;
        GameManager.instance.UpdateCrown();  // 점수 변경 시 왕관 갱신
    }

    public void AddScore(float score)
    {
        // 네트워크를 통해 모든 클라이언트에 점수 업데이트를 전달
        photonView.RPC("AddScoreRPC", RpcTarget.All, score);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ocean"))
        {
            isDeath = true;
        }
    }
}
