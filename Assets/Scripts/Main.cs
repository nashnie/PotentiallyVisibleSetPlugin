﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public Player player;
    private PoteniallyVisibleSetItem[] poteniallyVisibleSetItems;

    void Start ()
    {
        PoteniallyVisibleSetData.Instance.InitializePVSData();
        GameObject occlusionCullingRoot = GameObject.Find("OcclusionCulling");
        poteniallyVisibleSetItems = occlusionCullingRoot.GetComponentsInChildren<PoteniallyVisibleSetItem>();
    }

    private void Update()
    {
        PoteniallyVisibleSetData.Instance.Update(player);
        for (int i = 0; i < poteniallyVisibleSetItems.Length; i++)
        {
            PoteniallyVisibleSetItem pvsItem = poteniallyVisibleSetItems[i];
            bool isVisible = false;
            for (int j = 0; j < pvsItem.ownerCellIdList.Count; j++)
            {
                int cellId = pvsItem.ownerCellIdList[j];
                if(PoteniallyVisibleSetData.Instance.CheckVisible(cellId))
                {
                    isVisible = true;
                    break;
                }
            }
            pvsItem.SetVisivle(isVisible);
        }
    }
}
