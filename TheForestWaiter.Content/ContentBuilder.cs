using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
    class ContentBuilder
    {
        private const CompressionLevel DEFAULT_COMPRESSION = CompressionLevel.NoCompression;

        private string BasePath { get; set; }

        public ContentBuilder(string basePath)
        {
            BasePath = basePath;
        }

        public void Build(string output)
        {
            var contentConfig = GetConfig();

            using var fileStream = File.Create(output);
            using ZipArchive contentZip = new(fileStream, ZipArchiveMode.Create);

            string[] files = Directory.GetFiles(BasePath, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                Console.WriteLine($"Reading \"{file}\"");
                var relative = GetRelativePath(BasePath, file);
                contentConfig.Content.TryGetValue(relative, out ContentMeta info);
                AddFile(contentZip, file, info);
            }

            AddConfigFile(contentZip, files, contentConfig);
        }

        private ContentConfig GetConfig()
        {
            if (
                ContentSettings.CONTENT_CONFIG_ENTRY.Contains(Path.DirectorySeparatorChar) || 
                ContentSettings.CONTENT_CONFIG_ENTRY.Contains(Path.AltDirectorySeparatorChar)
                )
            {
                throw new InvalidOperationException("Content config entry name contains directory separator");
            }

            var path = Path.Join(BasePath, ContentSettings.CONTENT_CONFIG_ENTRY);
            return JsonConvert.DeserializeObject<ContentConfig>(File.ReadAllText(path));
        }

        private void AddFile(ZipArchive archive, string file, ContentMeta info)
        {
            var entryName = GetEntry(file);

            if (entryName == ContentSettings.CONTENT_CONFIG_ENTRY)
                return; //Exception for content config (else it will be written two times)

            byte[] rawContent = File.ReadAllBytes(file);
            CompressionLevel level = DEFAULT_COMPRESSION;

            if (info != null)
            {
                var pre = PreprocessorFactory.GetPreprocessor(info.Type);
                rawContent = pre?.Process(rawContent, info) ?? rawContent;

                if (level != info.Compression)
                {
                    Console.WriteLine($"Compression : {info.Compression}");
                    level = info.Compression;
                }
            }

            var entry = archive.CreateEntry(entryName, level);
            using var entryStream = entry.Open();

            entryStream.Write(rawContent, 0, rawContent.Length);
            entryStream.Flush();
            Console.WriteLine($"Saved to \"{entryName}\"");
        }

        private void AddConfigFile(ZipArchive contentZip, string[] files, ContentConfig config)
        {
            //Add missing content meta
            foreach(var file in files)
            {
                var entryName = GetEntry(file);
                if (!config.Content.ContainsKey(entryName))
                {
                    config.Content.Add(entryName, new ContentMeta
                    {
                        Compression = DEFAULT_COMPRESSION,
                        Type = ContentTypeDefinitions.FromFilename(entryName)
                    });
                    Console.WriteLine($"Added default content meta for {entryName}");
                }
            }

            var entry = contentZip.CreateEntry(ContentSettings.CONTENT_CONFIG_ENTRY, CompressionLevel.Optimal);
            using var stream = entry.Open();
            using StreamWriter writer = new(stream);
            writer.Write(JsonConvert.SerializeObject(config, ContentConfig.SerializerSettings));
            writer.Flush();
        }

        private string GetEntry(string file) => GetRelativePath(BasePath, file);

        private static string GetRelativePath(string absoluteBase, string absoluteSub)
        {
            if (!absoluteBase.EndsWith(Path.DirectorySeparatorChar))
                absoluteBase += Path.DirectorySeparatorChar;

            if (absoluteSub.StartsWith(absoluteBase))
            {
                var relative = absoluteSub[(absoluteBase.TrimEnd(Path.DirectorySeparatorChar).Length+1)..];
                return relative;
            }
            else
            {
                throw new ArgumentException("Sub path not in same directory as base path");
            }
        }
    }
}
