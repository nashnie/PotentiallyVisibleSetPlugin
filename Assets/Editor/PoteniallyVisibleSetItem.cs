using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoteniallyVisibleSetItem : MonoBehaviour
{
    public MapItemSize size;
    public List<int> ownerCellIdList;
    public MapItemOcclusionType occlusionType = MapItemOcclusionType.Occluder;
}
