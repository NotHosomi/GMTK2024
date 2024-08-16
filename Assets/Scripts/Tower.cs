using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower
{
#region singleton
    private static Tower instance;
    public static Tower Get()
    {
        return instance;
    }
    public static void Init()
    {
        instance = new Tower();
    }
    public static void Delete()
    {
        instance = null;
    }
    #endregion

    List<Vector2Int> m_vAttachmentPoints;
    List<Block> m_vBlocks;

    private Tower()
    {
        m_vBlocks = new List<Block>();
        for(int x = -2; x < 2; ++x)
        {
            m_vBlocks.Add(new Block(new Vector2Int(x, 0)));
            m_vAttachmentPoints.Add(new Vector2Int(x, 1));
        }
    }
}
