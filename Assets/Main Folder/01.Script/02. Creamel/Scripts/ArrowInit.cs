using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowInit : MonoBehaviour
{
    public string playerName;

    public void initArrow(Vector3 startPosition, Transform targetTrans, int power, string player)
    {
        playerName = player;
        transform.LookAt(targetTrans);
        GetComponent<Rigidbody>().AddForce(transform.forward * power);
    }
}