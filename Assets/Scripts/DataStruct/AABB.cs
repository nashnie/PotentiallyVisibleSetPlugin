using System.Diagnostics;
using UnityEngine;
//Axis aligned bounding box.
public class AABB : System.IDisposable
{
    public Vector3 minPoint = Vector3.zero;
    public Vector3 maxPoint = Vector3.zero;

    public AABB()
	{
	}

	public AABB(Vector3 pMinPoint, Vector3 pMaxPoint)
	{
        this.minPoint = pMinPoint;
        this.maxPoint = pMaxPoint;

        UnityEngine.Debug.Assert(pMinPoint.x <= pMaxPoint.x);
        UnityEngine.Debug.Assert(pMinPoint.y <= pMaxPoint.y);
        UnityEngine.Debug.Assert(pMinPoint.z <= pMaxPoint.z);
	}

    public void Dispose()
	{
	}

	public Vector3 getCenter()
	{
		return minPoint + (maxPoint - minPoint) * 0.5f;
	}

	public Vector3 getExtents()
	{
		return (maxPoint - minPoint) * 0.5f;
	}

	public Vector3 getSize()
	{
		return maxPoint - minPoint;
	}

	public void getExtremesArrays(float[] min, float[] max)
	{
		min[0] = minPoint.x;
		min[1] = minPoint.y;
		min[2] = minPoint.z;

		max[0] = maxPoint.x;
		max[1] = maxPoint.y;
		max[2] = maxPoint.z;
	}

}
