using System.Collections.Generic;

namespace Engine.Runtime
{
    public sealed class SceneRuntime
    {
        public string Id { get; }
        public string BackgroundPath { get; }
        public IReadOnlyList<HotspotRuntime> Hotspots { get; }

        public string? MusicCueId { get; }
        public IReadOnlyList<string>? AmbientCueIds { get; }

        public SceneRuntime(string id, string backgroundPath,
                            IReadOnlyList<HotspotRuntime> hotspots,
                            string? musicCueId,
                            IReadOnlyList<string>? ambientCueIds)
        {
            Id = id;
            BackgroundPath = backgroundPath;
            Hotspots = hotspots;
            MusicCueId = musicCueId;
            AmbientCueIds = ambientCueIds;
        }

        public HotspotRuntime? FindHotspotAt(float x, float y)
        {
            // simple hit-test; later we can add z-order or flags
            for (int i = Hotspots.Count - 1; i >= 0; i--)
                if (Hotspots[i].Contains(x, y)) return Hotspots[i];
            return null;
        }

        public HotspotRuntime? GetHotspot(string id)
        {
            foreach (var h in Hotspots)
                if (h.Id == id) return h;
            return null;
        }
    }
}
