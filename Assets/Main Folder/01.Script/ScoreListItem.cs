using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;  // 슬라이더를 사용하기 위한 네임스페이스

public class ScoreListItem : MonoBehaviourPunCallbacks
{

    [SerializeField] TMP_Text nicNametext;
    [SerializeField] TMP_Text scoreText;
    Player player;

    private void Start()
    {
    }
    public void Setup(Player _player)
    {
        player = _player;
        nicNametext.text = _player.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
