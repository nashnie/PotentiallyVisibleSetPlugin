using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PoteniallyVisibleSetSettings", menuName = "Configs/PoteniallyVisibleSetSettings")]
public class PoteniallyVisibleSetSettings : ScriptableObject
{
    [Tooltip("Tile Size")]
    public Vector3 tileSize = new Vector3(256, 0, 256);
    [Tooltip("大物件 Cell Size")]
    public Vector3 bigCellSize = new Vector3(128, 0, 128);
    [Tooltip("中物件 Cell Size")]
    public Vector3 middleCellSize = new Vector3(64, 0, 64);
    [Tooltip("小物件 Cell Size")]
    public Vector3 smallCellSize = new Vector3(32, 0, 32);

    [Tooltip("地图尺寸大小")]
    public Vector3 mapSize = new Vector3(1024, 0, 1024);
    [Tooltip("视口尺寸大小")]
    public Vector3 portalSize = new Vector3(32, 0, 32);
    [Tooltip("蒙特卡洛方法，随机起点数量")]
    [Range(1, 32)]
    public int startPortalPointCount = 16;
    [Tooltip("视野距离")]
    [Range(64, 1024)]
    public int visibleRange = 365;//Mathf.sqrt(tileSize * tileSize + tileSize * tileSize);
    [Tooltip("蒙特卡洛方法，随机终点数量，分大>>>中>>>小>>>最小值")]
    public Vector4 endPortalPointList = new Vector4(32, 16, 8, 4);
    [Tooltip("射线检测高度，可以分两层，或者三层，层数越多越精确，烘焙消耗时间也越长...")]
    public List<float> verticalSize = new List<float> { 1f, 3f };
}
