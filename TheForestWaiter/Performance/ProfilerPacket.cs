namespace TheForestWaiter.Performance;

internal struct ProfilerPacket
{
	public ProfileCategory Category { get; set; }
	public double Microseconds { get; set; }
}
