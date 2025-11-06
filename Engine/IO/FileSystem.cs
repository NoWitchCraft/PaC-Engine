using System;
using System.IO;

namespace Engine.IO
{
    public sealed class FileSystem : IFileSystem
    {
        public bool FileExists(string path) => File.Exists(path);
        public bool DirectoryExists(string path) => Directory.Exists(path);

        public string ReadAllText(string path) => File.ReadAllText(path);
        public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);
        public Stream OpenRead(string path) => File.OpenRead(path);

        public void WriteAllText(string path, string contents)
        {
            var dir = GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(path, contents);
        }

        public void CreateDirectory(string path) => Directory.CreateDirectory(path);

        public string Combine(params string[] parts) => Path.Combine(parts);
        public string GetFullPath(string path) => Path.GetFullPath(path);
        public string GetDirectoryName(string path) => Path.GetDirectoryName(path) ?? "";
    }
}
