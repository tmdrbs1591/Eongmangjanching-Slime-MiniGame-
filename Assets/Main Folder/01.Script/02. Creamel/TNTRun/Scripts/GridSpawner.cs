using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEditor.Experimental.GraphView.GraphView;
using TMPro;
public class GridSpawner : MonoBehaviour
{
    [Serializable]
    public struct Floor
    {
        public float yOffset;

        public Vector2 grid;

        public Vector3 CameraPosition;
        public float CameraFov;

        public GameObject gridObj;

        public Vector3 blockSize;

        public Floor(float yOffset, Vector2 grid, Vector3 CameraPosition, float CameraFov, GameObject gridObj, Vector3 blockSize)
        {
            this.yOffset = yOffset;
            this.grid = grid;
            this.CameraFov = CameraFov;
            this.CameraPosition = CameraPosition;
            this.gridObj = gridObj;
            this.blockSize = blockSize;
        }
    }

    public Floor[] floors;
    public GameObject player;
    public TextMeshProUGUI systemTmp;
    public GameObject StartFloor;

    private void Start()
    {
        StartCoroutine("StartTimer");
        for(int i = 0; i < floors.Length; i++)
        {
            SpawnFloor(i);
        }
    }

    private void Update()
    {
    }

    public void SpawnFloor(int index)
    {
        for (int i = 0; i < floors[index].grid.x; i++)
        {
            for (int j = 0; j < floors[index].grid.y; j++)
            {
                GameObject tempBlock = Instantiate(floors[index].gridObj, new Vector3(j * floors[index].blockSize.x, floors[index].yOffset, i * floors[index].blockSize.z), transform.rotation);
                tempBlock.transform.localScale = floors[index].blockSize;
            }
        }
    }

    IEnumerator StartTimer()
    {
        systemTmp.text = "잠시후 게임이 시작합니다\n3";
        yield return new WaitForSeconds(1);
        systemTmp.text = "잠시후 게임이 시작합니다\n2";
        yield return new WaitForSeconds(1);
        systemTmp.text = "잠시후 게임이 시작합니다\n1";
        yield return new WaitForSeconds(1);
        systemTmp.text = "잠시후 게임이 시작합니다\nStart!";
        StartFloor.SetActive(false);
    }
}
