using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Target : MonoBehaviour
{
    public int Point;
    public int hitCount;
    public int hitMax;

    public TextMeshPro left;

    public float moveSpeed;
    public float moveDistance;

    private Vector3 startPosition;


    private void Start()
    {
        if (hitMax == 1) left.text = "";
        else if (hitMax != 1) left.text = hitMax.ToString();
        startPosition = transform.position;
    }

    private void Update()
    {
        float x = Mathf.PingPong(Time.time * moveSpeed, moveDistance) - (moveDistance / 2);
        transform.position = startPosition + new Vector3(x, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Arrow"))
        {
            hitCount++;
            if(hitMax != 1)left.text = (hitMax - hitCount).ToString();
            if (hitMax <= hitCount)
            {
                GameObject player = GameObject.FindWithTag("Player");
                var playerscore = player.GetComponent<PlayerScore>();

                playerscore.AddScore(Point * 100);

                Debug.Log(other.gameObject.GetComponent<ArrowInit>().playerName + "님이 점수 " + Point + "점을 얻었습니다!");
                Destroy(gameObject);
            }
            Destroy(other.gameObject);
        }
    }
}
