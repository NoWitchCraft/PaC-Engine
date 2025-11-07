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
                // defensiv: falls Rect null war, ersetze mit 0-Rect
                var rect = hs.Rect ?? new RectF { X = 0, Y = 0, Width = 0, Height = 0 };
                list.Add(new HotspotRuntime(
                    id: hs.Id ?? "",
                    labelKey: hs.LabelKey ?? "",
                    rect: rect,
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
