using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        // I only added this because deflate stream does not support .Length...
        private static byte[] ReadBytesFromStream(Stream stream)
        {
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
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public void Dispose()
        {
            Archive.Dispose();
        }
    }
}
