using System.Text;

namespace Engine.Utils
{
    public static class PathUtil
    {
        public static string NormalizeContentPath(string p)
            => p.Replace('\\','/').TrimStart('/');

        public static string WithExtension(string path, string extWithDot)
        {
            int i = path.LastIndexOf('.');
            return (i >= 0 ? path[..i] : path) + extWithDot;
        }

        public static string MakeSafeFileName(string name)
        {
            var invalid = System.IO.Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(name.Length);
            foreach (var ch in name)
                sb.Append(System.Array.IndexOf(invalid, ch) >= 0 ? '_' : ch);
            return sb.ToString();
        }
    }
}
