using System;

namespace TheForestWaiter.Game.Essentials
{
	public static class MoreMath
	{
		public static float MoveTowardsZero(float value, float by)
		{
			var result = value - by * Math.Sign(value);
			return Math.Sign(result) != Math.Sign(value) ? 0 : result;
		}
	}
}
