namespace Engine.Input
{
    /// <summary>
    /// Provides input state tracking for the game.
    /// </summary>
    public interface IInputService
    {
        void Update(float dt);
        bool IsDown(Keys key);
        bool WasPressed(Keys key);
        bool WasReleased(Keys key);
    }
}
