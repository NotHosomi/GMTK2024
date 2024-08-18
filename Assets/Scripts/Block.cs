using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    static bool ms_bResourcesLoaded = false;
    static Sprite[] ms_aSpritesWall;
    static Sprite[] ms_aSpritesTop;
    static Sprite[] ms_aSpritesSide;
    static Sprite[] ms_aSpritesBottom;
    static void LoadSprites()
    {
        ms_aSpritesWall = Resources.LoadAll<Sprite>("Sprites/wall");
        ms_aSpritesTop = Resources.LoadAll<Sprite>("Sprites/top");
        ms_aSpritesSide = Resources.LoadAll<Sprite>("Sprites/side");
        ms_aSpritesBottom = Resources.LoadAll<Sprite>("Sprites/bottom");
    }

    GameObject m_oInterior;
    Vector2Int m_tOffset;
    bool m_bHasOpenRoof;
    bool m_bHasOpenBase;

    public static Block NewBlock(Transform tParent, Vector2Int tOffset, bool outline = true)
    {
        if(!ms_bResourcesLoaded) { LoadSprites(); }
        // set our gameobject pos
        GameObject obj = new GameObject();
        obj.name = "Wall";
        obj.transform.parent = tParent;
        obj.transform.localPosition = new Vector3(tOffset.x, tOffset.y);

        // create the block
        Block block = obj.AddComponent<Block>();
        block.m_tOffset = tOffset;

        // create the wall sprite
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        Sprite sprite = ms_aSpritesWall[Random.Range(0, ms_aSpritesWall.Length)];
        sr.sprite = sprite;
        sr.transform.position = obj.transform.position;

        // create the interior object
        block.m_oInterior = new GameObject();
        block.m_oInterior.name = "Interior";
        block.m_oInterior.transform.parent = obj.transform;
        block.m_oInterior.transform.localPosition = new Vector3(0,0,2);
        if(outline)
        {
            block.m_oInterior.transform.localScale *= 1.1f;
        }

        // create interior sprite
        sr = block.m_oInterior.AddComponent<SpriteRenderer>();
        sprite = Resources.Load<Sprite>("Sprites/Blank64");
        sr.sprite = sprite;
        sr.color = new Color(0f, 0f, 0f, 0.5f);

        BoxCollider2D col = obj.AddComponent<BoxCollider2D>();


        return block;
    }
    public enum E_ObstructionType
    {
        top, left, right, bottom, error
    }

    public static Block NewObstruction(Transform tParent, Vector2Int tOffset, E_ObstructionType type)
    {
        GameObject obj = new GameObject();
        obj.transform.parent = tParent;
        obj.transform.localPosition = new Vector3(tOffset.x, tOffset.y, 0);

        obj.AddComponent<BoxCollider2D>();

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        Sprite sprite = LoadSprite(type);
        sr.sprite = sprite;

        Block block = obj.AddComponent<Block>();
        block.m_tOffset = tOffset;


        return block;
    }

    static Sprite LoadSprite(E_ObstructionType type)
    {
        switch(type)
        {
            case E_ObstructionType.top: return ms_aSpritesTop[Random.Range(0, ms_aSpritesTop.Length)];
            case E_ObstructionType.left:
            case E_ObstructionType.right: return ms_aSpritesSide[Random.Range(0, ms_aSpritesSide.Length)];
            case E_ObstructionType.bottom: return ms_aSpritesBottom[Random.Range(0, ms_aSpritesBottom.Length)];
            default: return Resources.Load<Sprite>("Sprites/TempObstruction");
        }
    }
    public Vector2Int GetOffset()
    {
        return m_tOffset;
    }

    public void SetInteriorColour(Color newCol)
    {
        if(m_oInterior != null)
        {
            m_oInterior.GetComponent<SpriteRenderer>().color = newCol;
        }
    }

    public void SetOpenRoof(bool open) { m_bHasOpenRoof = open; }
    public void SetOpenBase(bool open) { m_bHasOpenBase = open; }
    public bool GetOpenRoof() { return m_bHasOpenRoof; }
    public bool GetOpenBase() { return m_bHasOpenBase; }

    public void OnAddToTower(Vector2Int shapeOrigin)
    {
        transform.parent = null;
        Vector3 pos = transform.position;
        pos.x = shapeOrigin.x + m_tOffset.x;
        pos.y = shapeOrigin.y + m_tOffset.y;
        pos.z = -1;
        transform.position = pos;
        if (m_oInterior != null)
        {
            SetInteriorColour(new Color(0, 1, 1, 1));
            m_oInterior.transform.localScale = new Vector3(1,1,1);
        }
        m_tOffset = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }
}
