using System;
using System.IO;
using System.IO.Compression;

namespace TheForestWaiter.Content
{
	sealed class ZipContentSource : IContentSource, IDisposable
    {
        private ZipArchive Archive { get; set; }

        public ZipContentSource(ZipArchive zip)
        {
            Archive = zip;
        }

        public byte[] GetBytes(string name)
        {
            using Stream stream = GetStream(name);
            return ReadBytesFromStream(stream);
        }
        
        private static byte[] ReadBytesFromStream(Stream stream)
        {
            //Added this because deflate stream does not have .Length
            using var mStream = new MemoryStream();
            stream.CopyTo(mStream);
            return mStream.ToArray();
        }

        public Stream GetStream(string name)
        {
            var entry = Archive.GetEntry(name);
            return entry.Open();
        }

        public string GetString(string name)
        {
            using Stream stream = GetStream(name);
            StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }

        public void Dispose()
        {
            Archive.Dispose();
        }
    }
}
