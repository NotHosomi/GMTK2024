using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crane : MonoBehaviour
{
    [SerializeField] GameObject CraneTower;
    [SerializeField] GameObject CraneArm;
    bool m_bLocked = false;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = CraneTower.transform.position;
        pos.y = Camera.main.transform.position.y + 4.5f;
        CraneTower.transform.position = pos;


        if (m_bLocked)
        {
            return;
        }
        pos = CraneArm.transform.position;
        pos.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        if (pos.x < 0)
        {
            pos.x = 0f;
        }
        if (pos.x > Tower.Get().GetMaxWidth()-0.1f)
        {
            pos.x = Tower.Get().GetMaxWidth()-0.1f;
        }
        CraneArm.transform.position = pos;
    }

    public void SetLocked(bool val)
    {
        m_bLocked = val;
    }
}
