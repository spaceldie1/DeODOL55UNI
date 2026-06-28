namespace ODOL5.Model.ODOL;

public class AnimationRTWeight : VerySmallArray
{
	public AnimationRTPair[] AnimationRTPairs
	{
		get
		{
			AnimationRTPair[] array = new AnimationRTPair[nSmall];
			for (int i = 0; i < nSmall; i++)
			{
				array[i] = new AnimationRTPair(smallSpace[i * 2], smallSpace[i * 2 + 1]);
			}
			return array;
		}
	}
}
