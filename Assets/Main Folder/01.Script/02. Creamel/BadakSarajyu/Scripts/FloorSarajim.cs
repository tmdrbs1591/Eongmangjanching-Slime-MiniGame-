using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEditor.Experimental.GraphView.GraphView;
using TMPro;
public class FloorSarajim : MonoBehaviour
{
    [Serializable]
    public struct FloorS
    {
        public float yOffset;
        public Vector2 grid;
        public GameObject gridObj;
        public Vector3 blockSize;
        public Vector3 CameraPosition;
        public float CameraFov;

        public FloorS(float yOffset, Vector2 grid, GameObject gridObj, Vector3 blockSize, Vector3 CameraPosition, float CameraFov)
        {
            this.yOffset = yOffset;
            this.grid = grid;
            this.gridObj = gridObj;
            this.blockSize = blockSize;
            this.CameraPosition = CameraPosition;
            this.CameraFov = CameraFov;
        }
    }

    public FloorS[] floors;
    public GameObject[][] floorList;

    private void Start()
    {
        for (int i = 0; i < floors.Length; i++)
        {
            SpawnFloor(i);
        }
    }

    public void SpawnFloor(int index)
    {
        floorList = new GameObject[(int)floors[index].grid.x][];

        for(int i = 0; i < floors[index].grid.x; i++)
        {
            floorList[i] = new GameObject[(int)floors[index].grid.y];
        }

        for (int i = 0; i < floors[index].grid.x; i++)
        {
            for (int j = 0; j < floors[index].grid.y; j++)
            {
                GameObject tempBlock = Instantiate(floors[index].gridObj, new Vector3(j * floors[index].blockSize.x, floors[index].yOffset, i * floors[index].blockSize.z), transform.rotation);
                tempBlock.transform.localScale = floors[index].blockSize;
                floorList[i][j] = tempBlock;
            }
        }
    }
}
