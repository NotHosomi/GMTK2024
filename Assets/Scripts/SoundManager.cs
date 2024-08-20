using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    static SoundManager instance;
    public static SoundManager Get() { return instance; }
    [SerializeField] GameObject m_oCanvas;
    [SerializeField] GameObject m_oEventManager;

    public enum E_Sfx
    {
        pop,
        click,
        womp,
        loss
    }
    [SerializeField] AudioClip[] m_aPop;
    [SerializeField] AudioClip[] m_aClick;
    [SerializeField] AudioClip m_oWomp;
    [SerializeField] AudioClip m_oLoss;
    AudioSource m_oSfx;
    public enum E_Song
    {
        calm,
        normal,
        tense
    }
    E_Song m_eCurrentSong;
    [SerializeField] AudioSource m_oCalm;
    [SerializeField] float m_fCalmVolume = 1;
    [SerializeField] AudioSource m_oNormal;
    [SerializeField] float m_fNormalVolume = 0;
    [SerializeField] AudioSource m_oTense;
    [SerializeField] float m_fTenseVolume = 0;

    Slider m_oSliderSFX;
    Slider m_oSliderMusic;
    Transform m_oSliderCoverSFX;
    Transform m_oSliderCoverMusic;

    float m_fMusicVolume = 0.5f;
    bool m_bIsTense = false;

    void Awake()
    {

        m_oSliderSFX = GameObject.Find("SFX Slider").GetComponent<Slider>();
        m_oSliderCoverSFX = GameObject.Find("SFXSliderHider").transform;
        m_oSliderMusic = GameObject.Find("Music Slider").GetComponent<Slider>();
        m_oSliderCoverMusic = GameObject.Find("MusicSliderHider").transform;

        if (instance != null)
        {
            Debug.LogError("Duplicate SoundManager!!!");
        }
        else
        {
            instance = this;
        }
        m_oSfx = GetComponent<AudioSource>();
        SetSongVolume(E_Song.calm, 1);
        SetSongVolume(E_Song.normal, 0);
        SetSongVolume(E_Song.tense, 0);
    }

    private void Update()
    {
        m_fMusicVolume = m_oSliderMusic.value;
        m_oSliderCoverMusic.localScale = new Vector3(Mathf.RoundToInt((1 - m_fMusicVolume) * 40) / 40.0f, 1, 1);
        UpdateMusicVolume();
        m_oSfx.volume = m_oSliderSFX.value;
        m_oSliderCoverSFX.localScale = new Vector3(Mathf.RoundToInt((1 - m_oSliderSFX.value) * 40) / 40.0f, 1, 1);
    }
    float GetMusicVolume() { return m_fMusicVolume; }
    float GetSfxVolume() { return m_oSfx.volume; }
    void SetMusicVolume(float val) { m_fMusicVolume = val; }
    void SetSfxVolume(float val) { m_oSfx.volume = val; }

    public void SetTense(bool val)
    {
        m_bIsTense = val;
    }

    public void OnNight()
    {
        if(!m_bIsTense)
        {
            ChangeSong(E_Song.calm);
        }
    }
    public void OnDay()
    {
        if (m_bIsTense)
        {
            // switch to day music
            ChangeSong(E_Song.tense);
        }
        else
        {
            // switch to day music
            ChangeSong(E_Song.normal);
        }
    }
    public void OnLoss()
    {
        ChangeSong(E_Song.calm);
    }

    public void ChangeSong(E_Song to)
    {
        m_eCurrentSong = to;
        StartCoroutine(crossfade(to, 1));
    }
    IEnumerator crossfade(E_Song to, float duration)
    {
        Debug.Log("Crossfade " + to);
        float progress = 0;
        while (progress < 1)
        {
            float delta = (Time.deltaTime / duration);
            progress += delta;
            AdjustSongVolume(to, delta);

            if (to != E_Song.calm) { AdjustSongVolume(E_Song.calm, -delta); }
            if (to != E_Song.normal) { AdjustSongVolume(E_Song.normal, -delta); }
            if (to != E_Song.tense) { AdjustSongVolume(E_Song.tense, -delta); }

            yield return new WaitForEndOfFrame();
        }
        Debug.Log("New sound mix: " + m_fCalmVolume + " " + m_fNormalVolume + " " + m_fTenseVolume);
    }
    void SetSongVolume(E_Song song, float volume)
    {
        AudioSource src = m_oCalm;
        switch (song)
        {
            case E_Song.calm:
                src = m_oCalm;
                m_fCalmVolume = volume;
                break;
            case E_Song.normal:
                src = m_oNormal;
                m_fNormalVolume = volume;
                break;
            case E_Song.tense:
                src = m_oTense;
                m_fTenseVolume = volume;
                break;
        }
        src.volume = volume * m_fMusicVolume;
    }
    void AdjustSongVolume(E_Song song, float nudge)
    {
        AudioSource src = m_oCalm;
        float volume = 0;
        switch (song)
        {
            case E_Song.calm:
                src = m_oCalm;
                m_fCalmVolume += nudge;
                if (m_fCalmVolume < 0) { m_fCalmVolume = 0; }
                else if (m_fCalmVolume > 1) { m_fCalmVolume = 1; }
                volume = m_fCalmVolume;
                break;
            case E_Song.normal:
                src = m_oNormal;
                m_fNormalVolume += nudge;
                if (m_fNormalVolume < 0) { m_fNormalVolume = 0; }
                else if (m_fNormalVolume > 1) { m_fNormalVolume = 1; }
                volume = m_fNormalVolume;
                break;
            case E_Song.tense:
                src = m_oTense;
                m_fTenseVolume += nudge;
                if (m_fTenseVolume < 0) { m_fTenseVolume = 0; }
                else if (m_fTenseVolume > 1) { m_fTenseVolume = 1; }
                volume = m_fTenseVolume;
                break;
        }
        src.volume = volume * m_fMusicVolume;
    }

    void UpdateMusicVolume()
    {
        m_oCalm.volume = m_fCalmVolume * m_fMusicVolume;
        m_oNormal.volume = m_fNormalVolume * m_fMusicVolume;
        m_oTense.volume = m_fTenseVolume * m_fMusicVolume;
    }

    public void PlaySound(E_Sfx sound)
    {
        switch (sound)
        {
        case E_Sfx.click: m_oSfx.PlayOneShot(m_aClick[Random.Range(0, m_aClick.Length)]);
            break;
        case E_Sfx.pop: m_oSfx.PlayOneShot(m_aPop[Random.Range(0, m_aPop.Length)]);
            break;
        case E_Sfx.womp: m_oSfx.PlayOneShot(m_oWomp);
            break;
        case E_Sfx.loss: m_oSfx.PlayOneShot(m_oLoss);
            break;
        }
    }
}
