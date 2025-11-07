# Rendering System

## Übersicht

Das Rendering-System der PaC Engine ist modular aufgebaut und ermöglicht verschiedene Backend-Implementierungen.

## Architektur

### IRenderer Interface

Das zentrale Interface für alle Renderer:

```csharp
public interface IRenderer
{
    void Initialize();          // Einmalige Initialisierung
    void BeginFrame();         // Frame-Start
    void RenderScene(SceneRuntime? scene);  // Szene rendern
    void EndFrame();           // Frame-Ende  
    void Shutdown();           // Cleanup
}
```

### Implementierungen

#### ConsoleRenderer

Eine einfache Console-basierte Implementierung für Entwicklung und Testing.

**Verwendung:**
```csharp
var renderer = new ConsoleRenderer(logger);
renderer.Initialize();

// Im Game-Loop
renderer.BeginFrame();
renderer.RenderScene(currentScene);
renderer.EndFrame();
```

**Features:**
- ASCII-basierte Ausgabe
- Zeigt Szenen-Metadaten
- Performance-optimiert (nur alle 60 Frames)
- Nützlich für Debugging

#### Zukünftige Renderer

- **MonoGameRenderer**: 2D-Grafik mit MonoGame
- **SDLRenderer**: SDL2-basiertes Rendering
- **EditorPreviewRenderer**: WPF-Integration für Editor

## Integration

### In GameApp

```csharp
public sealed class GameApp : IGameApp
{
    private IRenderer? _renderer;
    
    public void Initialize()
    {
        _renderer = new ConsoleRenderer(logger);
        _renderer.Initialize();
        ServiceRegistry.Register<IRenderer>(_renderer);
    }
    
    public void Render()
    {
        _renderer?.BeginFrame();
        _renderer?.RenderScene(_scenes?.Current);
        _renderer?.EndFrame();
    }
    
    public void Shutdown()
    {
        _renderer?.Shutdown();
    }
}
```

### Mit ServiceRegistry

```csharp
// Renderer registrieren
ServiceRegistry.Register<IRenderer>(renderer);

// Renderer von überall abrufen
var renderer = ServiceRegistry.Get<IRenderer>();
```

## Erweiterung

Eigene Renderer können einfach implementiert werden:

```csharp
public class MyCustomRenderer : IRenderer
{
    public void Initialize()
    {
        // Setup: Fenster erstellen, Grafik initialisieren
    }
    
    public void BeginFrame()
    {
        // Frame vorbereiten: Clear, Kameras setzen
    }
    
    public void RenderScene(SceneRuntime? scene)
    {
        if (scene == null) return;
        
        // Background rendern
        DrawBackground(scene.BackgroundPath);
        
        // Hotspots rendern
        foreach (var hotspot in scene.Hotspots)
        {
            if (hotspot.Highlight)
                DrawHotspot(hotspot.Rect);
        }
    }
    
    public void EndFrame()
    {
        // Frame abschließen: Swap buffers
    }
    
    public void Shutdown()
    {
        // Cleanup: Ressourcen freigeben
    }
}
```

## Best Practices

1. **Trennung von Update und Render**: Logik in Update(), Darstellung in Render()
2. **Frame-Timing**: BeginFrame/EndFrame für Performance-Messungen nutzen
3. **Null-Safety**: Immer auf null-Szenen prüfen
4. **Resource Management**: Ressourcen in Initialize() laden, in Shutdown() freigeben
5. **Logging**: Wichtige Events loggen (Initialisierung, Fehler)

## Performance-Tipps

- Render nur was sichtbar ist (Frustum Culling)
- Batch ähnliche Draw-Calls
- Nutze Sprite-Atlanten für Texturen
- Implementiere Level-of-Detail für komplexe Szenen
- Profile regelmäßig mit dem Frame-Counter
