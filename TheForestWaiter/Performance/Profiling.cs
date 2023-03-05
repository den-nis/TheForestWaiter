using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TheForestWaiter.Performance;

internal class Profiling
{
	private class Capture
	{
		private readonly Stopwatch _timer = new();

		public double Time { get; private set; } = double.NaN;

		public void Start()
		{
			_timer.Restart();
		}

		public void End()
		{
			_timer.Stop();
			Time = _timer.Elapsed.TotalMilliseconds * 1000.0;
		}

		public void Reset() => Time = double.NaN;
	}

#pragma warning disable CS0649
	private static readonly Capture[] _captures;
	private static readonly ConcurrentQueue<ProfilerPacket[]> _stream;
#pragma warning restore CS0649

	private static Profiler _profiler;

	static Profiling()
	{
#if PROFILE
		_stream = new();
		_captures = Enum.GetValues<ProfileCategory>()
			.Select(t => new Capture())
			.ToArray();
#endif
	}

	[Conditional("PROFILE")]
	public static void Start(ProfileCategory type)
	{
		_captures[(int)type].Start();
	}

	[Conditional("PROFILE")]
	public static void End(ProfileCategory type)
	{
		_captures[(int)type].End();
	}

	[Conditional("PROFILE")]
	public static void SendDataToProfiler()
	{
		ProfilerPacket[] packets = new ProfilerPacket[_captures.Length];
		for (int i = 0; i < _captures.Length; i++)
		{
			packets[i] = new ProfilerPacket
			{
				Category = (ProfileCategory)i,
				Microseconds = _captures[i].Time
			};

			_captures[i].Reset();
		}

		_stream.Enqueue(packets);
	}

	[Conditional("PROFILE")]
	public static void CreateProfiler()
	{
		_profiler = new Profiler(_stream);

		Task.Factory
			.StartNew((profiler) => (profiler as Profiler).Start(), _profiler, TaskCreationOptions.LongRunning)
			.ConfigureAwait(false);
	}
}
