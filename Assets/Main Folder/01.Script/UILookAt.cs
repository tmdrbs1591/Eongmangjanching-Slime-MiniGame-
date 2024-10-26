using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class UILookAt : MonoBehaviourPunCallbacks
{
    public Transform mainCameraTransform; // ���� ī�޶��� Transform�� ������ ����

    void Start()
    {
        // ���� ī�޶� ã���ϴ�.
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Make sure it is tagged as 'MainCamera'.");
            return;
        }

        // ���� ī�޶��� Transform�� �����ɴϴ�.
        mainCameraTransform = mainCamera.transform;
    }
    void LateUpdate()
    {
        if (mainCameraTransform == null)
        {
            return; // ���� ī�޶� �������� ���� ��� ������Ʈ�� �ߴ��մϴ�.
        }

        // ���� ī�޶��� ��ġ�� UI ����� ��ġ ���̸� ���մϴ�.
        Vector3 direction = mainCameraTransform.position - transform.position;

        // ���� ���͸� ������� ȸ���� ����մϴ�.
        Quaternion lookRotation = Quaternion.LookRotation(-direction, Vector3.up);

        // UI ��Ҹ� ȸ����ŵ�ϴ�.
        transform.rotation = lookRotation;
    }
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ���� ī�޶� ã���ϴ�.
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Make sure it is tagged as 'MainCamera'.");
            return;
        }

        // ���� ī�޶��� Transform�� �����ɴϴ�.
        mainCameraTransform = mainCamera.transform;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded; // ���� ����
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
            Camera mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError("Main camera not found. Make sure it is tagged as 'MainCamera'.");
                return;
            }

            // ���� ī�޶��� Transform�� �����ɴϴ�.
            mainCameraTransform = mainCamera.transform;
    }
}
