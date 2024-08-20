using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    bool m_bWait = true;
    bool m_bIsFreeplay = false;
    bool m_bLost = false;

    [SerializeField] GameObject scoreDisplay;
    [SerializeField] GameObject dayDisplay;
    [SerializeField] GameObject dayProgressBar;
    [SerializeField] GameObject lossBanner;
    [SerializeField] GameObject skyline;
    [SerializeField] GameObject RestockButton;
    [SerializeField] GameObject m_oHowToPlay;

    [SerializeField] Gradient tDaySky;
    [SerializeField] Gradient tNightSky;
    static GameManager instance;
    public static GameManager Get() { return instance; }

    float m_fDayLengthSecs;
    float m_fNightLengthSecs;
    float m_fPhaseTime = 0;
    float m_fTimeScale = 1;

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
        m_bIsFreeplay = SessionManager.Get().IsFreeplay();
        if (m_bIsFreeplay)
        {
            m_fDayLengthSecs = 30;
            m_fNightLengthSecs = 30;
            RestockButton.SetActive(true);
        }
        else
        {
            m_fDayLengthSecs = 30;
            m_fNightLengthSecs = 5;
        }

        Tower.Init();
        SoundManager.Get().SetTense(false);
        if(m_bIsFreeplay)
        {
            dayProgressBar.transform.parent.gameObject.SetActive(false);
            StartGame();
        }
    }

    private void Start()
    {
        Tray.Get().Refill();
    }

    void Update()
    {
        if(m_bWait) { return; }
        m_fPhaseTime += Time.deltaTime * m_fTimeScale;
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
        float progress = m_fPhaseTime / m_fDayLengthSecs;
        Camera.main.backgroundColor = tDaySky.Evaluate(progress);
        dayProgressBar.transform.localScale = new Vector3(progress, 1, 1);
        if (m_fPhaseTime > m_fDayLengthSecs)
        {
            BecomeNight();
        }
    }

    void Night()
    {
        float progress = m_fPhaseTime / m_fNightLengthSecs;
        Camera.main.backgroundColor = tNightSky.Evaluate(progress);
        if (m_fPhaseTime > m_fNightLengthSecs)
        {
            BecomeDay();
        }
    }

    void BecomeDay()
    {
        m_fPhaseTime = 0;
        m_ePhase = E_Phase.day;

        dayDisplay.GetComponent<TextMeshPro>().text = (m_nCycles + 1).ToString();
        skyline.transform.GetChild(0).gameObject.SetActive(false);
        Tower.Get().OnDayTime();
        SoundManager.Get().OnDay();

        if (Tray.Get().IsEmpty() && Cursor.Get().GetHeldShape() == null)
        {
            SetSpeedup(true);
        }
        if (!m_bLost)
        {
            ++m_nCycles;
        }
    }

    void BecomeNight()
    {
        dayProgressBar.transform.localScale = new Vector3(0, 1, 1);
        m_fPhaseTime = 0;
        m_ePhase = E_Phase.night;
        SetSpeedup(false);
        skyline.transform.GetChild(0).gameObject.SetActive(true);
        Tower.Get().OnNightTime();
        SoundManager.Get().OnNight();

        if (!m_bIsFreeplay)
        {
            // hasten the clock
            m_fDayLengthSecs -= 2;
            if(m_fDayLengthSecs < 5)
            {
                m_fDayLengthSecs = 5;
                SoundManager.Get().SetTense(true);
            }

            // handle the tray
            Shape shape = Cursor.Get().GetHeldShape();
            Cursor.Get().ForgetHeldShape();
            HighlightManager.Get().HidePreview();
            HighlightManager.Get().HideValidity();
            if (shape != null)
            {
                shape.ReturnToTray();
            }
            int nSlots = Tray.Get().Refill();
            if (nSlots == 0)
            {
                Loss();
            }
            else
            {
                m_fNightLengthSecs = nSlots;
            }
        }
    }

    public bool IsNight()
    {
        return m_ePhase == E_Phase.night;
    }

    public bool SkipDay()
    {
        if(m_ePhase == E_Phase.night)
        {
            return false;
        }
        m_fPhaseTime = m_fDayLengthSecs;
        return true;
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

    public bool IsFreeplay()
    {
        return m_bIsFreeplay;
    }

    public void Loss()
    {
        if(m_bLost)
        {
            return;
        }
        m_bLost = true;
        m_fNightLengthSecs = 15;
        m_fDayLengthSecs = 15;
        lossBanner.SetActive(true);
        StartCoroutine(LowerLossBanner());
        GetComponent<Crane>().SetLocked(true);
        SoundManager.Get().PlaySound(SoundManager.E_Sfx.loss);
        SoundManager.Get().SetTense(false);
        GetComponent<CameraController>().OnLose();
    }
    IEnumerator LowerLossBanner()
    {
        Vector3 pos = lossBanner.transform.localPosition;
        while(pos.y > 3.5)
        {
            pos.y -= Time.deltaTime;
            lossBanner.transform.localPosition = pos;
            yield return new WaitForEndOfFrame();
        }
    }
    public void SetSpeedup(bool fast)
    {
        if(m_bLost)
        {
            m_fTimeScale = 1;
            return;
        }
        m_fTimeScale = fast ? 4 : 1;
    }

    public void StartGame()
    {
        if(m_bWait)
        {
            m_oHowToPlay.SetActive(false);
            m_bWait = false;
        }
    }
}
