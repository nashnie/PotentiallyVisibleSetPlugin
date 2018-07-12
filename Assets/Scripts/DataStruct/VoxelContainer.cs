//Contains voxels inside a Axis aligned box.
//The container holds voxelCountX, voxelsCountY and VoxelsCountZ number of voxels of size voxelSize.

using UnityEngine;
using System;
using System.Collections.Generic;

public class VoxelContainer : System.IDisposable
{

	public enum NEIGHBOUR_DIRECTION
	{
		POSITIVE_X,
		NEGATIVE_X,
		POSITIVE_Y,
		NEGATIVE_Y,
		POSITIVE_Z,
		NEGATIVE_Z
	}

	public VoxelContainer(Vector3 voxelSize, int voxelCountX, int voxelCountY, int voxelCountZ)
	{
		if (voxelSize.x <= 0.0f || voxelSize.y <= 0.0f || voxelSize.z <= 0.0f)
		{
			throw new System.Exception("Invalid voxel size.");
		}

		if (voxelCountX <= 0 || voxelCountY <= 0 || voxelCountZ <= 0)
		{
            throw new System.Exception("Invalid voxel count.");
		}

        m_voxelSize = new Vector3(voxelSize.x, voxelSize.y, voxelSize.z);

		m_voxelCounts.x = voxelCountX;
		m_voxelCounts.y = voxelCountY;
		m_voxelCounts.z = voxelCountZ;
	}

	public virtual void Dispose()
	{
	}

    private void CopyFrom(Vector3 from, Vector3 to)
    {
        to.x = from.x;
        to.y = from.y;
        to.z = from.z;
    }

    //TODO: Change for something more optimized
    public List<AABB> getAllVoxelAABBFromVolume(AABB objectBounds, AABB worldBounds)
	{
        Vector3 fromVoxelPos = Vector3.zero;
        Vector3 toVoxelPos = Vector3.zero;

        CopyFrom(getVoxelIndexFromPoint(objectBounds.minPoint, worldBounds), fromVoxelPos);
        CopyFrom(getVoxelIndexFromPoint(objectBounds.maxPoint, worldBounds), toVoxelPos);

        List<AABB> voxelsAABB = new List<AABB>();

        for (int z = (int)fromVoxelPos.z ; z <= toVoxelPos.z; z++)
		{
			for (int y = (int)fromVoxelPos.y ; y <= toVoxelPos.y; y++)
			{
				for (int x = (int)fromVoxelPos.x ; x <= toVoxelPos.x; x++)
				{
					voxelsAABB.Add(getVoxelAABBFromIndex(new Vector3(x, y, z), worldBounds));
				}
			}
		}

		return voxelsAABB;
	}

	public List<Vector3> getAllVoxelPointsInRange(Vector3 minIndex, Vector3 maxIndex)
	{
		if (isPositionOutOfBounds(minIndex) || isPositionOutOfBounds(maxIndex))
		{
			throw new Exception("Index out of bounds.");
        }

		if (minIndex.x > maxIndex.x || minIndex.y > maxIndex.y || minIndex.z > maxIndex.z)
		{
			throw new Exception("Invalid Range");
		}

		List<Vector3> voxelsAABB = new List<Vector3>();

		for (int z = (int)minIndex.z ; z <= maxIndex.z; z++)
		{
			for (int y = (int)minIndex.y ; y <= maxIndex.y; y++)
			{
				for (int x = (int)minIndex.x ; x <= maxIndex.x; x++)
				{
					voxelsAABB.Add(new Vector3(x, y, z));
				}
			}
		}
		return voxelsAABB;
	}

	public Voxel voxelAt(Vector3 voxelIndex)
	{
		//Check valid index.
		if (isPositionOutOfBounds(voxelIndex))
		{
			throw new Exception("Invalid voxel position.");
		}
        if (m_voxels.ContainsKey(voxelIndex))
        {
            return new Voxel(Voxel.VoxelStatus.SOLID);
        }
        return new Voxel(Voxel.VoxelStatus.EMPTY);
	}

	//Adds a new solid voxel at a given position in the container.
	public void addVoxelAt(Vector3 voxelIndex)
	{
		//Check valid position.
		if (isPositionOutOfBounds(voxelIndex))
		{
			throw new Exception("Invalid voxel index.");
		}

        //add if not exists.
        if (m_voxels.ContainsKey(voxelIndex) == false)
        {
            m_voxels.Add(voxelIndex);
        }
	}

	public AABB getVoxelAABBFromIndex(Vector3 voxelIndex, AABB worldBounds)
	{
		if (isPositionOutOfBounds(voxelIndex))
		{
			throw new System.Exception("Voxel index out of bounds.");
		}

		Vector3 fromPos = new Vector3();
        Vector3 toPos = new Vector3();

		fromPos.x = voxelIndex.x * m_voxelSize.x + worldBounds.minPoint.x;
		fromPos.y = voxelIndex.y * m_voxelSize.y + worldBounds.minPoint.y;
		fromPos.z = voxelIndex.z * m_voxelSize.z + worldBounds.minPoint.z;

        CopyFrom(fromPos + m_voxelSize, toPos);

        return new AABB(new Vector3(fromPos.x, fromPos.y, fromPos.z), new Vector3(toPos.x, toPos.y, toPos.z));
	}

	public Vector3 getVoxelPositionFromIndex(Vector3 voxelIndex, AABB worldBounds)
	{
		if (isPositionOutOfBounds(voxelIndex))
		{
			throw new Exception("Voxel index out of bounds.");
		}

        Vector3 min = new Vector3(voxelIndex.x * m_voxelSize.x + worldBounds.minPoint.x, voxelIndex.y * m_voxelSize.y + worldBounds.minPoint.y, voxelIndex.z * m_voxelSize.z + worldBounds.minPoint.z);
		min += new Vector3(m_voxelSize.x / 2, m_voxelSize.y / 2, m_voxelSize.z / 2);
		return min;
	}

	public Vector3 getVoxelIndexFromPoint(Vector3 pointPosition, AABB worldBounds)
	{
        Vector3 voxelIndex = Vector3.zero;

		voxelIndex.x = (int)((pointPosition.x - worldBounds.minPoint.x) / m_voxelSize.x);
		voxelIndex.y = (int)((pointPosition.y - worldBounds.minPoint.y) / m_voxelSize.y);
		voxelIndex.z = (int)((pointPosition.z - worldBounds.minPoint.z) / m_voxelSize.z);

		//Check valid position.
		if (isPositionOutOfBounds(voxelIndex))
		{
			throw new System.Exception("Point out of voxel container.");
		}

		return voxelIndex;
	}

	public Vector3 getVoxelSize()
	{
		return m_voxelSize;
	}

	public Vector3 getVoxelCounts()
	{
		return m_voxelCounts;
	}

	public uint getTotalNumberOfVoxelsInContainer()
	{
		return (uint)(m_voxelCounts.x * m_voxelCounts.y * m_voxelCounts.z);
	}

	public List<Vector3> getAllSolidVoxels()
	{
		List<Vector3> indices = new List<Vector3>();
		if (m_voxels.Count <= 0)
		{
			return indices;
		}
        foreach (var item in m_voxels.Keys)
        {
            indices.Add(item);
        }
		return indices;
	}

	public uint getAllSolidVoxelsCount()
	{
        return (uint)m_voxels.Count;
	}

	public List<Vector3> getAllSolidVoxelPositionsInWorldSpace(AABB worldBounds)
	{
        List<Vector3> points = new List<Vector3>();
		if (getAllSolidVoxelsCount() <= 0)
		{
			return points;
		}
        foreach (var item in m_voxels.Keys)
        {
            points.Add(getVoxelPositionFromIndex(item, worldBounds));
        }
        return points;
	}

	public bool isPositionOutOfBounds(Vector3 voxelPosition)
	{
		return (voxelPosition.x < 0 || voxelPosition.y < 0 || voxelPosition.z < 0 || voxelPosition.x >= m_voxelCounts.x || voxelPosition.y >= m_voxelCounts.y || voxelPosition.z >= m_voxelCounts.z);
	}

	private Vector3 m_voxelCounts = Vector3.zero; //The number of voxels in x, y and z.
	private Vector3 m_voxelSize = Vector3.zero; //The size of the voxel in x, y and z.

	private Set<Vector3> m_voxels = new Set<Vector3>(); //TODO: Replace with a hierarchical structure like SVO

	private Vector3 getNeighbourVoxel(Vector3 voxelIndex, NEIGHBOUR_DIRECTION dir)
	{
		Vector3 newPos = new Vector3(voxelIndex.x, voxelIndex.y, voxelIndex.z);

		switch (dir)
		{
		    case NEIGHBOUR_DIRECTION.POSITIVE_X:
			    newPos.x += 1;
			    break;
		    case NEIGHBOUR_DIRECTION.NEGATIVE_X:
			    newPos.x -= 1;
			    break;
		    case NEIGHBOUR_DIRECTION.POSITIVE_Y:
			    newPos.y += 1;
			    break;
		    case NEIGHBOUR_DIRECTION.NEGATIVE_Y:
			    newPos.y -= 1;
			    break;
		    case NEIGHBOUR_DIRECTION.POSITIVE_Z:
			    newPos.z += 1;
			    break;
		    case NEIGHBOUR_DIRECTION.NEGATIVE_Z:
			    newPos.z -= 1;
			    break;
		}

		return newPos;
	}

	private void exploreNeighbourVoxel(Vector3 currentVoxelIndex, NEIGHBOUR_DIRECTION direction, Set<Vector3> exploredVoxels, Queue<Vector3> voxelQueue)
	{
		//Get near voxel depending on direction.
		Vector3 neighbourPoint = getNeighbourVoxel(currentVoxelIndex, direction);

		//Check if it is a voxel within bounds.
		if (isPositionOutOfBounds(neighbourPoint) == false)
		{
			//Add to the queue to explore it later.
			if (exploredVoxels.ContainsKey(neighbourPoint) == false)
			{
				exploredVoxels.Add(neighbourPoint);
				voxelQueue.Enqueue(neighbourPoint);
			}
		}
	}
}

public class Set<T> : SortedDictionary<T, bool>
{
    public void Add(T item)
    {
        this.Add(item, true);
    }
}