using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MenuSceneManager : MonoBehaviour
{
    // 메뉴 씬으로 이동하고 Photon 연결을 종료하는 함수
    public void GoToMenuScene()
    {
        // Photon에서 방을 떠나고 연결을 끊는다.
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom(); // 방을 떠난다
            PhotonNetwork.Disconnect(); // 서버 연결을 끊는다
        }

        // 씬을 메뉴 씬으로 변경 (씬 이름은 실제 사용 중인 이름으로 변경)
        SceneManager.LoadScene("00.Room"); // 메뉴 씬으로 변경
    }

    // Start는 현재 사용하지 않으므로 삭제해도 무방
    void Start()
    {

    }

    // Update는 현재 사용하지 않으므로 삭제해도 무방
    void Update()
    {

    }
}
