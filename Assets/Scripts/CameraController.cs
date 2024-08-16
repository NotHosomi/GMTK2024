using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    Transform rTransform;
    [SerializeField] float speed = 10;
    [SerializeField] float yLim = 100;
    [SerializeField] float xLim = 50;

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

        if (pos.x < -xLim)
        {
            pos.x = -xLim;
        }
        if (pos.x > xLim)
        {
            pos.x = xLim;
        }
        if (pos.y < 0)
        {
            pos.y = 0;
        }
        if (pos.y > yLim)
        {
            pos.y = yLim;
        }
        rTransform.position = pos;
    }
}
