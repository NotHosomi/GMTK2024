using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    GameObject m_oInterior;
    Vector2Int m_tOffset;
    bool m_bHasOpenRoof;
    bool m_bHasOpenBase;

    public static Block NewBlock(Transform tParent, Vector2Int tOffset, bool outline = true)
    {
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
        Sprite sprite = Resources.Load<Sprite>("Sprites/ExampleWall");
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
    public static Block NewObstruction(Transform tParent, Vector2Int tOffset)
    {
        GameObject obj = new GameObject();
        obj.transform.parent = tParent;
        obj.transform.localPosition = new Vector3(tOffset.x, tOffset.y, 0);

        obj.AddComponent<BoxCollider2D>();
        
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        Sprite sprite = Resources.Load<Sprite>("Sprites/TempObstruction");
        sr.sprite = sprite;

        Block block = obj.AddComponent<Block>();
        block.m_tOffset = tOffset;

        return block;
    }

    public Vector2Int GetOffet()
    {
        return m_tOffset;
    }

    public void SetInteriorColour(Color newCol)
    {
        m_oInterior.GetComponent<SpriteRenderer>().color = newCol;
    }

    public void SetOpenRoof(bool open) { m_bHasOpenRoof = open; }
    public void SetOpenBase(bool open) { m_bHasOpenBase = open; }
    public bool GetOpenRoof() { return m_bHasOpenRoof; }
    public bool GetOpenBase() { return m_bHasOpenBase; }

    public void OnAddToTower()
    {
        transform.parent = null;
        Vector3 pos = transform.position;
        pos.z = -1;
        transform.position = pos;
        if (m_oInterior != null)
        {
            SetInteriorColour(new Color(0, 1, 1, 1));
            m_oInterior.transform.localScale = new Vector3(1,1,1);
        }
    }
}
