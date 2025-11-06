using System;
using System.IO;

namespace Engine.IO
{
    public interface IFileSystem
    {
        bool FileExists(string path);
        bool DirectoryExists(string path);

        string ReadAllText(string path);
        byte[] ReadAllBytes(string path);
        Stream OpenRead(string path);

        void WriteAllText(string path, string contents);
        void CreateDirectory(string path);

        string Combine(params string[] parts);
        string GetFullPath(string path);
        string GetDirectoryName(string path);
    }
}
