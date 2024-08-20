using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager
{
    static SessionManager instance;
    public static SessionManager Get() { return instance;}
    public static void Init() { instance = new SessionManager(); }


    bool m_bIsFreeplay = false;
    public bool IsFreeplay() { return m_bIsFreeplay; }
    public void SetFreeplay(bool val) { m_bIsFreeplay = val; }
}
