using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCustomizing : MonoBehaviourPunCallbacks
{
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] public Material currentMaterial;
    [SerializeField] public GameObject currentHat;

    [SerializeField] public Material redColor;
    [SerializeField] public Material yellowColor;
    [SerializeField] public Material blueColor;
    [SerializeField] public Material whiteColor;
    [SerializeField] public Material pinkColor;
    [SerializeField] public Material skyColor;

    [SerializeField] private GameObject vikingHat;
    [SerializeField] private GameObject sproutHat;
    [SerializeField] private GameObject leafHat;
    [SerializeField] private GameObject metalHat;

    void Start()
    {
        if (photonView.IsMine)
        {
            Select();
        }
    }

    [PunRPC]
    public void RPC_Select()
    {
        skinnedMeshRenderer.material = currentMaterial;
        if (currentHat != null)
        {
            currentHat.SetActive(true); // 현재 모자를 활성화
        }
    }

    public void Select()
    {
        photonView.RPC("RPC_Select", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RPC_HatChange(string hatName)
    {
        // 현재 모자를 비활성화
        if (currentHat != null)
        {
            currentHat.SetActive(false);
        }

        switch (hatName.ToLower())
        {
            case "viking":
                currentHat = vikingHat;
                break;
            case "sprouthat": // 수정: "sproutHat"을 "sprouthat"로 변경
                currentHat = sproutHat;
                break;
            case "leafhat":
                currentHat = leafHat;
                break;
            case "metalhat":
                currentHat = metalHat;
                break;


            default:
                Debug.LogWarning("Unknown hat name: " + hatName);
                return; // 알 수 없는 모자 이름 처리
        }

    }
    void DumiHatChange(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "viking":
                GameManager.instance.HatActive(GameManager.instance.vikingHat);
                break;
            case "sprouthat": // 수정: "sproutHat"을 "sprouthat"로 변경
                GameManager.instance.HatActive(GameManager.instance.sproutHat);
                break;
            case "leafhat":
                GameManager.instance.HatActive(GameManager.instance.leafHat);
                break;
            case "metalhat":
                GameManager.instance.HatActive(GameManager.instance.metalHat);
                break;
            default:
                Debug.LogWarning("Unknown color name: " + colorName);
                break;
        }
    }
    [PunRPC]
    public void RPC_ColorChange(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "red":
                currentMaterial = redColor;
                break;
            case "yellow":
                currentMaterial = yellowColor;
                break;
            case "blue":
                currentMaterial = blueColor;
                break;
            case "white":
                currentMaterial = whiteColor;
                break;
            case "sky":
                currentMaterial = skyColor;
                break;
            case "pink":
                currentMaterial = pinkColor;
                break;
            default:
                Debug.LogWarning("Unknown color name: " + colorName);
                break;
        }
    }

    void ColorChangeGM(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "red":
                GameManager.instance.dumiSkinMeshRenderer.material = redColor;
                break;
            case "yellow":
                GameManager.instance.dumiSkinMeshRenderer.material = yellowColor;
                break;
            case "blue":
                GameManager.instance.dumiSkinMeshRenderer.material = blueColor;
                break;
            case "white":
                GameManager.instance.dumiSkinMeshRenderer.material = whiteColor;
                break;
            case "sky":
                GameManager.instance.dumiSkinMeshRenderer.material = skyColor;
                break;
            case "pink":
                GameManager.instance.dumiSkinMeshRenderer.material = pinkColor;
                break;
            default:
                Debug.LogWarning("Unknown color name: " + colorName);
                break;
        }
    }

    public void ColorChange(string colorName)
    {
        photonView.RPC("RPC_ColorChange", RpcTarget.AllBuffered, colorName);
        ColorChangeGM(colorName);
        Select();
    }

    public void HatChange(string hatName)
    {
        photonView.RPC("RPC_HatChange", RpcTarget.AllBuffered, hatName);
        DumiHatChange(hatName);
        Select();
    }
}
