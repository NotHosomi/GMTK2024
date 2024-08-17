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

    List<List<Block>> m_vRows = new List<List<Block>>();
    List<Vector2Int> m_vOpenPlatforms = new List<Vector2Int>();

    private Tower()
    {
        List<Block> row = AppendRow();
        for (int x = 0; x < 5; ++x)
        {
            Block b = Block.NewBlock(null, new Vector2Int(x, 0), false);
            row.Add(b);
            b.OnAddToTower();
            m_vOpenPlatforms.Add(new Vector2Int(x, 0));
        }
    }
    List<Block> AppendRow()
    {
        m_vRows.Add(new List<Block>());
        return m_vRows[m_vRows.Count-1];
    }

    public void UpdateHighlight(Shape incomingShape)
    {
        // check if any of the shape's open base's match openSupportingBlocks
    }

    public void CheckShapeSnap(Shape shape, List<Vector2Int> validBlocks, List<Vector2Int> invalidBlocks)
    {
        Vector3 pos = shape.transform.position;
        Vector2Int coord = new Vector2Int(Mathf.RoundToInt(pos.x), -Mathf.RoundToInt(pos.y));

        foreach(Vector2Int offset in shape.GetFootprint())
        {

        }
    }

    public void AttachToTower(Vector2Int pos, Shape shape)
    {
        shape.transform.position = new Vector3(pos.x, pos.y, 0);

        foreach(Vector2Int roofSlot in shape.GetOpenRoofSlots())
        {
            Vector2Int newPlatform = roofSlot;
            --newPlatform.y;
            m_vOpenPlatforms.Add(newPlatform);

        }
        foreach (Block block in shape.GetBlocks())
        {
            block.OnAddToTower();
        }
        Shape.ms_vShapes.Remove(shape);
        GameObject.Destroy(shape.gameObject);
    }

    public void OnDayTime()
    {
        foreach (List<Block> row in m_vRows)
        {
            foreach (Block block in row)
            {
                block.SetInteriorColour(new Color(0,1,1));
            }
        }
    }
    public void OnNightTime()
    {
        if(Shape.ms_vShapes.Count > 0)
        {
            // todo: loss screen
            Debug.Log("Loss");
        }
        foreach(List<Block> row in m_vRows)
        {
            foreach (Block block in row)
            {
                block.SetInteriorColour(new Color(1,0.73f,0));
            }
        }
    }
    public List<Vector2Int> GetPlatforms()
    {
        return m_vOpenPlatforms;
    }
}
