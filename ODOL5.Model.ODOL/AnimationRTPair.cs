namespace ODOL5.Model.ODOL;

public class AnimationRTPair
{
	public byte SelectionIndex { get; }

	public byte Weight { get; }

	public AnimationRTPair(byte sel, byte weight)
	{
		SelectionIndex = sel;
		Weight = weight;
	}
}
