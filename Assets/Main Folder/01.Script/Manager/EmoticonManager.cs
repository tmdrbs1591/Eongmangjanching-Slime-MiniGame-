using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EmoticonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject happyEmoticon;
    [SerializeField] GameObject sadEmoticon;
    [SerializeField] GameObject angryEmoticon;

    [SerializeField] GameObject canvas;


    private void Awake()
    {
        if (!photonView.IsMine)
        {
            canvas.SetActive(false);
        }
    }
    public void HappyEmoticonTrue()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_HappyEmoticonTrue", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void RPC_HappyEmoticonTrue()
    {
        happyEmoticon.SetActive(false);
        happyEmoticon.SetActive(true);
    }

    public void SadEmoticonTrue()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_SadEmoticonTrue", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void RPC_SadEmoticonTrue()
    {
        sadEmoticon.SetActive(false);
        sadEmoticon.SetActive(true);
    }

    public void AngryEmoticonTrue()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_AngryEmoticonTrue", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void RPC_AngryEmoticonTrue()
    {
        angryEmoticon.SetActive(false);
        angryEmoticon.SetActive(true);
    }
}
