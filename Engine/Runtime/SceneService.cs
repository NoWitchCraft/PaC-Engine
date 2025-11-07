using System;
using Engine.Content;
using Engine.Core;
using Engine.Data.Scene;
using Engine.Diagnostics;

namespace Engine.Runtime
{
    public interface ISceneService
    {
        SceneRuntime? Current { get; }
        void LoadFromContent(string relativeScenePath);
        void LoadFromDTO(SceneDTO dto);
        HotspotRuntime? HitTest(float x, float y);
    }

    public sealed class SceneService : ISceneService
    {
        public SceneRuntime? Current { get; private set; }

        public void LoadFromContent(string relativeScenePath)
        {
            var log = ServiceRegistry.Get<ILogger>();
            
            try
            {
                log.Info(nameof(SceneService), $"Loading scene from: {relativeScenePath}");
                var scene = SceneIO.LoadFromContent(relativeScenePath);
                LoadFromDTO(scene);
                log.Info(nameof(SceneService), $"Scene loaded successfully: {Current?.Id}");
            }
            catch (Exception ex)
            {
                log.Error(nameof(SceneService), $"Failed to load scene '{relativeScenePath}': {ex.Message}");
                throw;
            }
        }

        public void LoadFromDTO(SceneDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
                
            Current = SceneRuntimeMapper.FromDTO(dto);
        }

        public HotspotRuntime? HitTest(float x, float y)
            => Current?.FindHotspotAt(x, y);
    }
}
