using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    Transform rTransform;
    float speed = 20;
    float yLimLower = 3;
    bool m_bLockMoveDown = false;

    void Start()
    {
        cam = Camera.main;
        rTransform = cam.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = rTransform.position;
        if (m_bLockMoveDown)
        {
            pos.y -= speed * Time.deltaTime * 2;
            if (pos.y < yLimLower)
            {
                pos.y = yLimLower;
            }
            transform.position = pos;
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float delta = speed * Time.deltaTime;
        float extraHeight = 0.5f;
        float extraDepth = 0.5f;
        Shape heldShape = Cursor.Get().GetHeldShape();
        if(heldShape != null)
        {
            extraHeight += heldShape.GetMaxs(true).y;
            extraDepth += heldShape.GetMins(true).y;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            pos.y = Tower.Get().GetHeight() + 4;
            rTransform.position = pos;
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            delta *= 2;
        }

        // move up
        if (Input.GetKey(KeyCode.W))
        {
            pos.y += delta;
        }
        else if(mousePos.y + extraHeight > Camera.main.transform.position.y + 4f)
        {
            pos.y += delta / 2;
        }

        // move down
        if (Input.GetKey(KeyCode.S))
        {
            pos.y -= delta;
        }
        else if (mousePos.y + extraDepth < Camera.main.transform.position.y - 4f)
        {
            pos.y -= delta / 2;
        }

        if (pos.y > Tower.Get().GetHeight() + 3)
        {
            pos.y = Tower.Get().GetHeight() + 3;
        }
        if (pos.y < yLimLower)
        {
            pos.y = yLimLower;
        }
        rTransform.position = pos;
    }

    public void OnLose()
    {

    }
}
