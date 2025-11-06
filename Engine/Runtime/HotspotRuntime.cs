using Engine.Common;

namespace Engine.Runtime
{
    public sealed class HotspotRuntime
    {
        public string Id { get; }
        public string LabelKey { get; }
        public RectF Rect { get; }
        public string? EventPathId { get; }
        public string? CursorId { get; }
        public bool Highlight { get; }

        public HotspotRuntime(string id, string labelKey, RectF rect, string? eventPathId, string? cursorId, bool highlight)
        {
            Id = id;
            LabelKey = labelKey;
            Rect = rect;
            EventPathId = eventPathId;
            CursorId = cursorId;
            Highlight = highlight;
        }

        public bool Contains(float x, float y)
        {
            return x >= Rect.X && y >= Rect.Y &&
                   x <= Rect.X + Rect.Width &&
                   y <= Rect.Y + Rect.Height;
        }
    }
}
