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
                if (hit.collider != null && hit.collider.transform.parent != null)
                {
                    m_oHeldShape = hit.collider.transform.parent.GetComponent<Shape>();
                    if (m_oHeldShape != null)
                    {
                        m_oHeldShape.Grab();
                    }
                    if(hit.collider.name == "SkipButton")
                    {
                        hit.collider.gameObject.SetActive(false);
                        GameManager.Get().SkipDay();
                    }
                }
            }
            else
            {
                if (m_oHeldShape.Release())
                {
                    m_oHeldShape = null;
                }
            }
        }
        if(Input.GetMouseButtonDown(2))
        {
            if(!Tray.Get().IsFull())
            {
                new GameObject().AddComponent<Shape>();
            }
        }
    }

    public Shape GetHeldShape()
    {
        return m_oHeldShape;
    }
    public void ForgetHeldShape()
    {
        m_oHeldShape = null;
    }
}
