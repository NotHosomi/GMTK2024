using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    Transform rTransform;
    float speed = 10;
    float yLimUpper = 100;
    float yLimLower = 3;
    float xLimUpper = 11;
    float xLimLower = 0;

    void Start()
    {
        cam = Camera.main;
        rTransform = cam.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = rTransform.position;
        float delta = speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
        {
            pos.y += delta;
        }
        if (Input.GetKey(KeyCode.S))
        {
            pos.y -= delta;
        }
        if (Input.GetKey(KeyCode.A))
        {
            pos.x -= delta;
        }
        if (Input.GetKey(KeyCode.D))
        {
            pos.x += delta;
        }

        if (pos.x < xLimLower)
        {
            pos.x = xLimLower;
        }
        if (pos.x > xLimUpper)
        {
            pos.x = xLimUpper;
        }
        if (pos.y < yLimLower)
        {
            pos.y = yLimLower;
        }
        if (pos.y > yLimUpper)
        {
            pos.y = yLimUpper;
        }
        rTransform.position = pos;
    }
}
