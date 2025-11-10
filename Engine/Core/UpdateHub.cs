using System.Collections.Generic;

namespace Engine.Core
{
    /// <summary>
    /// Interface for objects that need to be updated each frame.
    /// </summary>
    public interface IUpdatable
    {
        void Update(float dt);
    }

    /// <summary>
    /// Manages and updates all IUpdatable instances.
    /// </summary>
    public sealed class UpdateHub
    {
        private readonly List<IUpdatable> _updateables = new();

        public void Add(IUpdatable updatable)
        {
            if (updatable != null && !_updateables.Contains(updatable))
            {
                _updateables.Add(updatable);
            }
        }

        public void Remove(IUpdatable updatable)
        {
            _updateables.Remove(updatable);
        }

        public void Update(float dt)
        {
            // Iterate over a copy to allow modifications during update
            var snapshot = _updateables.ToArray();
            foreach (var updatable in snapshot)
            {
                updatable.Update(dt);
            }
        }

        public void Clear()
        {
            _updateables.Clear();
        }
    }
}
