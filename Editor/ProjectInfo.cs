using System;

namespace Editor
{
    /// <summary>
    /// Represents information about a game project.
    /// </summary>
    public class ProjectInfo
    {
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public DateTime LastOpened { get; set; } = DateTime.Now;

        public override string ToString() => Name;
    }
}
