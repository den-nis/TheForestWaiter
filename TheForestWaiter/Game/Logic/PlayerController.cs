using SFML.System;
using System;
using System.Diagnostics;
using System.Linq;
using TheForestWaiter.Game.Constants;

namespace TheForestWaiter.Game.Logic
{
	internal class PlayerController 
	{
		private float _aim;

		private readonly bool[] _binaryActions;
		private static readonly bool[] _binaryActionsTemplate;

		static PlayerController()
		{
			var values = (ActionTypes[])Enum.GetValues(typeof(ActionTypes));
			_binaryActionsTemplate = new bool[values.Length];

			Debug.Assert(values.SequenceEqual(Enumerable.Range(0, values.Length).Cast<ActionTypes>()), "Unexpected/incompatible enum values");
		}

		public PlayerController()
		{
			_binaryActions = new bool[_binaryActionsTemplate.Length];
			Array.Copy(_binaryActionsTemplate, _binaryActions, _binaryActions.Length);
		}

		public void ToggleOn(ActionTypes action)
		{
			_binaryActions[(int)action] = true;
		}

		public void ToggleOff(ActionTypes action)
		{
			_binaryActions[(int)action] = false;
		}

		public void Aim(float radians)
		{
			_aim = radians;
		}

		public float GetAim() => _aim;

		public bool IsActive(ActionTypes action)
		{
			return _binaryActions[(int)action];
		}
	}
}
