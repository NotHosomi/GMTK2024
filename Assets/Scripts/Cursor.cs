using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    #region singleton
    private static Cursor instance;
    public static Cursor Get()
    {
        return instance;
    }
    public static void Init()
    {
        instance = new Cursor();
    }
    public static void Delete()
    {
        instance = null;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(2))
        {
            GameObject obj = new GameObject();
            Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tMousePos.z = 0;
            obj.transform.position = tMousePos;
            obj.AddComponent<Shape>();
        }
    }
}
