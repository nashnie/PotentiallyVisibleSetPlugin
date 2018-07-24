using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoteniallyVisibleSetItem : MonoBehaviour
{
    [SerializeField]
    public MapItemSize size;
    [SerializeField]
    public List<int> ownerCellIdList = new List<int>();
    [SerializeField]
    public MapItemOcclusionType occlusionType = MapItemOcclusionType.Occluder;
}
