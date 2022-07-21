using System;
using TheForestWaiter.Game;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("cameralock", "Locks or unlocks the camera")]
    internal class CameraLock : ICommand
    {
        private readonly Camera _camera;

        public CameraLock(Camera camera)
        {
            _camera = camera;
        }

        public void Execute(CommandHandler handler, string[] args)
        {
            _camera.LockView = !_camera.LockView;
            Console.WriteLine(_camera.LockView ? "Camera locked" : "Camera unlocked");
        }
    }
}
