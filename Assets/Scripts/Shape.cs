using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Shape : MonoBehaviour
{
    public static List<Shape> ms_vShapes = new List<Shape>();
    bool m_bHeld = false;
    List<Block> m_vBlocks;

    List<Vector2Int> m_vBlockPositions;
    List<Vector2Int> m_vObstructedSlots;

    List<Vector2Int> m_vOpenSlots;
    List<Vector2Int> m_vOpenBaseSlots;
    List<Vector2Int> m_vOpenRoofSlots;

    Vector3 m_tGrabDelta = new Vector3();

    void Awake()
    {
        ms_vShapes.Add(this);
        int yOffset = 0;
        // add the initial block
        m_vBlocks = new List<Block>();
        m_vBlocks.Add(Block.NewBlock(transform, new Vector2Int(0, 0)));
        m_vOpenSlots = new List<Vector2Int>
        {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
        };
        m_vBlockPositions = new List<Vector2Int> { new Vector2Int(0, 0) };
        m_vOpenBaseSlots = new List<Vector2Int> { new Vector2Int(0, -1) };
        m_vOpenRoofSlots = new List<Vector2Int> { new Vector2Int(0, 1) };
        m_vObstructedSlots = new List<Vector2Int>();

        int blockCount = 1;
        int obstructionCount = 0;
        int maxBlocks = Random.Range(2, 5); // create 1 to 4 additional blocks
        int maxObstructions = Random.Range(0, maxBlocks); 
        Debug.Log("Generating shape with " + maxBlocks + " blocks and " + maxObstructions + " obstructions");

        // create blocks
        while (blockCount < maxBlocks)
        {
            Vector2Int selection = PickOpenSlot();
            m_vBlockPositions.Add(selection);
            FindNewNeighbours(selection);

            // instantiate the block
            m_vBlocks.Add(Block.NewBlock(transform, selection));
            ++blockCount;

            // check if the shape's initial location needs to be raised
            if(selection.y < yOffset)
            {
                yOffset = selection.y;
            }
        }

        // create obstructions
        while(obstructionCount < maxObstructions)
        {
            Vector2Int selection = PickOpenSlot();
            m_vObstructedSlots.Add(selection);
        
            // instantiate the block
            m_vBlocks.Add(Block.NewObstruction(transform, selection));
            ++obstructionCount;
        
            // check if the shape's initial location needs to be raised
            if (selection.y < yOffset)
            {
                yOffset = selection.y;
            }
        }

        // set the initial pos of all 
        Vector3 pos = transform.position;
        pos.y -= yOffset;
        pos.z = -5;
        transform.position = pos;
    }

    Vector2Int PickOpenSlot()
    {
        // pick where to place the new block
        int offsetIdx = Random.Range(0, m_vOpenSlots.Count);
        Vector2Int selection = m_vOpenSlots[offsetIdx];

        // remove the selected position from slots
        m_vOpenSlots.RemoveAt(offsetIdx);
        if (m_vOpenBaseSlots.Contains(selection)) { m_vOpenBaseSlots.Remove(selection); }
        if (m_vOpenRoofSlots.Contains(selection)) { m_vOpenBaseSlots.Remove(selection); }
        return selection;
    }

    void FindNewNeighbours(Vector2Int pos)
    {
        Vector2Int newNeighbour = pos;
        newNeighbour.x -= 1;
        if (!m_vBlockPositions.Contains(newNeighbour)) { m_vOpenSlots.Add(newNeighbour); }
        newNeighbour = pos;
        newNeighbour.x += 1;
        if (!m_vBlockPositions.Contains(newNeighbour)) { m_vOpenSlots.Add(newNeighbour); }
        newNeighbour.y += 1;
        if (!m_vBlockPositions.Contains(newNeighbour))
        {
            m_vOpenSlots.Add(newNeighbour);
            m_vOpenRoofSlots.Add(newNeighbour);
        }
        newNeighbour = pos;
        newNeighbour.y -= 1;
        if (!m_vBlockPositions.Contains(newNeighbour))
        {
            m_vOpenSlots.Add(newNeighbour);
            m_vOpenBaseSlots.Add(newNeighbour);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_bHeld)
        {
            HeldUpdate();
        }
        else
        {
            NotHeldUpdate();
        }
    }

    void HeldUpdate()
    {
        Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tMousePos.z = 0;
        transform.position = tMousePos + m_tGrabDelta;

        // check if we can snap
        List<Vector2Int> platforms = Tower.Get().GetPlatforms();
        Vector2Int roundedOrigin = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        bool couldAttach = false;
        foreach(Vector2Int platformCoord in platforms)
        {
            foreach(Vector2Int offset in m_vBlockPositions)
            {
                if(roundedOrigin + offset == platformCoord)
                {
                    couldAttach = true;
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            m_bHeld = false;
            if(couldAttach)
            {
                if(!TryAttach())
                {
                    // failed to attach
                }
            }
        }
        if (!couldAttach)
        {
            return;
        }
    }

    void NotHeldUpdate()
    {
        Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tMousePos.z = 0;

        if (Input.GetMouseButtonDown(0))
        {
            m_bHeld = true;
            m_tGrabDelta = transform.position - tMousePos;
        }
    }

    public List<Vector2Int> GetFootprint()
    {
        return m_vBlockPositions.Union(m_vObstructedSlots).ToList();
    }

    public List<Block> GetBlocks()
    {
        return m_vBlocks;
    }

    bool TryAttach()
    {
        Debug.Log("Trying to attach shape to tower");
        return true;
    }
    public List<Vector2Int> GetOpenRoofSlots()
    {
        return m_vOpenRoofSlots;
    }
}
