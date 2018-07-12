using UnityEngine;
using System;
using System.Collections.Generic;

public class Cell : System.IDisposable
{
    public Vector3 minPoint = Vector3.zero;
    public Vector3 maxPoint = Vector3.zero;

    public Cell(Vector3 p_minPoint, Vector3 p_maxPoint)
	{
        this.minPoint = p_minPoint;
        this.maxPoint = p_maxPoint;
    }

	public void Dispose()
	{
	}

    public List<Portal> getPortals()
	{
		return m_portals;
	}
	public void addPortal(Portal val)
	{
		m_portals.Add(val);
	}

	public List<Cell> getVisibleCells()
	{
		return m_visibleSetCells;
	}
	public void addVisibleCell(Cell val)
	{
		m_visibleSetCells.Add(val);
	}

	public static bool operator == (Cell lhs, Cell rhs)
	{
		return lhs.minPoint == rhs.minPoint && lhs.maxPoint == rhs.maxPoint;
	}
	public static bool operator != (Cell lhs, Cell rhs)
	{
		return lhs.minPoint != rhs.minPoint || lhs.maxPoint != rhs.maxPoint;
	}

	public void addModelId(int modelId)
	{
		m_modelIds.Add(modelId);
	}

	public void getModelsIds(List<int> modelIds)
	{
        modelIds.AddRange(m_modelIds);
    }

	public bool isModelIdInCell(int modelId)
	{
        for (int i = 0; i < m_modelIds.Count; i++)
        {
            if (m_modelIds[i] == modelId)
            {
                return true;
            }
        }
        return false;
    }


	private List<int> m_modelIds = new List<int>(); //The id's of the models inside the cell

	private List<Portal> m_portals = new List<Portal>(); //The portals that connect to other Cells.
	private List<Cell> m_visibleSetCells = new List<Cell>(); //The Cells that are potentially visible from the current Cell

}


