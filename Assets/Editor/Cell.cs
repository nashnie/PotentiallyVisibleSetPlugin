using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public int id;
    public int x;
    public int z;
    public MapItemSize size;
    public bool isVisible;
    public List<Vector3> rayEndPointList;
}