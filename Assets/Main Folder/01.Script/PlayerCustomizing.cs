using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCustomizing : MonoBehaviourPunCallbacks
{
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] public Material currentMaterial;


    [SerializeField] public Material redColor;
    [SerializeField] public Material yellowColor;
    [SerializeField] public Material blueColor;
    [SerializeField] public Material whiteColor;
    [SerializeField] public Material pinkColor;
    [SerializeField] public Material skyColor;
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
    }

    public void Select()
    {
        photonView.RPC("RPC_Select", RpcTarget.AllBuffered);
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

}
