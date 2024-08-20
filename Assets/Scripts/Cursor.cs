using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    float m_tClickTime;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GameManager.Get().StartGame();
            m_tClickTime = Time.time;
            if (m_oHeldShape == null)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.transform.parent != null)
                {
                    m_oHeldShape = hit.collider.transform.parent.GetComponent<Shape>();
                    if (m_oHeldShape != null)
                    {
                        m_oHeldShape.Grab();
                    }
                    else if(hit.collider.name == "BtnHome")
                    {
                        SoundManager.Get().SetTense(false);
                        SoundManager.Get().ChangeSong(SoundManager.E_Song.calm);
                        SceneManager.LoadScene("Menu");
                    }
                    else if (hit.collider.name == "BtnRestockCollider")
                    {
                        if (!Tray.Get().IsRefilling())
                        {
                            Tray.Get().Refill(false);
                        }
                    }
                    else if (hit.collider.name == "BtnRestockAllCollider")
                    {
                        if (!Tray.Get().IsRefilling())
                        {
                            Tray.Get().Clear();
                            Tray.Get().Refill();
                        }
                    }
                }
            }
            else
            {
                if (m_oHeldShape != null && m_oHeldShape.Release())
                {
                    m_oHeldShape = null;
                }
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            // click was held
            if(m_tClickTime < Time.time - 0.25f && m_oHeldShape != null)
            {
                if (m_oHeldShape.Release())
                {
                    m_oHeldShape = null;
                }
            }
        }
        //if(Input.GetMouseButtonDown(2))
        //{
        //    if(!Tray.Get().IsFull())
        //    {
        //        new GameObject().AddComponent<Shape>();
        //    }
        //}
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
