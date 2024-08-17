using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    static Cursor instance;
    public static Cursor Get() { return instance; }
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Duplicate Cursor!!!");
        }
        else
        {
            instance = this;
        }
    }

    Shape m_oHeldShape;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(m_oHeldShape == null)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if(hit.collider != null)
                {
                    m_oHeldShape = hit.collider.transform.parent.GetComponent<Shape>();
                    if (m_oHeldShape != null)
                    {
                        m_oHeldShape.Grab();
                    }
                }
            }
            else
            {
                m_oHeldShape.Release();
                m_oHeldShape = null;
            }
        }
        if(Input.GetMouseButtonDown(2))
        {
            GameObject obj = new GameObject();
            Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tMousePos.z = 0;
            obj.transform.position = tMousePos;
            obj.AddComponent<Shape>();
        }
    }
}
