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
    bool m_bZooming = false;
    float m_fZoomSpeed = 100;

    void Start()
    {
        cam = Camera.main;
        rTransform = cam.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = rTransform.position;
        if(m_bZooming)
        {
            pos.y += m_fZoomSpeed * Time.deltaTime;
            if(pos.y > Tower.Get().GetHeight()-1)
            {
                pos.y = Tower.Get().GetHeight()-1;
                m_bZooming = false;
            }
            rTransform.position = pos;
            return;
        }
        if (m_bLockMoveDown)
        {
            pos.y -= speed * Time.deltaTime / 2;
            if (pos.y < yLimLower)
            {
                pos.y = yLimLower;
            }
            rTransform.position = pos;
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float delta = speed * Time.deltaTime;
        float extraHeight = 0.5f;
        float extraDepth = 0.5f;
        Shape heldShape = Cursor.Get().GetHeldShape();
        if(heldShape != null)
        {
            extraHeight += heldShape.GetMaxs(true).y + heldShape.GetGrabDelta().y;
            extraDepth += heldShape.GetMins(true).y + heldShape.GetGrabDelta().y;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            JumpToTop();
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

    public void JumpToTop()
    {
        m_bZooming = true;
    }    

    public void OnLose()
    {
        m_fZoomSpeed = speed / 2;
        JumpToTop();
        m_bLockMoveDown = true;
    }
}
