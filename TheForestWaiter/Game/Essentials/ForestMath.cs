using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Game.Essentials
{
	public static class ForestMath
	{
		public static float MoveTowardsZero(float value, float by)
		{
			var result = value - by * Math.Sign(value);
			return Math.Sign(result) != Math.Sign(value) ? 0 : result;
		}
	}
}
