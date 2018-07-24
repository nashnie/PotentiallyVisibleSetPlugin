using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapTile
{
    public int id;
    public int x;
    public int z;
    public List<Portal> portalList;
    public List<Cell> bigAreaList;
    public List<Cell> middleAreaList;
    public List<Cell> smallAreaList;

    public int tileSize = 512;
    public int portalSize;
    public int bigAreaSize;
    public int middleAreaSize;
    public int smallAreaSize;
}
