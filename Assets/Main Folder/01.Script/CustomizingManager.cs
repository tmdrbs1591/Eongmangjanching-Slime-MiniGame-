using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class CustomizingManager : MonoBehaviourPunCallbacks
{
    public static CustomizingManager instance { get; private set; }

    [SerializeField] public SkinnedMeshRenderer dumiSkinMeshRenderer;


    [SerializeField] public Material redColor;
    [SerializeField] public Material yellowColor;
    [SerializeField] public Material blueColor;
    // Start is called before the first frame update
    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // ���� ����Ǿ �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject);  // �̹� �ν��Ͻ��� ������ �ڽ��� �ı�
        }
    }

    // Update is called once per frame
    void Start()
    {
    }


   
}
