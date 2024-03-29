﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
	class ContentBuilder
	{
		private const char PreferredDirectorySeparator = '/';
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
				string entry = GetEntry(file);
				var info = contentConfig.TryGetByPath(entry);
				AddFile(contentZip, file, info);
			}

			AddConfigFile(contentZip, files, contentConfig);
		}

		private ContentConfig GetConfig()
		{
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
			foreach (var file in files)
			{
				var entryName = ConvertPath(GetEntry(file));
				if (!config.HasFile(entryName))
				{
					config.Content.Add(new ContentMeta
					{
						Path = entryName,
						Type = ContentTypeDefinitions.FromFilename(entryName),
						Compression = DEFAULT_COMPRESSION,
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

		private string GetEntry(string file)
		{
			return ConvertPath(GetRelativePath(BasePath, file));
		}

		private static string ConvertPath(string path)
		{
			return path.Replace(Path.DirectorySeparatorChar, PreferredDirectorySeparator);
		}

		private static string GetRelativePath(string absoluteBasePath, string absoluteSubPath)
		{
			if (!absoluteBasePath.EndsWith(Path.DirectorySeparatorChar))
				absoluteBasePath += Path.DirectorySeparatorChar;

			if (absoluteSubPath.StartsWith(absoluteBasePath))
			{
				var relative = absoluteSubPath[(absoluteBasePath.TrimEnd(Path.DirectorySeparatorChar).Length + 1)..];
				return relative;
			}
			else
			{
				throw new ArgumentException("Sub path not in same directory as base path");
			}
		}
	}
}
