using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
	interface IPreprocessor
	{
		byte[] Process(byte[] input, ContentMeta meta);
	}
}
