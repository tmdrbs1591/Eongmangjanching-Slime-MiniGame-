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
        systemTMP.text = "잠시 후 게임이 시작됩니다\n3";
        yield return new WaitForSeconds(1);
        systemTMP.text = "잠시 후 게임이 시작됩니다\n2";
        yield return new WaitForSeconds(1);
        systemTMP.text = "잠시 후 게임이 시작됩니다\n1";
        yield return new WaitForSeconds(1);
        systemTMP.text = "시작!";
        floor.SetActive(false);
        StartCoroutine(Timer());
        yield return new WaitForSeconds(12);
        StartCoroutine(Timer());
        yield return new WaitForSeconds(12);
    }

    IEnumerator Timer()
    {
        int DelTX = Random.Range(0, floorManager.floorList.Length);
        int DelTY = Random.Range(0, floorManager.floorList[0].Length);
        floorManager.floorList[DelTX][DelTY].gameObject.GetComponent<MeshRenderer>().material = lightMat;
        systemTMP.text = "잠시 후 랜덤한 밟판이 사라집니다!\n5";
        yield return new WaitForSeconds(1);
        systemTMP.text = "잠시 후 랜덤한 밟판이 사라집니다!\n4";
        yield return new WaitForSeconds(1);
        systemTMP.text = "잠시 후 랜덤한 밟판이 사라집니다!\n3";
        yield return new WaitForSeconds(1);
        floorManager.floorList[DelTX][DelTY].gameObject.GetComponent<MeshRenderer>().material = normalMat;
        systemTMP.text = "잠시 후 랜덤한 밟판이 사라집니다!\n2";
        yield return new WaitForSeconds(1);
        systemTMP.text = "잠시 후 랜덤한 밟판이 사라집니다!\n1";
        yield return new WaitForSeconds(1);
        systemTMP.text = "잠시 후 랜덤한 밟판이 사라집니다!\n0";
        yield return new WaitForSeconds(1);
        RandomDel(DelTX, DelTY);
        yield return new WaitForSeconds(3);
        Recover();
        systemTMP.text = "";
        yield return new WaitForSeconds(3);
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
