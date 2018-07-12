using UnityEngine;

public class Portal : System.IDisposable
{
    public Cell fromCell;
    public Cell toCell;

    public Portal(Cell p_fromCell, Cell p_toCell, Vector3 p_minPoint, Vector3 p_maxPoint, Vector3 p_facingPlane)
	{
		this.fromCell = p_fromCell;
		this.toCell = p_toCell;
        this.minPoint = p_minPoint;
        this.maxPoint = p_maxPoint;

        this.facingPlane = p_facingPlane;
    }

	public void Dispose()
	{
	}

	public Vector3 minPoint = Vector3.zero; //The voxels shared between cells.
	public Vector3 maxPoint = Vector3.zero;

	public Vector3 facingPlane = Vector3.zero; //The plane where it points at. eg. (1,0,0) YZ plane pointing to +X.


}
