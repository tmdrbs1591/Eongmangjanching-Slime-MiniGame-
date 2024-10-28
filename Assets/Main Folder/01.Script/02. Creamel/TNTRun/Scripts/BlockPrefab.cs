using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPrefab : MonoBehaviour
{
    public float blockFallCool;
    public Material lightMat;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine("BlockFalling");
        }
    }

    IEnumerator BlockFalling()
    {
        gameObject.GetComponent<MeshRenderer>().material = lightMat;
        yield return new WaitForSeconds(blockFallCool);
        gameObject.AddComponent<Rigidbody>();
        gameObject.GetComponent<Rigidbody>().mass = 10;
        Destroy(gameObject, 0.9f);
    }
}
