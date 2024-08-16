using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    bool m_bCarried = false;
    List<Block> m_vBlocks;
    List<Vector2Int> m_vBlockOffsets;
    List<Vector2Int> m_vObstructionOffsets;
    List<Vector2Int> m_vOpenEdges;

    bool IsEdgeFree(Vector2Int offset)
    {
        foreach(Vector2Int tUsed in m_vBlockOffsets)
        {
            if(offset == tUsed)
            {
                return false;
            }    
        }
        return true;
    }
    void Start()
    {
        m_vOpenEdges = new List<Vector2Int>
        {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
        };
        m_vBlockOffsets = new List<Vector2Int> { new Vector2Int(0, 0) };

        m_vBlocks = new List<Block>();
        m_vBlocks.Add(new Block());

        int blockCount = 1;
        int maxBlocks = Random.Range(2, 5);
        while (blockCount < maxBlocks)
        {
            // pick where to store the new block
            int offsetIdx = Random.Range(0, m_vOpenEdges.Count);
            Vector2Int newBlockOffset = m_vOpenEdges[offsetIdx];
            m_vOpenEdges.RemoveAt(offsetIdx);
            m_vBlockOffsets.Add(newBlockOffset);

            // find new open edges
            Vector2Int newEdge = newBlockOffset;
            newEdge.y += 1;
            if (IsEdgeFree(newEdge)) { m_vOpenEdges.Add(newEdge); }
            newEdge = newBlockOffset;
            newEdge.y -= 1;
            if (IsEdgeFree(newEdge)) { m_vOpenEdges.Add(newEdge); }
            newEdge = newBlockOffset;
            newEdge.x -= 1;
            if (IsEdgeFree(newEdge)) { m_vOpenEdges.Add(newEdge); }
            newEdge = newBlockOffset;
            newEdge.x += 1;
            if (IsEdgeFree(newEdge)) { m_vOpenEdges.Add(newEdge); }


            m_vBlocks.Add(new Block());
            ++blockCount;
        }

        int obstructionCount = 0;
        int maxObstructions = Random.Range(0, blockCount);
        while(obstructionCount < maxObstructions)
        {

        }
    }

    void Init()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(m_bCarried)
        {
            CarryUpdate();
        }
        else
        {
            DropUpdate();
        }
    }

    void CarryUpdate()
    {

    }

    void DropUpdate()
    {

    }

    void Grab()
    {
        m_bCarried = true;
    }
    void Drop()
    {
        m_bCarried = false;
    }

    void SetPos(Vector2 pos)
    {
        for(int i = 0; i < m_vBlocks.Count; ++i)
        {
            m_vBlocks[i].SetPos(pos + m_vBlockOffsets[i]);
        }
    }
}
