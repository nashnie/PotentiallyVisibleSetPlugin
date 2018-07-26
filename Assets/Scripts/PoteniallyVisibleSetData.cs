using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

/// <summary>
/// Nash
/// </summary>
public class PoteniallyVisibleSetData : Singleton<PoteniallyVisibleSetData>
{
    public TileGroup tileGroup;
    public MapTile currentMapTile;
    public Dictionary<int, bool> currentCellItemVisibleSetMap = new Dictionary<int, bool>();
    public Dictionary<Rect, List<int>> currentTilePortalMap = new Dictionary<Rect, List<int>>();
    private List<int> currentTilePortalVisibleCellList = null;
    private List<Rect> tileList = new List<Rect>();
    private int currentTileIndex = 0;
    private Rect currentPortalIndex;

    public void InitializePVSData()
    {
        tileGroup = GameObject.Instantiate(Resources.Load("TileGroup")) as TileGroup;
        for (int i = 0; i < tileGroup.tileList.Count; i++)
        {
            MapTile tile = tileGroup.tileList[i];
            Rect rect = new Rect(tile.x, tile.z, tile.tileSize, tile.tileSize);
            tileList.Add(rect);
        }
    }

    public void Update(Player player)
    {
        for (int i = 0; i < tileList.Count; i++)
        {
            Rect rect = tileList[i];
            if (rect.Contains(player.playerPosition))
            {
                if (currentTileIndex != i + 1)
                {
                    //TODO 缓冲区域以及缓存Tile数据
                    currentTileIndex = i + 1;
                    player.currentTileIndex = currentTileIndex;
                    LoadCellItemVisibleSetMapXml(currentTileIndex);
                }
                break;
            }
        }
        foreach (Rect portal in currentTilePortalMap.Keys)
        {
            if (portal.Contains(player.playerPosition))
            {
                if (currentPortalIndex != portal)
                {
                    currentPortalIndex = portal;
                    player.currentPortalIndex = currentPortalIndex;
                    currentTilePortalVisibleCellList = currentTilePortalMap[currentPortalIndex];
                    player.currentTilePortalVisibleCellList = currentTilePortalVisibleCellList;
                }
                break;
            }
        }
    }

    public bool CheckVisible(int cellId)
    {
        if (currentTilePortalVisibleCellList != null)
        {
            return currentTilePortalVisibleCellList.IndexOf(cellId) >= 0;
        }
        return false;
    }

    private void LoadCellItemVisibleSetMapXml(int index)
    {
        currentTilePortalMap.Clear();
        currentMapTile = tileGroup.tileList[index - 1];
        TextAsset textAsset = Resources.Load(index.ToString()) as TextAsset;
        string content = textAsset.text;
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(content);

        XmlNodeList xmlNodeList = xml.GetElementsByTagName("root");
        XmlElement xmlElement = xmlNodeList[0] as XmlElement;
        XmlNodeList tileXmlNodeList = xmlElement.GetElementsByTagName("tile");
        XmlElement tileXmlElement = tileXmlNodeList[0] as XmlElement;
        XmlNodeList portalXmlNodeList = tileXmlElement.GetElementsByTagName("portal");
        foreach (XmlElement portal in portalXmlNodeList)
        {
            int x;
            TryParseInt(portal.GetAttribute("x"), out x);
            int y;
            TryParseInt(portal.GetAttribute("z"), out y);
            Rect rect = new Rect(x, y, currentMapTile.portalSize, currentMapTile.portalSize);
            XmlNodeList cellXmlNodeList = portal.GetElementsByTagName("cell");
            List<int> cellIdList = new List<int>();
            foreach (XmlElement cell in cellXmlNodeList)
            {
                int id;
                TryParseInt(cell.GetAttribute("id"), out id);
                cellIdList.Add(id);
            }
            currentTilePortalMap.Add(rect, cellIdList);
        }
    }

    private bool TryParseInt(string str, out int value)
    {
        return int.TryParse(str, System.Globalization.NumberStyles.Any, null, out value);
    }

    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }
}
