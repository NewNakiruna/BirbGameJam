using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMover : MonoBehaviour
{
    public float speed = 2.0f;
    public float interval = 5.0f;
    private Vector3 startPos;
    private Quaternion startRot;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        InvokeRepeating("Restart", interval, interval);

    }
    
    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }

    void Restart()
    {
        transform.SetPositionAndRotation(startPos, startRot);
    }
}