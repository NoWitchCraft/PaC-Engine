using System.Collections.Generic;

namespace Engine.Runtime
{
    public sealed class GlobalState
    {
        public Dictionary<string, int> Vars { get; } = new();
        public Dictionary<string, bool> Switches { get; } = new();
    }
}
