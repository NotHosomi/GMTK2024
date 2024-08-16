using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    GameObject m_oFront;
    GameObject m_oBack;
    Vector2Int m_tSnappedPos;
    Vector2Int m_tOffset;
    
    public Block(Vector2Int tOffset)
    {
        m_tOffset = tOffset;

        // spriteFront = random wall texture
        m_oFront = new GameObject();
        SpriteRenderer frontRenderer = m_oFront.AddComponent<SpriteRenderer>();
        Sprite frontSprite = Resources.Load<Sprite>("Sprites/ExampleWall");
        frontRenderer.sprite = frontSprite;
        frontRenderer.sortingLayerID = 1;

        // create rear
        m_oBack = new GameObject();
        SpriteRenderer backRenderer = m_oBack.AddComponent<SpriteRenderer>();
        Sprite backSprite = Resources.Load<Sprite>("Sprites/Blank64");
        backRenderer.sprite = backSprite;
        backRenderer.color = new Color(0.2f, 0.2f, 0.2f);
        backRenderer.sortingLayerID = -1;
    }

    public void SetOffset(Vector2Int tOffset)
    {
        m_tOffset = tOffset;
    }

    public void SetPos(Vector2 pos)
    {
        m_oBack.transform.position = pos + m_tOffset;
        m_oFront.transform.position = pos + m_tOffset;
    }

    public Vector2 GetPos()
    {
        return m_tSnappedPos;
    }
}
