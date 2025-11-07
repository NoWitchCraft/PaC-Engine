using System.Collections.Generic;
using Engine.Input;
using Raylib_cs;

public sealed class RaylibInputService : IInputService
{
    private readonly HashSet<Keys> _down = new();
    private readonly HashSet<Keys> _pressed = new();
    private readonly HashSet<Keys> _released = new();

    public void Update(float dt)
    {
        _pressed.Clear();
        _released.Clear();

        // Keyboard
        CheckKey(KeyboardKey.Up, Keys.Up);
        CheckKey(KeyboardKey.Down, Keys.Down);
        CheckKey(KeyboardKey.Left, Keys.Left);
        CheckKey(KeyboardKey.Right, Keys.Right);
        CheckKey(KeyboardKey.Space, Keys.Space);
        CheckKey(KeyboardKey.I, Keys.I);
        CheckKey(KeyboardKey.Tab, Keys.Tab);
        CheckKey(KeyboardKey.Escape, Keys.Escape);
        CheckKey(KeyboardKey.H, Keys.H);

#if DEBUG
        CheckKey(KeyboardKey.Grave, Keys.DevConsole);
#endif

        // Mouse
        CheckMouse(MouseButton.Left, Keys.MouseLeft);
        CheckMouse(MouseButton.Right, Keys.MouseRight);
    }

    private void CheckKey(KeyboardKey k, Keys e)
    {
        if (Raylib.IsKeyPressed(k))  _pressed.Add(e);
        if (Raylib.IsKeyReleased(k)) _released.Add(e);
        if (Raylib.IsKeyDown(k)) _down.Add(e); else _down.Remove(e);
    }

    private void CheckMouse(MouseButton mb, Keys e)
    {
        if (Raylib.IsMouseButtonPressed(mb))  _pressed.Add(e);
        if (Raylib.IsMouseButtonReleased(mb)) _released.Add(e);
        if (Raylib.IsMouseButtonDown(mb)) _down.Add(e); else _down.Remove(e);
    }

    public bool IsDown(Keys key) => _down.Contains(key);
    public bool WasPressed(Keys key) => _pressed.Contains(key);
    public bool WasReleased(Keys key) => _released.Contains(key);
}
