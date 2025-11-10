using System.Collections.Generic;

namespace Engine.Rendering
{
    /// <summary>
    /// Interface for objects that can be rendered.
    /// </summary>
    public interface IRenderable
    {
        int Layer { get; }
        void Draw();
    }

    /// <summary>
    /// Manages and renders all IRenderable instances in layer order.
    /// </summary>
    public sealed class Renderer
    {
        private readonly List<IRenderable> _renderables = new();
        private bool _needsSort = false;

        public void Add(IRenderable renderable)
        {
            if (renderable != null && !_renderables.Contains(renderable))
            {
                _renderables.Add(renderable);
                _needsSort = true;
            }
        }

        public void Remove(IRenderable renderable)
        {
            _renderables.Remove(renderable);
        }

        public void DrawAll()
        {
            if (_needsSort)
            {
                _renderables.Sort((a, b) => a.Layer.CompareTo(b.Layer));
                _needsSort = false;
            }

            foreach (var renderable in _renderables)
            {
                renderable.Draw();
            }
        }

        public void Clear()
        {
            _renderables.Clear();
        }
    }
}
