using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Gradient tDaySky;
    [SerializeField] Gradient tNightSky;
    static GameManager instance;
    static GameManager Get() { return instance; }

    [SerializeField] float m_fDayLengthSecs;
    [SerializeField] float m_fNightLengthSecs;
    [SerializeField] float m_fPhaseTime = 0;
    int m_nCycles = 0;

    enum E_Phase
    {
        day,
        night
    };
    E_Phase m_ePhase = E_Phase.day;


    void Awake()
    {
        if(instance!=null)
        {
            Debug.LogError("Duplicate GameManager!!!");
        }
        else
        {
            instance = this;
        }
        Tower.Init();
    }

    

    void Update()
    {
        m_fPhaseTime += Time.deltaTime;
        switch(m_ePhase)
        {
            case E_Phase.day: Day();
                break;
            case E_Phase.night: Night();
                break;
        }
    }

    void Day()
    {
        Camera.main.backgroundColor = tDaySky.Evaluate(m_fPhaseTime / m_fDayLengthSecs);
        if (m_fPhaseTime > m_fDayLengthSecs)
        {
            // become night
            m_fPhaseTime = 0;
            m_ePhase = E_Phase.night;
            Tower.Get().OnNightTime();
        }
    }

    void Night()
    {
        Camera.main.backgroundColor = tNightSky.Evaluate(m_fPhaseTime / m_fNightLengthSecs);
        if (m_fPhaseTime > m_fNightLengthSecs)
        {
            // become day
            m_fPhaseTime = 0;
            m_ePhase = E_Phase.day;
            Tower.Get().OnDayTime();
            m_nCycles++;
        }
    }

    IEnumerator SpawnShapes(int n)
    {
        for (int i = 0; i < n; ++i)
        {

            yield return new WaitForSeconds(0.5f);
        }
    }
}
