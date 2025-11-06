using Engine.Common;

namespace Engine.Data.Scene
{
    /// <summary>
    /// Interaktives Objekt in einer Szene
    /// </summary>
    public class HotspotDTO
    {
        public string Id { get; set; } = "";
        public string LabelKey { get; set; } = ""; // für Lokalisierung
        public RectF Rect { get; set; } = new RectF();
        public string? EventPathId { get; set; }        // verweist auf Sequenz/Events (später)
        public string? CursorId { get; set; }           // optionaler Cursor
        public bool Highlight { get; set; } = true;     // optionale Hervorhebung
    }
}