public class Voxel : System.IDisposable
{
	public enum VoxelStatus
	{
		EMPTY = 0,
		SOLID
	}

	public Voxel()
	{
		status = VoxelStatus.EMPTY;
	}

	public Voxel(VoxelStatus voxelStatus)
	{
		status = voxelStatus;
	}

	public void Dispose()
	{
	}

	public VoxelStatus getStatus()
	{
		return status;
	}
	public void setStatus(VoxelStatus val)
	{
		status = val;
	}

	private VoxelStatus status;
}
