using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    private int id;
    public int x;
    public int z;
    public MapItemSize size;
    public bool isVisible;
    public List<Vector3> rayEndPointList;

    public int Id
    {
        get
        {
            return id;
        }
        set
        {
            switch (size)
            {
                case MapItemSize.Big:
                    id = value;
                    break;
                case MapItemSize.Middle:
                    id = value + 1000;
                    break;
                case MapItemSize.Small:
                    id = value + 10000;
                    break;
                default:
                    break;
            }
            
        }
    }
}