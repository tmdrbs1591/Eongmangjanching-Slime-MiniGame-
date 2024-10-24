using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public Transform testPlayerTransform;
    public Rigidbody testPlayerRB;
    private float speed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        testPlayerTransform = GetComponent<Transform>();
        testPlayerRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        // -1 ~ 1

        Vector3 velocity = new Vector3(inputX, 0, inputZ);
        velocity *= speed;
        testPlayerRB.velocity = velocity;
    }
}
