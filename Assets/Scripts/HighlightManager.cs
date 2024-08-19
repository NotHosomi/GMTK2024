using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    static HighlightManager instance;
    public static HighlightManager Get() { return instance; }
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Duplicate HighlightManager!!!");
        }
        else
        {
            instance = this;
        }
    }

    List<GameObject> m_vValidHighlights = new List<GameObject>();
    List<GameObject> m_vInvalidHighlights = new List<GameObject>();
    List<GameObject> m_vIndicators = new List<GameObject>();
    float m_fTime;
    const float mz_fTimeLimit = 1.5f;

    public void Start()
    {
        GameObject group = new GameObject();
        for(int i = 0; i < 10; ++i)
        {
            m_vValidHighlights.Add(Instantiate(Resources.Load("Prefabs/ValidHighlight") as GameObject, group.transform));
            m_vInvalidHighlights.Add(Instantiate(Resources.Load("Prefabs/InvalidHighlight") as GameObject, group.transform));
            m_vIndicators.Add(Instantiate(Resources.Load("Prefabs/Indicator") as GameObject, group.transform));
        }
        HideValidity();
        HidePreview();
    }

    public void DrawValidity(List<Vector2Int> m_vValid, List<Vector2Int> m_vInvalid)
    {
        m_fTime = mz_fTimeLimit;
        while (m_vValid.Count >= m_vValidHighlights.Count)
        {
            m_vValidHighlights.Add(GameObject.Instantiate(Resources.Load("Prefabs/ValidHighlight") as GameObject));
        }
        while (m_vInvalid.Count > m_vInvalidHighlights.Count)
        {
            m_vValidHighlights.Add(GameObject.Instantiate(Resources.Load("Prefabs/InvalidHighlight") as GameObject));
        }
        int i = 0;
        for (; i < m_vValid.Count; ++i)
        {
            m_vValidHighlights[i].transform.position = new Vector3(m_vValid[i].x, m_vValid[i].y, -7);
            m_vValidHighlights[i].SetActive(true);
        }
        for(; i < m_vValidHighlights.Count; ++i)
        {
            m_vValidHighlights[i].SetActive(false);
        }

        for(i = 0; i < m_vInvalid.Count; ++i)
        {
            m_vInvalidHighlights[i].transform.position = new Vector3(m_vInvalid[i].x, m_vInvalid[i].y, -7);
            m_vInvalidHighlights[i].SetActive(true);
        }
        for (; i < m_vInvalidHighlights.Count; ++i)
        {
            m_vInvalidHighlights[i].SetActive(false);
        }
    }

    public void DrawPreview(Vector2Int origin, List<Vector2Int> blocks)
    {
        while (blocks.Count > m_vIndicators.Count)
        {
            m_vIndicators.Add(GameObject.Instantiate(Resources.Load("Sprites/Indicator") as GameObject));
        }
        int i = 0;
        for (; i < blocks.Count; ++i)
        {
            m_vIndicators[i].transform.position = new Vector3(blocks[i].x + origin.x, blocks[i].y + origin.y, -6);
            m_vIndicators[i].SetActive(true);
        }
        for (; i < m_vIndicators.Count; ++i)
        {
            m_vIndicators[i].SetActive(false);
        }
    }

    public void HideValidity()
    {
        foreach (GameObject go in m_vValidHighlights)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in m_vInvalidHighlights)
        {
            go.SetActive(false);
        }
    }

    public void HidePreview()
    {
        foreach (GameObject go in m_vIndicators)
        {
            go.SetActive(false);
        }
    }

    private void Update()
    {
        if(m_fTime > 0)
        {
            m_fTime -= Time.deltaTime;
            if(m_fTime < 0)
            {
                m_fTime = 0;
                HideValidity();
            }
        }
    }
}
