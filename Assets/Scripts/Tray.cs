using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tray : MonoBehaviour
{
    static Tray instance;
    public static Tray Get() { return instance; }

    List<Shape> m_vShapes = new List<Shape>();
    [SerializeField] GameObject mz_oLockedMarker;
    [SerializeField] GameObject mz_oSkipButton;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Duplicate Tray!!!");
        }
        else
        {
            instance = this;
        }

        for (int i = 0; i < 4; ++i)
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
        if(i == m_vShapes.Count)
        {
            Debug.Log("Tried to add to full tray");
            return;
        }
        m_vShapes[i] = shape;
        shape.transform.parent = transform;

        Vector3 pos = shape.transform.localPosition;
        Vector2 center = shape.GetCenterDelta(false);
        Vector2 slotPos = SlotPos(i);
        pos.y = -(center.y / 2) + slotPos.y;
        pos.x = -(center.x / 2) + slotPos.x;
        pos.z = -0.5f;
        shape.transform.localPosition = pos;
        shape.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        mz_oSkipButton.SetActive(false);
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
            return;
        }
        m_vShapes[i] = null;
        shape.transform.localScale = new Vector3(1, 1, 1);
    }

    public bool IsFull()
    {
        for (int i = 0; i < m_vShapes.Count; ++i)
        {
            if (m_vShapes[i] == null)
            {
                return false;
            }
        }
        return true;
    }
    public bool IsEmpty()
    {
        for (int i = 0; i < m_vShapes.Count; ++i)
        {
            if (m_vShapes[i] != null && !m_vShapes[i].IsLocked())
            {
                return false;
            }
        }
        return true;
    }

    // returns the number of slots refilled
    public int Refill()
    {
        int nSlotsToFill = 0;
        bool bAnyLocked = false;
        for (int i = 0; i < m_vShapes.Count; ++i)
        {
            if (m_vShapes[i] == null)
            {
                ++nSlotsToFill;
            }
            else
            {
                if(m_vShapes[i].Lock())
                {
                    GameObject lockMarker = Instantiate(mz_oLockedMarker, transform);
                    Vector2 pos = SlotPos(i);
                    lockMarker.transform.localPosition = new Vector3(pos.x, pos.y, -1);
                    lockMarker.transform.localScale = new Vector3(2, 2, 2);
                    bAnyLocked = true;
                }
            }
        }
        if(bAnyLocked) { SoundManager.Get().PlaySound(SoundManager.E_Sfx.womp); }
        if (nSlotsToFill > 0) { StartCoroutine(SpawnShapes(nSlotsToFill)); }
        return nSlotsToFill;
    }

    public IEnumerator SpawnShapes(int n)
    {
        //Debug.Log("Started coroutine SpawnPieces(" + n + ")");
        for (int i = 0; i < n; ++i)
        {
            yield return new WaitForSeconds(0.5f);
            new GameObject().AddComponent<Shape>();
            SoundManager.Get().PlaySound(SoundManager.E_Sfx.pop);
        }
    }

    Vector2 SlotPos(int i)
    {
        return new Vector2(
            (i % 2 - 0.5f) * 1.2f,
            2.25f - i * 1.5f);
    }
}
