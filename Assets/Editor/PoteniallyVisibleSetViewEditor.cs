using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Main))]
public class PoteniallyVisibleSetViewEditor : Editor 
{
    private void OnSceneGUI()
    {
        Handles.color = Color.white;
        PoteniallyVisibleSetData poteniallyVisibleSetData = PoteniallyVisibleSetData.Instance;
        if (poteniallyVisibleSetData != null)
        {
            TileGroup tileGroup = poteniallyVisibleSetData.tileGroup;
            if (tileGroup != null)
            {
                Handles.color = Color.yellow;
                for (int i = 0; i < tileGroup.tileList.Count; i++)
                {
                    MapTile tile = tileGroup.tileList[i];
                    float halfTileSize = tile.tileSize / 2;
                    Handles.RectangleHandleCap(0, new Vector3(tile.x + halfTileSize, 0f, tile.z + halfTileSize), Quaternion.Euler(90f, 0f, 0f), halfTileSize, EventType.Repaint);

                    /*for (int j = 0; j < poteniallyVisibleSetData.currentTilePortalMap.Count; j++)
                    {
                        Portal portal = tile.portalList[j];
                        float halfPortalSize = tile.portalSize / 2;
                        Handles.RectangleHandleCap(0, new Vector3(portal.x + halfPortalSize, 0f, portal.z + halfPortalSize), Quaternion.Euler(90f, 0f, 0f), halfPortalSize, EventType.Repaint);
                    }*/
                }

                Handles.color = Color.green;
                foreach (Rect item in poteniallyVisibleSetData.currentTilePortalMap.Keys)
                {
                    float halfPortalSize = poteniallyVisibleSetData.currentMapTile.portalSize / 2;
                    Handles.RectangleHandleCap(0, new Vector3(item.x + halfPortalSize, 0f, item.y + halfPortalSize), Quaternion.Euler(90f, 0f, 0f), halfPortalSize, EventType.Repaint);
                }
            }
        }
    }
}
