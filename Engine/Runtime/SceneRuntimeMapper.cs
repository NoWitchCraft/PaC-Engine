using System.Collections.Generic;
using Engine.Data.Scene;
using Engine.Common;

namespace Engine.Runtime
{
    public static class SceneRuntimeMapper
    {
        public static SceneRuntime FromDTO(SceneDTO dto)
        {
            var list = new List<HotspotRuntime>(dto.Hotspots.Count);
            foreach (var hs in dto.Hotspots)
            {
                list.Add(new HotspotRuntime(
                    id: hs.Id ?? "",
                    labelKey: hs.LabelKey ?? "",
                    rect: hs.Rect,
                    eventPathId: hs.EventPathId,
                    cursorId: hs.CursorId,
                    highlight: hs.Highlight
                ));
            }

            return new SceneRuntime(
                id: dto.Id ?? "",
                backgroundPath: dto.BackgroundPath ?? "",
                hotspots: list,
                musicCueId: dto.MusicCueId,
                ambientCueIds: dto.AmbientCueIds
            );
        }
    }
}
