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
    private Vector3 tileSize = new Vector3(256, 0, 256);
    private List<float> verticalSize = new List<float> { 1f, 3f};
    private Vector3 bigCellSize = new Vector3(128, 0, 128);
    private Vector3 middleCellSize = new Vector3(64, 0, 64);
    private Vector3 smallCellSize = new Vector3(32, 0, 32);

    private Vector3 mapSize = new Vector3(1024, 0, 1024);
    private Vector3 portalSize = new Vector3(32, 0, 32);
    private const int startPortalPointCount = 16;
    private Vector4 endPortalPointList = new Vector4(32, 16, 8, 4);
    private int targetAreaPointCount = 0;

    private List<PoteniallyVisibleSetItem> poteniallyVisibleSetItemList;

    private TileGroup tileGroup;
    private List<MapTile> tileList;

    [MenuItem("Tools/CalculatePVS")]

    public static void CalculatePVS()
    {
        PoteniallyVisibleSet poteniallyVisibleSet = new PoteniallyVisibleSet();
        poteniallyVisibleSet.CaptureMapGrid();
        poteniallyVisibleSet.AnalyzerMapItemOwner();
        poteniallyVisibleSet.CalculateMapPVS();
        EditorUtility.DisplayDialog("CalculatePVS", "Calculated PVS successfully!", "OK");
    }

    private void AnalyzerMapItemOwner()
    {
        if (poteniallyVisibleSetItemList == null)
        {
            GameObject root = GameObject.Find("OcclusionCulling");
            PoteniallyVisibleSetItem[] poteniallyVisibleSetItems = root.GetComponentsInChildren<PoteniallyVisibleSetItem>();
            poteniallyVisibleSetItemList = new List<PoteniallyVisibleSetItem>(poteniallyVisibleSetItems);
        }
        ClearAllMapItemOwner();
        for (int i = 0; i < tileGroup.tileList.Count; i++)
        {
            MapTile tile = tileGroup.tileList[i];
            ShowSpecifiedMapItem(MapItemSize.Big);
            HideSpecifiedMapItem(MapItemSize.Middle);
            HideSpecifiedMapItem(MapItemSize.Small);
            for (int j = 0; j < tile.bigAreaList.Count; j++)
            {
                AnalyzerMapItemOwner(tile.bigAreaList[j], tile);
            }
            ShowSpecifiedMapItem(MapItemSize.Middle);
            HideSpecifiedMapItem(MapItemSize.Big);
            HideSpecifiedMapItem(MapItemSize.Small);
            for (int k = 0; k < tile.middleAreaList.Count; k++)
            {
                AnalyzerMapItemOwner(tile.middleAreaList[k], tile);
            }
            ShowSpecifiedMapItem(MapItemSize.Small);
            HideSpecifiedMapItem(MapItemSize.Big);
            HideSpecifiedMapItem(MapItemSize.Middle);
            for (int n = 0; n < tile.smallAreaList.Count; n++)
            {
                AnalyzerMapItemOwner(tile.smallAreaList[n], tile);
            }
        }

        ShowAllMapItem();
    }

    private void AnalyzerMapItemOwner(Cell cell, MapTile tile)
    {
        int size = GetCellSize(cell.size);
        Vector3 center = new Vector3(cell.x + size / 2, 1f, cell.z + size / 2) + Vector3.up * 100f;
        Vector3 halfExtents = new Vector3(size / 2, 1f, size / 2);
        Vector3 direction = Vector3.down;

        RaycastHit hit;
        if (Physics.BoxCast(center, halfExtents, direction, out hit))
        {
            PoteniallyVisibleSetItem pvsItem = hit.collider.GetComponent<PoteniallyVisibleSetItem>();
            if (pvsItem != null)
            {
                if (pvsItem.size != cell.size)
                {
                    Debug.LogWarning(string.Format("AnalyzerMapItemOwner PVSItem size{0} is not equal cell size{1}.", pvsItem.size, cell.size));
                    return;
                }
                if (pvsItem.ownerCellIdList.IndexOf(cell.Id) >= 0)
                {
                    Debug.LogWarning(string.Format("AnalyzerMapItemOwner duplicate ownerCellId{0}.", cell.Id));
                    return;
                }
                pvsItem.ownerCellIdList.Add(cell.Id);
            }
        }
    }

    private int GetCellSize(MapItemSize size)
    {
        switch (size)
        {
            case MapItemSize.Big:
                return (int)bigCellSize.x;
            case MapItemSize.Middle:
                return (int)middleCellSize.x;
            case MapItemSize.Small:
                return (int)smallCellSize.x;
            default:
                return (int)bigCellSize.x;
        }
    }

    private void CalculateMapPVS()
    {
        for (int i = 0; i < tileList.Count; i++)
        {
            MapTile tile = tileList[i];
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

            string xmlDataPath = Application.dataPath + "/Resources/" + tile.id + ".xml";
            xmlDoc.AppendChild(xmlRoot);
            xmlDoc.Save(xmlDataPath);
        }
    }

    private XmlElement CalculateCellPVS(Cell cell, Portal portal, XmlDocument xml)
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
                    RaycastHit hitInfo;
                    if (Physics.Raycast(start, direction, out hitInfo, distance))
                    {
                        PoteniallyVisibleSetItem pvsItem = hitInfo.collider.GetComponent<PoteniallyVisibleSetItem>();
                        if (pvsItem != null)
                        {
                            if (pvsItem.size != cell.size)
                            {
                                Debug.LogWarning(string.Format("CalculateMapPVS PVSItem size{0} is not equal cell size{1}.", pvsItem.size, cell.size));
                                continue;
                            }
                            else if (pvsItem.occlusionType != MapItemOcclusionType.Occluder)
                            {
                                Debug.LogWarning(string.Format("CalculateMapPVS PVSItem occlusionType{0} is not equal Occluder.", pvsItem.occlusionType));
                                //TODO HideSpecified(not Occluder)MapItem 
                                continue;
                            }
                            else if (pvsItem.ownerCellIdList.IndexOf(cell.Id) >= 0)
                            {
                                Debug.DrawLine(start, end, Color.green);
                                cell.isVisible = true;
                                xmlElement.SetAttribute("id", cell.Id.ToString());
                                return xmlElement;
                            }
                        }
                    }
                    else
                    {
                        Debug.DrawLine(start, end, Color.red);
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
        tileList = new List<MapTile>(mapHorizontalTiles * mapVerticalTiles);
        for (int i = 0; i < mapHorizontalTiles; i++)
        {
            for (int j = 0; j < mapVerticalTiles; j++)
            {
                MapTile tile = new MapTile();
                tile.id = tileList.Count + 1;
                tile.x = (int)(i * tileSize.x);
                tile.z = (int)(j * tileSize.z);
                tile.tileSize = (int)tileSize.x;
                tile.bigAreaSize = (int)bigCellSize.x;
                tile.middleAreaSize = (int)middleCellSize.x;
                tile.smallAreaSize = (int)smallCellSize.x;
                tile.portalSize = (int)portalSize.x;
                tileList.Add(tile);
                CapturePortalGrid(tile);
                CaptureCellGrid(tile, MapItemSize.Big);
                CaptureCellGrid(tile, MapItemSize.Middle);
                CaptureCellGrid(tile, MapItemSize.Small);
            }
        }

        tileGroup = ScriptableObject.CreateInstance<TileGroup>();
        tileGroup.mapHorizontalTiles = mapHorizontalTiles;
        tileGroup.mapVerticalTiles = mapVerticalTiles;
        tileGroup.tileList = tileList;
        AssetDatabase.CreateAsset(tileGroup, "Assets/Resources/TileGroup.asset");
        AssetDatabase.SaveAssets();
    }

    private void CapturePortalGrid(MapTile tile)
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
                portal.x = (int)(tile.x + i * portalSize.x);
                portal.z = (int)(tile.z + j * portalSize.z);
                portal.rayStartPointList = new List<Vector3>(startPortalPointCount);
                for (int k = 0; k < startPortalPointCount; k++)
                {
                    Vector3 point = new Vector3();
                    float x = UnityEngine.Random.Range(0, portalSize.x);
                    float z = UnityEngine.Random.Range(0, portalSize.z);

                    point.x = portal.x + x;
                    point.z = portal.z + z;
                    portal.rayStartPointList.Add(point);
                }
                portalList.Add(portal);
            }
        }
        tile.portalList = portalList;
    }

    private void CaptureCellGrid(MapTile tile, MapItemSize size)
    {
        Vector3 cellSize = Vector3.zero;
        targetAreaPointCount = 0;
        switch (size)
        {
            case MapItemSize.Big:
                cellSize = bigCellSize;
                targetAreaPointCount = UnityEngine.Random.Range((int)endPortalPointList.y, (int)endPortalPointList.x);
                break;
            case MapItemSize.Middle:
                cellSize = middleCellSize;
                targetAreaPointCount = UnityEngine.Random.Range((int)endPortalPointList.x, (int)endPortalPointList.y);
                break;
            case MapItemSize.Small:
                cellSize = smallCellSize;
                targetAreaPointCount = UnityEngine.Random.Range((int)endPortalPointList.w, (int)endPortalPointList.z);
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
                cell.size = size;
                cell.Id = cellList.Count + 1;
                cell.x = (int)(tile.x + i * cellSize.x);
                cell.z = (int)(tile.z + j * cellSize.z);
                cell.rayEndPointList = new List<Vector3>(targetAreaPointCount);
                for (int k = 0; k < targetAreaPointCount; k++)
                {
                    Vector3 point = new Vector3();
                    float x = UnityEngine.Random.Range(0, cellSize.x);
                    float z = UnityEngine.Random.Range(0, cellSize.z);
  
                    point.x = cell.x + x;
                    point.z = cell.z + z;
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

    private void HideSpecifiedMapItem(MapItemSize size)
    {
        for (int i = 0; i < poteniallyVisibleSetItemList.Count; i++)
        {
            PoteniallyVisibleSetItem pvsItem = poteniallyVisibleSetItemList[i];
            if (pvsItem.size == size)
            {
                pvsItem.gameObject.SetActive(false);
            }
        }
    }

    private void ShowSpecifiedMapItem(MapItemSize size)
    {
        for (int i = 0; i < poteniallyVisibleSetItemList.Count; i++)
        {
            PoteniallyVisibleSetItem pvsItem = poteniallyVisibleSetItemList[i];
            if (pvsItem.size == size)
            {
                pvsItem.gameObject.SetActive(true);
            }
        }
    }

    private void ShowAllMapItem()
    {
        for (int i = 0; i < poteniallyVisibleSetItemList.Count; i++)
        {
            PoteniallyVisibleSetItem pvsItem = poteniallyVisibleSetItemList[i];
            pvsItem.gameObject.SetActive(true);
        }
    }

    private void ClearAllMapItemOwner()
    {
        for (int i = 0; i < poteniallyVisibleSetItemList.Count; i++)
        {
            PoteniallyVisibleSetItem pvsItem = poteniallyVisibleSetItemList[i];
            pvsItem.ownerCellIdList.Clear();
        }
    }
}
