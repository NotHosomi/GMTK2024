using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Awake()
    {
        if(SoundManager.Get() == null)
        {
            GameObject go = Instantiate(Resources.Load("Core/LiveForever") as GameObject);
            DontDestroyOnLoad(go);
        }
    }
    enum E_MenuItems
    {
        challenge,
        freeplay,
        credits
    }

    [SerializeField] GameObject[] m_aMenuEntries;
    [SerializeField] GameObject m_oCredits;
    GameObject m_oLastHover = null;
    bool m_bShowCredits = false;

    // Start is called before the first frame update
    void Start()
    {
        SessionManager.Init();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if(hit.collider == null && m_oLastHover != null)
        {
            m_oLastHover.transform.localScale = new Vector3(1, 1, 1);
            m_oLastHover = null;
            return;
        }
        if(hit.collider == null) { return; }
        GameObject hover = hit.collider.gameObject;
        if(hover == null) { return; }
        hover.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        m_oLastHover = hover;
        foreach (GameObject go in m_aMenuEntries)
        {
            if(hover != go)
            {
                go.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(hover == m_aMenuEntries[(int)E_MenuItems.challenge])
            {
                SessionManager.Get().SetFreeplay(false);
                SceneManager.LoadScene("GameScene");
            }
            else if (hover == m_aMenuEntries[(int)E_MenuItems.freeplay])
            {
                SessionManager.Get().SetFreeplay(true);
                SceneManager.LoadScene("GameScene");
            }
            else if (hover == m_aMenuEntries[(int)E_MenuItems.credits])
            {
                m_bShowCredits = !m_bShowCredits;
                m_oCredits.SetActive(m_bShowCredits);
            }
            else if (hover.gameObject.name == "BtnExit")
            {
#if UNITY_STANDALONE_WIN
                Application.Quit();
#endif
            }
        }
    }
}
