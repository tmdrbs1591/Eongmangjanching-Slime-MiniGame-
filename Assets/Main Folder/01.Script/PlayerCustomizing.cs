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
                GameManager.instance.dumiSkinMeshRenderer.material = redColor;
                currentMaterial = redColor;
                break;

            case "yellow":
                GameManager.instance.dumiSkinMeshRenderer.material = yellowColor;

                currentMaterial = yellowColor;
                break;

            case "blue":
                GameManager.instance.dumiSkinMeshRenderer.material = blueColor;
                currentMaterial =blueColor;
                break;

            default:
                Debug.LogWarning("Unknown color name: " + colorName);
                break;
        }
    }

    public void ColorChange(string colorName)
    {
        photonView.RPC("RPC_ColorChange", RpcTarget.AllBuffered, colorName);
        Select();
    }

}
