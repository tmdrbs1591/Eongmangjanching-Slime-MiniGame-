using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static GridSpawner;
using Unity.VisualScripting;
using Photon.Realtime;
using static FloorSarajim;
public class FullManager : MonoBehaviour
{
    public FloorSarajim floorManager;
    public TextMeshProUGUI systemTMP;
    public GameObject floor;

    public GameObject player;

    public Material lightMat;
    public Material normalMat;

    public void Start()
    {

        StartCoroutine(InGame());
    }

    private void Update()
    {
        for (int i = 0; i < floorManager.floors.Length; i++)
        {
            if (player.transform.position.y > (floorManager.floors[i].yOffset - 2f) && player.transform.position.y < (floorManager.floors[i].yOffset + 5f))
            {
                Camera.main.transform.position = floorManager.floors[i].CameraPosition;
                Camera.main.fieldOfView = floorManager.floors[i].CameraFov;
            }
        }
    }

    IEnumerator InGame()
    {
        yield return new WaitForSeconds(3);
        systemTMP.text = "잠시후 게임이 시작합니다\n3";
        yield return new WaitForSeconds(1);
        systemTMP.text = "잠시후 게임이 시작합니다\n2";
        yield return new WaitForSeconds(1);
        systemTMP.text = "잠시후 게임이 시작합니다\n1";
        yield return new WaitForSeconds(1);
        systemTMP.text = "����!";
        floor.SetActive(false);
        StartCoroutine(Timer());
        yield return new WaitForSeconds(9);
        StartCoroutine(Timer());
        yield return new WaitForSeconds(9);
        StartCoroutine(Timer());
        yield return new WaitForSeconds(9);
        systemTMP.text = "END!";
    }

    IEnumerator Timer()
    {
        int DelTX = Random.Range(0, floorManager.floorList.Length);
        int DelTY = Random.Range(0, floorManager.floorList[0].Length);
        floorManager.floorList[DelTX][DelTY].gameObject.GetComponent<MeshRenderer>().material = lightMat;
        systemTMP.text = "잠시후 바닥이 사라집니다!\n5";
        yield return new WaitForSeconds(1);
        systemTMP.text = "잠시후 바닥이 사라집니다!\n4";
        yield return new WaitForSeconds(1);
        systemTMP.text = "잠시후 바닥이 사라집니다!\n3";
        yield return new WaitForSeconds(1);
        floorManager.floorList[DelTX][DelTY].gameObject.GetComponent<MeshRenderer>().material = normalMat;
        systemTMP.text = "잠시후 바닥이 사라집니다!\n2";
        yield return new WaitForSeconds(1);
        systemTMP.text = "잠시후 바닥이 사라집니다!\n1";
        yield return new WaitForSeconds(1);
        systemTMP.text = "잠시후 바닥이 사라집니다!\n0";
        RandomDel(DelTX, DelTY);
        yield return new WaitForSeconds(2);
        Recover();
        systemTMP.text = "";
        yield return new WaitForSeconds(2);
    }

    public void RandomDel(int x, int y)
    {
        

        for (int i = 0; i < floorManager.floorList.Length; i++)
        {
            for (int j = 0; j < floorManager.floorList[0].Length; j++)
            {
                if(i != x || j != y)
                {
                    floorManager.floorList[i][j].SetActive(false);
                }
            }
        }
    }

    public void Recover()
    {
        for (int i = 0; i < floorManager.floorList.Length; i++)
        {
            for (int j = 0; j < floorManager.floorList[0].Length; j++)
            {
                floorManager.floorList[i][j].SetActive(true);
            }
        }
    }
}
