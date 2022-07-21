using SFML.Graphics;
using SFML.System;
using System;

namespace TheForestWaiter.Game.Essentials
{
	class Collisions
	{
		//TODO: maybe use something else than SweptAABB
		public static Vector2f RayCast(FloatRect sBox, Vector2f start, Vector2f end)
		{
			var move = end - start;

			var t = SweptAABB(
				sBox,
				new FloatRect(start, default),
				move,
				out _
				);

			return start + move * t;
		}

		// Sources:
		// Parts of this method are from : https://www.gamedev.net/articles/programming/general-and-gameplay-programming/swept-aabb-collision-detection-and-response-r3084/
		public static float SweptAABB(FloatRect sBox, FloatRect mover, Vector2f move, out Vector2f normal)
		{
			Vector2f InvEntry = default;
			Vector2f InvExit = default;

			if (move.X > 0.0f)
			{
				InvEntry.X = sBox.Left - (mover.Left + mover.Width);
				InvExit.X = (sBox.Left + sBox.Width) - mover.Left;
			}
			else
			{
				InvEntry.X = (sBox.Left + sBox.Width) - mover.Left;
				InvExit.X = sBox.Left - (mover.Left + mover.Width);
			}

			if (move.Y > 0.0f)
			{
				InvEntry.Y = sBox.Top - (mover.Top + mover.Height);
				InvExit.Y = (sBox.Top + sBox.Height) - mover.Top;
			}
			else
			{
				InvEntry.Y = (sBox.Top + sBox.Height) - mover.Top;
				InvExit.Y = sBox.Top - (mover.Top + mover.Height);
			}

			Vector2f Entry = default;
			Vector2f Exit = default;

			if (move.X == 0.0f)
			{
				Entry.X = float.NegativeInfinity;
				Exit.X = float.PositiveInfinity;
			}
			else
			{
				Entry.X = InvEntry.X / move.X;
				Exit.X = InvExit.X / move.X;
			}

			if (move.Y == 0.0f)
			{
				Entry.Y = float.NegativeInfinity;
				Exit.Y = float.PositiveInfinity;
			}
			else
			{
				Entry.Y = InvEntry.Y / move.Y;
				Exit.Y = InvExit.Y / move.Y;
			}

			if (Entry.Y > 1.0f) Entry.Y = -float.MaxValue;
			if (Entry.X > 1.0f) Entry.X = -float.MaxValue;

			float entryTime = Math.Max(Entry.X, Entry.Y);
			float exitTime = Math.Min(Exit.X, Exit.Y);
			normal = new Vector2f(0, 0);

			if (entryTime >= exitTime || Entry.X < 0.0f && Entry.Y < 0.0f || Entry.X > 1.0f || Entry.Y > 1.0f)
			{
				return 1.0f;
			}

			if (Entry.X < 0.0f)
			{
				if (sBox.Left + sBox.Width <= mover.Left || sBox.Left >= mover.Left + mover.Width)
					return 1.0f;
			}

			if (Entry.Y < 0.0f)
			{
				if (sBox.Top + sBox.Height <= mover.Top || sBox.Top >= mover.Top + mover.Height)
					return 1.0f;
			}

			if (Entry.X > Entry.Y)
			{
				normal = InvEntry.X < 0.0f ? new Vector2f(1, 0) : new Vector2f(-1, 0);
			}
			else
			{
				normal = InvEntry.Y < 0.0f ? new Vector2f(0, 1) : new Vector2f(0, -1);
			}

			return entryTime;
		}
	}
}
