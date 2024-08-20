using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Shape : MonoBehaviour
{
    bool m_bHeld = false;
    List<Block> m_vBlocks;

    List<Vector2Int> m_vWallPositions;
    List<Vector2Int> m_vObstructedSlots;

    List<Vector2Int> m_vOpenSlots;
    List<Vector2Int> m_vOpenBaseSlots;
    List<Vector2Int> m_vOpenRoofSlots;
    List<Vector2Int> m_vOpenLeftSlots;
    List<Vector2Int> m_vOpenRightSlots;

    Vector3 m_tGrabDelta = new Vector3();

    bool m_bCouldAttach = false;
    bool m_bLocked = false;

    void Awake()
    {
        // add the initial block
        m_vBlocks = new List<Block>();
        m_vBlocks.Add(Block.NewBlock(transform, new Vector2Int(0, 0), true));
        m_vOpenSlots = new List<Vector2Int>
        {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
        };
        m_vWallPositions = new List<Vector2Int> { new Vector2Int(0, 0) };
        m_vOpenBaseSlots = new List<Vector2Int> { new Vector2Int(0, -1) };
        m_vOpenRoofSlots = new List<Vector2Int> { new Vector2Int(0, 1) };
        m_vOpenLeftSlots = new List<Vector2Int>();
        m_vOpenRightSlots = new List<Vector2Int>();
        m_vObstructedSlots = new List<Vector2Int>();

        int maxBlocks = Random.Range(2, 6); // create 1 to 4 additional blocks
        int maxObstructions = Random.Range(0, maxBlocks+2); 
        //Debug.Log("Generating shape with " + maxBlocks + " blocks and " + maxObstructions + " obstructions");

        // create blocks
        for (int i = 1;  i < maxBlocks; ++i)
        {
            Vector2Int selection = PickOpenSlot();
            MarkSlotAsOccupied(selection);
            m_vWallPositions.Add(selection);
            FindNewNeighbours(selection);

            // instantiate the block
            m_vBlocks.Add(Block.NewBlock(transform, selection));
        }

        // create obstructions
        for(int i = 0; i < maxObstructions; ++i)
        {
            Vector2Int selection = PickOpenSlot();
            // check if this is the only available base slot
            if(m_vOpenBaseSlots.Count == 1 && m_vOpenBaseSlots.Contains(selection))
            {   // just don't bother with this one
                Debug.Log("Aborted creating obstruction in only base slot");
                continue;
            }
            Block.E_ObstructionType obType = PickObstructionType(selection);
            if(obType == Block.E_ObstructionType.error)
            {
                // something went wrong
                continue;
            }
            MarkSlotAsOccupied(selection);
            m_vObstructedSlots.Add(selection);

            // instantiate the block
            m_vBlocks.Add(Block.NewObstruction(transform, selection, obType));
        }

        Tray.Get().Add(this);
    }

    Vector2Int PickOpenSlot()
    {
        // pick where to place the new block
        int offsetIdx = Random.Range(0, m_vOpenSlots.Count);
        return m_vOpenSlots[offsetIdx];
    }
    void MarkSlotAsOccupied(Vector2Int tOffset)
    {
        // remove the selected position from slots
        m_vOpenSlots.Remove(tOffset);
        m_vOpenBaseSlots.Remove(tOffset);
        m_vOpenRoofSlots.Remove(tOffset);
        m_vOpenLeftSlots.Remove(tOffset);
        m_vOpenRightSlots.Remove(tOffset);
    }
    void FindNewNeighbours(Vector2Int pos)
    {
        Vector2Int newNeighbour = pos;
        newNeighbour.x -= 1;
        if (!m_vWallPositions.Contains(newNeighbour))
        {
            m_vOpenSlots.Add(newNeighbour);
            m_vOpenLeftSlots.Add(newNeighbour);
        }
        newNeighbour = pos;
        newNeighbour.x += 1;
        if (!m_vWallPositions.Contains(newNeighbour))
        {
            m_vOpenSlots.Add(newNeighbour);
            m_vOpenRightSlots.Add(newNeighbour);
        }
        newNeighbour = pos;
        newNeighbour.y += 1;
        if (!m_vWallPositions.Contains(newNeighbour))
        {
            m_vOpenSlots.Add(newNeighbour);
            m_vOpenRoofSlots.Add(newNeighbour);
        }
        newNeighbour = pos;
        newNeighbour.y -= 1;
        if (!m_vWallPositions.Contains(newNeighbour))
        {
            m_vOpenSlots.Add(newNeighbour);
            m_vOpenBaseSlots.Add(newNeighbour);
        }
    }
    Block.E_ObstructionType PickObstructionType(Vector2Int tOffset)
    {
        List<Block.E_ObstructionType> validType = new List<Block.E_ObstructionType>();
        if (m_vOpenLeftSlots.Contains(tOffset)) { validType.Add(Block.E_ObstructionType.left); }
        if (m_vOpenRightSlots.Contains(tOffset)) { validType.Add(Block.E_ObstructionType.right); }
        if (m_vOpenRoofSlots.Contains(tOffset)) { validType.Add(Block.E_ObstructionType.top); }
        if (m_vOpenBaseSlots.Contains(tOffset)) { validType.Add(Block.E_ObstructionType.bottom); }
        if(validType.Count == 0)
        {
            return Block.E_ObstructionType.error;
        }
        else
        {
            return validType[Random.Range(0, validType.Count)];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_bHeld)
        {
            HeldUpdate();
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
            HighlightManager.Get().DrawPreview(Origin(), m_vWallPositions);
        }
        else
        {
            HighlightManager.Get().HidePreview();
        }
    }

    public List<Vector2Int> GetFootprint()
    {
        return m_vWallPositions.Union(m_vObstructedSlots).ToList();
    }
    public List<Vector2Int> GetWallOffsets()
    {
        return m_vWallPositions;
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
        transform.parent = null;
        Tray.Get().Remove(this);

        Vector3 pos = transform.position;
        pos.z = -3;
        transform.position = pos;

        // set the grab delta
        Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tMousePos.z = 0;
        m_tGrabDelta = transform.position - tMousePos;
    }

    // returns false if the shape is still held
    public bool Release(bool forceDrop = false)
    {
        // check if we can snap now?
        if (m_bCouldAttach)
        {
            bool attached = TryAttach();
            if (!forceDrop)
            {
                return attached;
            }
        }
        m_bHeld = false;
        Tray.Get().Add(this);
        return true;
    }
    public bool HasPotentialConnection()
    {
        bool couldConnect = false;
        Vector2Int origin = Origin();
        foreach(Vector2Int offset in m_vOpenBaseSlots)
        {
            if(Tower.Get().GetPlatforms().Contains(origin + offset))
            {
                couldConnect = true;
                break;
            }
        }
        return couldConnect;
    }
    // returns true if it attaches to the tower
    bool TryAttach()
    {
        if (Tower.Get().CheckValidity(this))
        {
            Tower.Get().AttachToTower(this);
            return true;
        }
        return false;
    }

    public Vector2Int Origin()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }

    // returns the vector from the shape's origin to its center
    public Vector2 GetCenterDelta(bool includeObstructions)
    {
        Vector2 mins = GetMins(includeObstructions);
        Vector2 maxs = GetMaxs(includeObstructions);
        maxs.x += 0.5f;
        maxs.y += 0.5f;
        mins.x -= 0.5f;
        mins.y -= 0.5f;
        return (maxs - mins) / 2 + mins;
    }
    public Vector2Int GetMins(bool includeObstructions)
    {
        Vector2Int min = new Vector2Int();
        foreach (Vector2Int offset in m_vWallPositions)
        {
            if (offset.x < min.x)
            {
                min.x = offset.x;
            }
            if (offset.y < min.y)
            {
                min.y = offset.y;
            }
        }
        if (includeObstructions)
        {
            foreach (Vector2Int offset in m_vObstructedSlots)
            {
                if (offset.x < min.x)
                {
                    min.x = offset.x;
                }
                if (offset.y < min.y)
                {
                    min.y = offset.y;
                }
            }
        }
        return min;
    }
    public Vector2Int GetMaxs(bool includeObstructions)
    {
        Vector2Int max = new Vector2Int();
        foreach (Vector2Int offset in m_vWallPositions)
        {
            if (offset.x > max.x)
            {
                max.x = offset.x;
            }
            if (offset.y > max.y)
            {
                max.y = offset.y;
            }
        }
        if (includeObstructions)
        {
            foreach (Vector2Int offset in m_vObstructedSlots)
            {
                if (offset.x > max.x)
                {
                    max.x = offset.x;
                }
                if (offset.y > max.y)
                {
                    max.y = offset.y;
                }
            }
        }
        return max;
    }

    // returns false if it was already locked;
    public bool Lock()
    {
        if(m_bLocked)
        {
            return false;
        }
        m_bLocked = true;
        foreach (Block block in m_vBlocks)
        {
            BoxCollider2D col = block.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.enabled = false;
            }
        }
        Vector3 pos = transform.localPosition;
        pos.z = 1;
        return true;
    }
    public void ReturnToTray()
    {
        m_bHeld = false;
        Tray.Get().Add(this);
    }
    public bool IsLocked()
    {
        return m_bLocked;
    }

    public Vector2 GetGrabDelta()
    {
        return m_tGrabDelta;
    }
}
