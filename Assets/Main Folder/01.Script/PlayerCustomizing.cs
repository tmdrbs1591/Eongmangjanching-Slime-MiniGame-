using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCustomizing : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject Canvas;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            Canvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
