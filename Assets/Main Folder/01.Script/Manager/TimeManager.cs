using System.Collections;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class TimeManager : MonoBehaviourPunCallbacks
{
    [SerializeField] protected TMP_Text countdownText; // �ؽ�Ʈ UI�� ���� ����
    [SerializeField] protected string[] sceneNames; // �� �̸� �迭
    public int timeRemaining = 5; // 5�� ī��Ʈ�ٿ�, int�� ����
    protected float countdownTimer = 1f; // 1�� ī��Ʈ�ٿ� Ÿ�̸�

    [SerializeField] protected GameObject Fadein;
    [SerializeField] protected GameObject FadeOut;

    protected virtual void Start()
    {
        // ó�� ������ �� �ؽ�Ʈ �ʱ�ȭ
        countdownText.text = timeRemaining.ToString();
        FadeOut.SetActive(true);
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
                // ������ Ŭ���̾�Ʈ���� ���� �ε��� ����
                if (PhotonNetwork.IsMasterClient)
                {
                    int randomIndex = Random.Range(0, sceneNames.Length);
                    PhotonView photonView = PhotonView.Get(this);
                    photonView.RPC("LoadRandomScene", RpcTarget.All, randomIndex);
                }
            }
        }
    }

    [PunRPC]
    private void LoadRandomScene(int index)
    {
        StartCoroutine(FadeScene(sceneNames[index]));
    }

    protected IEnumerator FadeScene(string sceneName)
    {
        Fadein.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        // ���õ� ������ �̵�
        PhotonNetwork.LoadLevel(sceneName);
    }
}
