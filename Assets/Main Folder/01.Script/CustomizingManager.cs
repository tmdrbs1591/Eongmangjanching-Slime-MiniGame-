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
        // 싱글톤 인스턴스 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 씬이 변경되어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);  // 이미 인스턴스가 있으면 자신을 파괴
        }
    }

    // Update is called once per frame
    void Start()
    {
    }


   
}
