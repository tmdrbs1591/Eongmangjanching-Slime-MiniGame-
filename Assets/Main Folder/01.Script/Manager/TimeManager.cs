using System.Collections;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class TimeManager : MonoBehaviourPunCallbacks
{
    [SerializeField] protected TMP_Text countdownText; // �ؽ�Ʈ UI�� ���� ����
    [SerializeField] protected string[] sceneName; // �� �̸� �迭
    public int timeRemaining = 5; // 5�� ī��Ʈ�ٿ�, int�� ����
    protected float countdownTimer = 1f; // 1�� ī��Ʈ�ٿ� Ÿ�̸�

    [SerializeField] protected GameObject Fadein;
    [SerializeField] protected GameObject FadeOut;

    protected virtual void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // �� �ڵ� ����ȭ ����
    }

    protected virtual void Start()
    {
        // ó�� ������ �� �ؽ�Ʈ �ʱ�ȭ
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
        photonView.RPC("ActivateFadeIn", RpcTarget.All); // ��� Ŭ���̾�Ʈ���� Fadein Ȱ��ȭ

        yield return new WaitForSeconds(1.5f);

        // �� ��ȯ�� ���� RPC ȣ��
        photonView.RPC("LoadRandomScene", RpcTarget.All);
    }

    [PunRPC]
    void ActivateFadeIn()
    {
        Fadein.SetActive(true); // ��� Ŭ���̾�Ʈ���� Fadein Ȱ��ȭ
    }


    [PunRPC]
    public void LoadRandomScene() // ���ټ��� public���� ����
    {
        // �������� �� �̸� ����
        int randomIndex = Random.Range(0, sceneName.Length);
        string randomScene = sceneName[randomIndex];

        // �������� ���õ� ������ �̵�
        if (PhotonNetwork.IsMasterClient) // ������ Ŭ���̾�Ʈ�� LoadLevel ȣ��
        {
            PhotonNetwork.LoadLevel(randomScene);
        }
    }
}
