using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPrefab : MonoBehaviour
{
    public float blockFallCool;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine("BlockFalling");
        }
    }

    IEnumerator BlockFalling()
    {
        yield return new WaitForSeconds(blockFallCool);
        gameObject.AddComponent<Rigidbody>();
        gameObject.GetComponent<Rigidbody>().mass = 10;
        Destroy(gameObject, 0.9f);
    }
}
