using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShootArrow : MonoBehaviourPunCallbacks
{
    public GameObject player;
    public int power;
    public Transform target;

    public float coolTime;
    public bool isCanShoot;

    public GameObject arrowPrefab;

    public string name;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && photonView.IsMine)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && isCanShoot)
            {
                isCanShoot = false;
                Debug.Log("hit to " + hit.transform.gameObject.name + " at position " + hit.point);
                if (target == null)
                {
                    GameObject tempTarget = new GameObject("TempTarget");
                    target = tempTarget.transform;
                }

                target.position = hit.point;

                GameObject TempArrow = PhotonNetwork.Instantiate(arrowPrefab.name, transform.position, Quaternion.identity);
                TempArrow.GetComponent<ArrowInit>().initArrow(transform.position, target, power, name);
                StartCoroutine("CoolTime");
            }
        }
    }

    IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(coolTime);
        isCanShoot = true;
    }
}