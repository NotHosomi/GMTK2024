using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager
{
    List<GameObject> m_vValidHighlights;
    List<GameObject> m_vInvalidHighlights;

    HighlightManager()
    {
        for(int i = 0; i < 10; ++i)
        {
            m_vValidHighlights.Add(GameObject.Instantiate(Resources.Load("Sprites/ValidHighlight") as GameObject));
            m_vValidHighlights.Add(GameObject.Instantiate(Resources.Load("Sprites/InvalidHighlight") as GameObject));
        }
    }

    void DisplayHighlight(List<Vector2Int> m_vValid, List<Vector2Int> m_vInvalid)
    {
        while(m_vValid.Count > m_vValidHighlights.Count)
        {
            m_vValidHighlights.Add(GameObject.Instantiate(Resources.Load("Sprites/ValidHighlight") as GameObject));
        }
        while (m_vInvalid.Count > m_vInvalidHighlights.Count)
        {
            m_vValidHighlights.Add(GameObject.Instantiate(Resources.Load("Sprites/InvalidHighlight") as GameObject));
        }
        int i = 0;
        for (; i < m_vValid.Count; ++i)
        {
            m_vValidHighlights[i].transform.position = new Vector3(m_vValid[i].x, m_vValid[i].y, -6);
            m_vValidHighlights[i].SetActive(true);
        }
        for(; i < m_vValidHighlights.Count; ++i)
        {
            m_vValidHighlights[i].SetActive(false);
        }

        for(i = 0; i < m_vValid.Count + m_vInvalid.Count; ++i)
        {
            m_vInvalidHighlights[i].transform.position = new Vector3(m_vInvalid[i].x, m_vInvalid[i].y, -6);
            m_vInvalidHighlights[i].SetActive(true);
        }
        for (; i < m_vValidHighlights.Count; ++i)
        {
            m_vInvalidHighlights[i].SetActive(false);
        }
    }
}
