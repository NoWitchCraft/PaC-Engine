using System.Collections.Generic;
using Engine.Common;
using Engine.Data.Scene;

namespace Engine.Data.Scene
{
    ///<summary>
    /// Minimale Szenendefinition: Hintergrund, Startposition, Hotspots.
    /// </summary>
    public class SceneDTO
    {
        public string Id { get; set; } = "";
        public string BackgroundPath { get; set; } = "";
        public Vector2 StartPosition { get; set; } = new Vector2(0, 0);
        public string? WalkPath { get; set; }

        public List<HotspotDTO> Hotspots { get; set; } = new List<HotspotDTO>();
        public string? MusicCueId { get; set; }
        public List<string>? AmbientCueIds { get; set; }
    }
}