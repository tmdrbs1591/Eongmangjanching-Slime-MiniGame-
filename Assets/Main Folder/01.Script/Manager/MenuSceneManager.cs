using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MenuSceneManager : MonoBehaviour
{
    // �޴� ������ �̵��ϰ� Photon ������ �����ϴ� �Լ�
    public void GoToMenuScene()
    {
        // Photon���� ���� ������ ������ ���´�.
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom(); // ���� ������
            PhotonNetwork.Disconnect(); // ���� ������ ���´�
        }

        // ���� �޴� ������ ���� (�� �̸��� ���� ��� ���� �̸����� ����)
        SceneManager.LoadScene("00.Room"); // �޴� ������ ����
    }

    // Start�� ���� ������� �����Ƿ� �����ص� ����
    void Start()
    {

    }

    // Update�� ���� ������� �����Ƿ� �����ص� ����
    void Update()
    {

    }
}
