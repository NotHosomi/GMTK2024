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

    bool m_bCouldAttach = false;

    void Awake()
    {
        ms_vShapes.Add(this);
        int yOffset = 0;
        // add the initial block
        m_vBlocks = new List<Block>();
        m_vBlocks.Add(Block.NewBlock(transform, new Vector2Int(0, 0), true, "Sprites/Wall1"));
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
        m_bCouldAttach = HasPotentialConnection();
        if (m_bCouldAttach)
        {
            HighlightManager.Get().DrawPreview(Origin(), m_vBlockPositions);
        }
        else
        {
            HighlightManager.Get().HidePreview();
        }
    }

    void NotHeldUpdate()
    {
    }

    public List<Vector2Int> GetFootprint()
    {
        return m_vBlockPositions.Union(m_vObstructedSlots).ToList();
    }

    public List<Block> GetBlocks()
    {
        return m_vBlocks;
    }

    public List<Vector2Int> GetOpenRoofSlots()
    {
        return m_vOpenRoofSlots;
    }

    public void Grab()
    {
        m_bHeld = true;
        
        // have the object jump up when grabbed
        Vector3 pos = transform.position; 
        pos.y += 0.1f;
        transform.position = pos;

        // set the grab delta
        Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tMousePos.z = 0;
        m_tGrabDelta = transform.position - tMousePos;
    }

    public void Release()
    {
        // check if we can snap now?
        if (m_bCouldAttach && TryAttach())
        {
            return;
        }
        else
        {
            m_bHeld = false;
            Vector3 pos = transform.position;
            pos.x = 9;
            foreach(Vector2Int offset in m_vBlockPositions)
            {
                if (offset.y < -pos.y)
                {
                    pos.y = -offset.y;
                }
            }
            transform.position = pos;
        }
    }
    public bool HasPotentialConnection()
    {
        bool couldConnect = false;
        Vector2Int origin = Origin();
        foreach (Vector2Int platformCoord in Tower.Get().GetPlatforms())
        {
            foreach (Vector2Int offset in m_vOpenBaseSlots)
            {
                if (origin + offset == platformCoord)
                {
                    Debug.Log("Valid connection coord: " + platformCoord);
                    couldConnect = true;
                }
            }
        }
        return couldConnect;
    }
    bool TryAttach()
    {
        Debug.Log("Trying to attach shape to tower");

        if (Tower.Get().CheckValidity(this))
        {
            Tower.Get().AttachToTower(this);
            return true;
        }

        return true;
    }

    public Vector2Int Origin()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }
}
