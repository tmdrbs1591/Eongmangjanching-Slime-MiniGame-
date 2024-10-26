using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootArrow : MonoBehaviour
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
        if (Input.GetMouseButtonDown(0))
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

                GameObject TempArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
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