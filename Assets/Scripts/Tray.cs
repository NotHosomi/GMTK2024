using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tray : MonoBehaviour
{
    static Tray instance;
    public static Tray Get() { return instance; }

    int msz_nTrayLimit = 6;
    List<Shape> m_vShapes = new List<Shape>();

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

        for (int i = 0; i < msz_nTrayLimit; ++i)
        {
            m_vShapes.Add(null);
        }
    }

    public void Add(Shape shape)
    {
        // find first empty slot
        int i = 0;
        for (; i < m_vShapes.Count; ++i)
        {
            if(m_vShapes[i] == null) { break; }
        }
        m_vShapes[i] = shape;
        shape.transform.parent = transform;

        Vector3 pos = shape.transform.localPosition;
        Vector2 center = shape.GetCenterDelta(false);
        pos.y = -(center.y/2) + (2f - i) * 1.25f;
        pos.x = -(center.x/2) + (i % 2 - 0.5f) * 1.5f;
        pos.z = -0.5f;
        Debug.Log("Center offset: " + center + "\t pos: " + pos.x + " " + pos.y);
        shape.transform.localPosition = pos;
        shape.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    public void Remove(Shape shape)
    {
        int i = 0;
        for (; i < m_vShapes.Count; ++i)
        {
            if (m_vShapes[i] == shape) { break; }
        }
        if(i == m_vShapes.Count)
        {
            Debug.Log("Could not find shape in tray");
        }
        m_vShapes[i] = null;
        shape.transform.localScale = new Vector3(1, 1, 1);
    }

    public bool IsFull()
    {
        for(int i = 0; i < m_vShapes.Count; ++i)
        {
            if(m_vShapes[i] == null)
            {
                return false;
            }
        }
        return true;
    }

    void Refill()
    {

    }
}
