using Engine.Common;

namespace Engine.Rendering
{
    /// <summary>
    /// 2D-Kamera mit Top-Left-Position (Weltkoordinaten), Zoom und optionalen Bounds.
    /// Nutze Screen<->World für korrektes Hit-Testing und Zeichnen via raylib BeginMode2D (in C2).
    /// </summary>
    public sealed class Camera2D
    {
        // Welt-TopLeft der Kamera (was oben links im Viewport sichtbar ist)
        public Vector2 Position { get; private set; } = new Vector2(0, 0);

        /// <summary>Zoom-Faktor (1 = 100%). Werte &lt;= 0 werden auf einen minimalen Wert geklemmt.</summary>
        public float Zoom
        {
            get => _zoom;
            set => _zoom = value <= 0.001f ? 0.001f : value;
        }
        private float _zoom = 1f;

        /// <summary>Viewportgröße in Pixeln (Fenstergröße bzw. Render-Target).</summary>
        public int ViewportWidth  { get; private set; } = 1280;
        public int ViewportHeight { get; private set; } = 720;

        /// <summary>Optionale Weltbegrenzung (Scene-Bounds). Null = keine Begrenzung.</summary>
        public RectF? WorldBounds { get; private set; }

        public Camera2D() { }

        public Camera2D(int viewportWidth, int viewportHeight)
        {
            ViewportWidth = viewportWidth;
            ViewportHeight = viewportHeight;
        }

        /// <summary>Viewport aktualisieren (z. B. bei Window-Resize).</summary>
        public void SetViewport(int width, int height)
        {
            ViewportWidth = width;
            ViewportHeight = height;
            ClampToBounds();
        }

        /// <summary>Optionale Weltbegrenzung setzen/entfernen.</summary>
        public void SetBounds(RectF? worldBounds)
        {
            WorldBounds = worldBounds;
            ClampToBounds();
        }

        /// <summary>Bewegt die Kamera relativ.</summary>
        public void MoveBy(float dx, float dy)
        {
            Position = new Vector2(Position.X + dx, Position.Y + dy);
            ClampToBounds();
        }

        /// <summary>Setzt die Kamera-Position absolut (Top-Left).</summary>
        public void SetPosition(float x, float y)
        {
            Position = new Vector2(x, y);
            ClampToBounds();
        }

        /// <summary>Zentriert die Kamera so, dass der gegebene Weltpunkt in der Mitte des Viewports liegt.</summary>
        public void CenterOn(float worldX, float worldY)
        {
            var halfW = (ViewportWidth  / Zoom) * 0.5f;
            var halfH = (ViewportHeight / Zoom) * 0.5f;
            Position = new Vector2(worldX - halfW, worldY - halfH);
            ClampToBounds();
        }

        /// <summary>Welt → Screen (Pixel, Viewport-TopLeft ist (0,0)).</summary>
        public Vector2 WorldToScreen(float worldX, float worldY)
        {
            var sx = (worldX - Position.X) * Zoom;
            var sy = (worldY - Position.Y) * Zoom;
            return new Vector2(sx, sy);
        }

        /// <summary>Screen (Pixel) → Weltkoordinaten.</summary>
        public Vector2 ScreenToWorld(float screenX, float screenY)
        {
            var wx = screenX / Zoom + Position.X;
            var wy = screenY / Zoom + Position.Y;
            return new Vector2(wx, wy);
        }

        /// <summary>Klemmt die Kamera-Position an WorldBounds (falls gesetzt).</summary>
        public void ClampToBounds()
        {
            if (WorldBounds == null) return;

            var vbWidth  = ViewportWidth  / Zoom;
            var vbHeight = ViewportHeight / Zoom;

            var b = WorldBounds.Value;

            // Wenn Bounds kleiner als Viewport sichtbar sind, zentrieren wir innerhalb der Bounds
            if (b.Width <= vbWidth)
            {
                Position = new Vector2(b.X + (b.Width - vbWidth) * 0.5f, Position.Y);
            }
            else
            {
                var minX = b.X;
                var maxX = b.X + b.Width - vbWidth;
                if (Position.X < minX) Position = new Vector2(minX, Position.Y);
                else if (Position.X > maxX) Position = new Vector2(maxX, Position.Y);
            }

            if (b.Height <= vbHeight)
            {
                Position = new Vector2(Position.X, b.Y + (b.Height - vbHeight) * 0.5f);
            }
            else
            {
                var minY = b.Y;
                var maxY = b.Y + b.Height - vbHeight;
                if (Position.Y < minY) Position = new Vector2(Position.X, minY);
                else if (Position.Y > maxY) Position = new Vector2(Position.X, maxY);
            }
        }
    }
}
