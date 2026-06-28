using ODOL6.Common.Math;

namespace ODOL6.Model.ODOL;

public abstract class STPair
{
	public Vector3P S { get; } = new Vector3P();

	public Vector3P T { get; } = new Vector3P();
}
