using System;
using Engine.Config;
using Engine.Core;
using Engine.IO;

namespace Engine.Content
{
    public sealed class ContentResolver : IContentResolver
    {
        private readonly IFileSystem _fs;

        public string ContentRootAbsolute { get; }

        public ContentResolver(IFileSystem fs, string? contentRootFromSettings = null)
        {
            _fs = fs ?? throw new ArgumentNullException(nameof(fs));

            // Basis ist das Ausgabeverzeichnis (neben EXE)
            var baseDir = AppContext.BaseDirectory;
            var rootFromSettings = (contentRootFromSettings ?? Settings.Current.ContentRoot ?? "Content").TrimEnd('/', '\\');

            ContentRootAbsolute = _fs.GetFullPath(_fs.Combine(baseDir, rootFromSettings));
        }

        public string ResolveContentPath(string relative)
        {
            relative = relative.Replace('\\', '/').TrimStart('/');
            return _fs.GetFullPath(_fs.Combine(ContentRootAbsolute, relative));
        }

        public string ToContentRelative(string absolute)
        {
            var full = _fs.GetFullPath(absolute);
            var root = ContentRootAbsolute.TrimEnd('\\', '/');
            if (full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                var rel = full.Substring(root.Length).TrimStart('\\', '/');
                return rel.Replace('\\', '/');
            }
            return absolute;
        }
    }
}
