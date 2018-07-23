using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;

/// <summary>
/// Nash
/// </summary>
public class PoteniallyVisibleSet
{
    private Vector3 tileSize = new Vector3(128, 0, 128);
    private List<float> verticalSize = new List<float> { 1f, 3f};
    private Vector3 bigCellSize = new Vector3(128, 0, 128);
    private Vector3 middleCellSize = new Vector3(64, 0, 64);
    private Vector3 smallCellSize = new Vector3(64, 0, 64);

    private Vector3 mapSize = new Vector3(128, 0, 128);
    private Vector3 portalSize = new Vector3(64, 0, 64);
    private const int portalPointCount = 8;
    private int targetAreaPointCount = 0;

    private List<Tile> tileList;

    public enum MapItemSize
    {
        Big,
        Middle,
        Small
    }

    public class TileGroup : ScriptableObject
    {
        public List<Tile> tileList;
    }

    [System.Serializable]
    public class Tile
    {
        public int id;
        public int x;
        public int z;
        public List<Portal> portalList;
        public List<Cell> bigAreaList;
        public List<Cell> middleAreaList;
        public List<Cell> smallAreaList;
    }

    [System.Serializable]
    public class Portal
    {
        public int id;
        public int x;
        public int z;
        public List<Vector3> rayStartPointList;
    }

    [System.Serializable]
    public class Cell
    {
        public int id;
        public int x;
        public int z;
        public bool isVisible;
        public List<Vector3> rayEndPointList;
    }

    [MenuItem("Tools/CalculatePVS")]

    public static void CalculatePVS()
    {
        PoteniallyVisibleSet poteniallyVisibleSet = new PoteniallyVisibleSet();
        poteniallyVisibleSet.CaptureMapGrid();
        poteniallyVisibleSet.CalculateMapPVS();
    }

    private void CalculateMapPVS()
    {
        for (int i = 0; i < tileList.Count; i++)
        {
            Tile tile = tileList[i];
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlRoot = xmlDoc.CreateElement("root");

            XmlElement tileElement = xmlDoc.CreateElement("tile");
            tileElement.SetAttribute("x", tile.x.ToString());
            tileElement.SetAttribute("z", tile.z.ToString());
            xmlRoot.AppendChild(tileElement);

            for (int j = 0; j < tile.portalList.Count; j++)
            {
                Portal portal = tile.portalList[j];
                XmlElement portalElement = xmlDoc.CreateElement("portal");
                portalElement.SetAttribute("x", portal.x.ToString());
                portalElement.SetAttribute("z", portal.z.ToString());
                tileElement.AppendChild(portalElement);

                for (int c1 = 0; c1 < tile.bigAreaList.Count; c1++)
                {
                    Cell cell = tile.bigAreaList[c1];
                    XmlElement cellElement = CalculateCellPVS(cell, portal, xmlDoc);
                    if (cellElement != null)
                    {
                        portalElement.AppendChild(cellElement);
                    }
                }
                for (int c2 = 0; c2 < tile.middleAreaList.Count; c2++)
                {
                    Cell cell = tile.middleAreaList[c2];
                    XmlElement cellElement = CalculateCellPVS(cell, portal, xmlDoc);
                    if (cellElement != null)
                    {
                        portalElement.AppendChild(cellElement);
                    }
                }
                for (int c3 = 0; c3 < tile.smallAreaList.Count; c3++)
                {
                    Cell cell = tile.smallAreaList[c3];
                    XmlElement cellElement = CalculateCellPVS(cell, portal, xmlDoc);
                    if (cellElement != null)
                    {
                        portalElement.AppendChild(cellElement);
                    }
                }
            }

            string xmlDataPath = Application.dataPath + "/Editor/PVS/" + tile.id + ".xml";
            xmlDoc.AppendChild(xmlRoot);
            xmlDoc.Save(xmlDataPath);
        }
    }

    private XmlElement CalculateCellPVS(Cell cell, Portal portal, XmlDocument xml)
    {
        if (cell.isVisible == false)
        {
            for (int k = 0; k < portal.rayStartPointList.Count; k++)
            {
                Vector3 origin = portal.rayStartPointList[k];
                XmlElement xmlElement = xml.CreateElement("cell");
                for (int i = 0; i < cell.rayEndPointList.Count; i++)
                {
                    for (int j = 0; j < verticalSize.Count; j++)
                    {
                        float height = verticalSize[j];
                        Vector3 start = origin + Vector3.up * height;
                        Vector3 end = cell.rayEndPointList[i] + Vector3.up * height;
                        Vector3 direction = (end - start).normalized;
                        float distance = Vector3.Distance(end, start);

                        if (Physics.Raycast(start, direction, distance))
                        {
                            Debug.DrawLine(start, end, Color.green);
                            cell.isVisible = true;
                            xmlElement.SetAttribute("x", cell.x.ToString());
                            xmlElement.SetAttribute("z", cell.z.ToString());
                            return xmlElement;
                        }
                        else
                        {
                            Debug.DrawLine(start, end, Color.red);
                        }
                    }

                }
            }           
        }
        return null;
    }

    private void CaptureMapGrid()
    {
        int mapHorizontalTiles = Mathf.CeilToInt(mapSize.x / tileSize.x);
        int mapVerticalTiles = Mathf.CeilToInt(mapSize.z / tileSize.z);
        Debug.Log("mapHorizontalTiles " + mapHorizontalTiles + " " + mapVerticalTiles);
        tileList = new List<Tile>(mapHorizontalTiles * mapVerticalTiles);
        for (int i = 0; i < mapHorizontalTiles; i++)
        {
            for (int j = 0; j < mapVerticalTiles; j++)
            {
                Tile tile = new Tile();
                tile.id = tileList.Count + 1;
                tile.x = i;
                tile.z = j;
                tileList.Add(tile);
                CapturePortalGrid(tile);
                CaptureCellGrid(tile, MapItemSize.Big);
                CaptureCellGrid(tile, MapItemSize.Middle);
                CaptureCellGrid(tile, MapItemSize.Small);
            }
        }

        TileGroup tileGroup = ScriptableObject.CreateInstance<TileGroup>();
        tileGroup.tileList = tileList;
        AssetDatabase.CreateAsset(tileGroup, "Assets/Editor/PVS/TileGroup.asset");
        AssetDatabase.SaveAssets();
    }

    private void CapturePortalGrid(Tile tile)
    {
        int tileHorizontalPortals = Mathf.CeilToInt(tileSize.x / portalSize.x);
        int tileVerticalPortals = Mathf.CeilToInt(tileSize.z / portalSize.z);

        List<Portal> portalList = new List<Portal>(tileHorizontalPortals * tileVerticalPortals);
        for (int i = 0; i < tileHorizontalPortals; i++)
        {
            for (int j = 0; j < tileVerticalPortals; j++)
            {
                Portal portal = new Portal();
                portal.id = portalList.Count + 1;
                portal.x = i;
                portal.z = j;
                portal.rayStartPointList = new List<Vector3>(portalPointCount);
                for (int k = 0; k < portalPointCount; k++)
                {
                    float x = UnityEngine.Random.Range(0, portalSize.x);
                    float z = UnityEngine.Random.Range(0, portalSize.z);
                    float startX = tile.x * tileSize.x + i * portalSize.x;
                    float startZ = tile.z * tileSize.z + j * portalSize.z;
                    Vector3 point = new Vector3();
                    point.x = startX + x;
                    point.z = startZ + z;
                    portal.rayStartPointList.Add(point);
                }
                portalList.Add(portal);
            }
        }
        tile.portalList = portalList;
    }

    private void CaptureCellGrid(Tile tile, MapItemSize size)
    {
        Vector3 cellSize = Vector3.zero;
        targetAreaPointCount = 0;
        switch (size)
        {
            case MapItemSize.Big:
                cellSize = bigCellSize;
                targetAreaPointCount = UnityEngine.Random.Range(16, 16);
                break;
            case MapItemSize.Middle:
                cellSize = middleCellSize;
                targetAreaPointCount = UnityEngine.Random.Range(8, 8);
                break;
            case MapItemSize.Small:
                cellSize = smallCellSize;
                targetAreaPointCount = UnityEngine.Random.Range(4, 4);
                break;
            default:
                cellSize = bigCellSize;
                targetAreaPointCount = UnityEngine.Random.Range(16, 16);
                break;
        }
       
        int tileHorizontalCells = Mathf.CeilToInt(tileSize.x / cellSize.x);
        int tileVerticalCells = Mathf.CeilToInt(tileSize.z / cellSize.z);

        List<Cell> cellList = new List<Cell>(tileHorizontalCells * tileVerticalCells);
        for (int i = 0; i < tileHorizontalCells; i++)
        {
            for (int j = 0; j < tileVerticalCells; j++)
            {
                Cell cell = new Cell();
                cell.id = cellList.Count + 1;
                cell.x = i;
                cell.z = j;
                cell.rayEndPointList = new List<Vector3>(targetAreaPointCount);
                for (int k = 0; k < targetAreaPointCount; k++)
                {
                    float x = UnityEngine.Random.Range(0, cellSize.x);
                    float z = UnityEngine.Random.Range(0, cellSize.z);
                    float startX = tile.x * tileSize.x + i * cellSize.x;
                    float startZ = tile.z * tileSize.z + j * cellSize.z;
                    Vector3 point = new Vector3();
                    point.x = startX + x;
                    point.z = startZ + z;
                    cell.rayEndPointList.Add(point);
                }
                cellList.Add(cell);
            }
        }

        switch (size)
        {
            case MapItemSize.Big:
                tile.bigAreaList = cellList;
                break;
            case MapItemSize.Middle:
                tile.middleAreaList = cellList;
                break;
            case MapItemSize.Small:
                tile.smallAreaList = cellList;
                break;
            default:
                tile.bigAreaList = cellList;
                break;
        }
    }
}
