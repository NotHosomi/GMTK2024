using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject scoreDisplay;
    [SerializeField] GameObject dayDisplay;

    [SerializeField] Gradient tDaySky;
    [SerializeField] Gradient tNightSky;
    static GameManager instance;
    public static GameManager Get() { return instance; }

    [SerializeField] float m_fDayLengthSecs;
    [SerializeField] float m_fNightLengthSecs;
    [SerializeField] float m_fPhaseTime = 0;

    int m_nScore = 0;
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

    private void Start()
    {
        Tray.Get().Refill();
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
            BecomeNight();
        }
    }

    void Night()
    {
        Camera.main.backgroundColor = tNightSky.Evaluate(m_fPhaseTime / m_fNightLengthSecs);
        if (m_fPhaseTime > m_fNightLengthSecs)
        {
            BecomeDay();
        }
    }

    void BecomeDay()
    {
        m_fPhaseTime = 0;
        m_ePhase = E_Phase.day;
        Tower.Get().OnDayTime();
        ++m_nCycles;
        dayDisplay.GetComponent<TextMeshPro>().text = (m_nCycles+1).ToString();
    }

    void BecomeNight()
    {
        m_fPhaseTime = 0;
        m_ePhase = E_Phase.night;
        Tower.Get().OnNightTime();

        Shape shape = Cursor.Get().GetHeldShape();
        Cursor.Get().ForgetHeldShape();
        HighlightManager.Get().HidePreview();
        HighlightManager.Get().HideValidity();
        if (shape != null)
        {
            shape.ReturnToTray();
        }
        Tray.Get().Refill();
    }

    public bool IsNight()
    {
        return m_ePhase == E_Phase.night;
    }

    public void SkipDay()
    {
        if(m_ePhase == E_Phase.night)
        {
            return;
        }
        m_fPhaseTime = m_fDayLengthSecs - 2;
    }

    public void AddScore(int amount, Vector2Int coord)
    {
        if (amount != 0)
        {
            GameObject number = Instantiate(Resources.Load("Prefabs/ScoreNumber") as GameObject);
            number.GetComponent<ScoreNumber>().init(amount);
            number.transform.position = new Vector3(coord.x, coord.y, -9.5f);
            AddScore(amount);
        }
    }
    public void AddScore(int amount)
    {
        m_nScore += amount;
        scoreDisplay.GetComponent<TextMeshPro>().text = "<b>" + m_nScore.ToString() + "</b>";
    }
}
