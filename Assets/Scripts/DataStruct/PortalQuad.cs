//Portal quadrilateral in Counter Clock wise order.
//eg.
//     d ------- c
//		|		| 
//		|		|
//	   a ------- b	

using UnityEngine;
public class PortalQuad : System.IDisposable
{
	public PortalQuad(Vector3[] p_points)
	{
		points[0] = p_points[0];
		points[1] = p_points[1];
		points[2] = p_points[2];
		points[3] = p_points[3];
	}

    public Vector3[] points = new Vector3[4];

	public void Dispose()
	{
	}
}
