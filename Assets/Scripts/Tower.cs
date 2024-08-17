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

    int mz_nMaxWidth = 10;

    private Tower()
    {
        List<Block> row = AppendRow();
        for (int x = 0; x < 5; ++x)
        {
            Block b = Block.NewBlock(null, new Vector2Int(x, 0), false);
            row[x] = b;
            b.OnAddToTower(new Vector2Int(0,0));
            m_vOpenPlatforms.Add(new Vector2Int(x, 0));
        }
    }
    List<Block> AppendRow()
    {
        m_vRows.Add(new List<Block>());
        List<Block> row = m_vRows[m_vRows.Count - 1];
        for (int i = 0; i < mz_nMaxWidth; ++i)
        {
            row.Add(null);
        }
        return row;
    }

    public bool CheckValidity(Shape shape)
    {
        Vector2Int origin = shape.Origin();
        List<Vector2Int> validCoords = new List<Vector2Int>();
        List<Vector2Int> invalidCoords = new List<Vector2Int>();
        
        bool invalid = false;
        foreach(Vector2Int offset in shape.GetFootprint())
        {
            Vector2Int coord = offset + origin;
            if (coord.x < 0 || coord.x >= mz_nMaxWidth || coord.y < 0)
            { // oob
                invalidCoords.Add(coord);
                invalid = true;
                continue;
            }
            while(coord.y >= m_vRows.Count)
            {
                AppendRow();
            }
            if (m_vRows[coord.y][coord.x] != null)
            { // this is an invalid space
                invalidCoords.Add(coord);
                invalid = true;
                continue;
            }
            validCoords.Add(coord);
        }
        if(invalid)
        {
            HighlightManager.Get().DrawValidity(validCoords, invalidCoords);
        }
        else
        {
            HighlightManager.Get().HidePreview();
            HighlightManager.Get().HideValidity();
        }
        return !invalid;
    }

    public void AttachToTower(Shape shape)
    {
        Vector2Int shapeOrigin = shape.Origin();
        // expand the OpenPlatforms list
        foreach (Vector2Int roofSlot in shape.GetOpenRoofSlots())
        {
            Vector2Int newPlatform = roofSlot + shapeOrigin;
            if(IsFree(newPlatform))
            {
                --newPlatform.y;
                m_vOpenPlatforms.Add(newPlatform);
            }

        }
        foreach (Block block in shape.GetBlocks())
        {
            Vector2Int coord = shapeOrigin + block.GetOffset();
            block.OnAddToTower(shapeOrigin);
            m_vRows[coord.y][coord.x] = block;
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
                if(block!=null)
                {
                    block.SetInteriorColour(new Color(0,1,1));
                }
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
                if(block!=null)
                {
                    block.SetInteriorColour(new Color(1,0.73f,0));
                }
            }
        }
    }
    public List<Vector2Int> GetPlatforms()
    {
        return m_vOpenPlatforms;
    }

    public bool IsFree(Vector2Int coord)
    {
        while(coord.y >= m_vRows.Count)
        {
            AppendRow();
        }
        if (coord.x < 0 || coord.y < 0 || coord.x > mz_nMaxWidth)
        {
            return false;
        }
        return m_vRows[coord.y][coord.x] == null;
    }
}
