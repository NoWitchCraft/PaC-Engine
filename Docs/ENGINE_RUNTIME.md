# Engine Runtime - Scene Load & Render Loop

## Übersicht

Die PaC Engine verfügt jetzt über eine vollständig lauffähige Runtime mit getrennten Update- und Render-Loops. Die Engine kann Szenen aus JSON-Dateien laden und sie im Preview-Modus darstellen.

## Komponenten

### 1. EngineHost

Der `EngineHost` ist der zentrale Game-Loop-Manager mit folgenden Features:

- **Getrennte Update- und Render-Loops**: Update-Phase für Logik, Render-Phase für Darstellung
- **Fixed-Rate Loop**: Standardmäßig 60 FPS
- **Frame-Time-Management**: Automatisches Throttling für konsistente Performance

```csharp
var app = new GameApp();
var host = new EngineHost(app, targetFps: 60);
host.Run();
```

### 2. IGameApp Interface

Das `IGameApp` Interface definiert den Lebenszyklus einer Anwendung:

```csharp
public interface IGameApp
{
    void Initialize();  // Einmalige Initialisierung
    void Update(float dt);  // Logik-Update (60x/Sekunde)
    void Render();  // Rendering (60x/Sekunde)
    void Shutdown();  // Cleanup
}
```

### 3. Scene Service

Der `SceneService` verwaltet das Laden und Zugreifen auf Szenen:

**Features:**
- Laden von Szenen aus JSON-Dateien
- Umfangreiche Fehlerbehandlung mit Logging
- Hit-Testing für Hotspots
- DTO zu Runtime-Object Mapping

**Verwendung:**
```csharp
var sceneService = new SceneService();
sceneService.LoadFromContent("Scenes/first.scene.json");

// Hit-Test auf Hotspots
var hotspot = sceneService.HitTest(900, 520);
if (hotspot != null)
{
    // Hotspot gefunden
}
```

### 4. Rendering System

#### IRenderer Interface

Das `IRenderer` Interface definiert das Rendering-Backend:

```csharp
public interface IRenderer
{
    void Initialize();
    void BeginFrame();
    void RenderScene(SceneRuntime? scene);
    void EndFrame();
    void Shutdown();
}
```

#### ConsoleRenderer

Eine einfache Console-basierte Implementierung für Preview-Zwecke:

- Zeigt Szeneninformationen im Console-Output
- Rendert nur alle 60 Frames (1x pro Sekunde)
- Zeigt Hotspot-Positionen und Events an

**Output-Beispiel:**
```
╔════════════════════════════════════════╗
║   PaC Engine - Scene Preview          ║
╚════════════════════════════════════════╝

[Frame 60] Rendering Scene: first_room
  Background: Rooms/first_room_bg.png
  Hotspots: 2
  ┌─ Hotspots ──────────────────────────
  │ • hs.console
  │   Position: (900, 520)
  │   Size: 180x120
  │   Event: console_inspect
  └─────────────────────────────────────
```

### 5. ScenePreview

Der `ScenePreview` ist ein spezieller Modus für die Editor-Integration:

**Features:**
- Kann einzelne Szenen ohne vollständiges GameApp laden
- Unterstützt dynamisches Nachladen von Szenen
- Nutzt vorhandene Services aus dem ServiceRegistry
- Ideal für Live-Preview im Editor

**Verwendung:**
```csharp
var preview = new ScenePreview("Scenes/first.scene.json");
var host = new EngineHost(preview, targetFps: 60);
host.Run();

// Szene zur Laufzeit wechseln
preview.LoadScene("Scenes/menu_room.scene.json");
```

## Szenen-Format

Szenen werden als JSON-Dateien gespeichert:

```json
{
  "id": "first_room",
  "backgroundPath": "Rooms/first_room_bg.png",
  "startPosition": {
    "x": 220,
    "y": 610
  },
  "hotspots": [
    {
      "id": "hs.console",
      "labelKey": "hs.console",
      "rect": {
        "x": 900,
        "y": 520,
        "width": 180,
        "height": 120
      },
      "eventPathId": "console_inspect",
      "cursorId": "inspect",
      "highlight": true
    }
  ],
  "musicCueId": null,
  "ambientCueIds": []
}
```

## Fehlerbehandlung

Die Engine bietet umfassende Fehlerbehandlung:

### Scene Loading Errors

```csharp
try
{
    sceneService.LoadFromContent("Scenes/invalid.scene.json");
}
catch (Exception ex)
{
    // Fehler wird geloggt:
    // ERROR SceneService: Failed to load scene 'Scenes/invalid.scene.json': Scene-Datei nicht gefunden
}
```

### Fallback-Mechanismen

Die `GameApp` implementiert Fallback-Logik:
- Bei Fehler beim Laden wird eine Warnung geloggt
- Die Anwendung stürzt nicht ab
- Ermöglicht graceful degradation

## Service Registry

Zentrales Service-Management mit Dependency Injection:

```csharp
// Service registrieren
ServiceRegistry.Register<ISceneService>(sceneService);

// Service abrufen
var sceneService = ServiceRegistry.Get<ISceneService>();

// Optional: Service abrufen (null wenn nicht vorhanden)
var sceneService = ServiceRegistry.TryGet<ISceneService>();
```

## Logging

Umfassendes Logging auf verschiedenen Ebenen:

- **DEBUG**: Detaillierte Informationen (z.B. Resource Loading)
- **INFO**: Wichtige Events (z.B. Scene Loaded)
- **WARN**: Warnungen (z.B. Fallback verwendet)
- **ERROR**: Fehler (z.B. Scene nicht gefunden)

```csharp
var logger = new ConsoleLogger(LogLevel.Debug);
Log.Use(logger);

Log.Info("GameApp", "Application started");
Log.Error("SceneService", "Failed to load scene");
```

## Performance

- **Target FPS**: 60 FPS (konfigurierbar)
- **Frame-Time Management**: Automatisches Throttling
- **Lazy Rendering**: ConsoleRenderer rendert nur 1x pro Sekunde zur Performance-Optimierung

## Nächste Schritte

Für die weitere Entwicklung:

1. **Grafisches Rendering**: Integration von MonoGame/SDL für echtes 2D-Rendering
2. **Input-System**: Maus- und Tastatur-Eingabe verarbeiten
3. **Animation-System**: Sprite-Animationen und Transitions
4. **Editor-Integration**: Live-Preview im WPF-Editor
5. **Audio-System**: Musik und Sound-Effekte

## Beispiel-Anwendung

Minimales Beispiel für eine funktionierende Engine-Runtime:

```csharp
// Services initialisieren
Settings.Load();
ServiceRegistry.Clear();

var logger = new ConsoleLogger(LogLevel.Debug);
ServiceRegistry.Register<ILogger>(logger);

var fs = new FileSystem();
ServiceRegistry.Register<IFileSystem>(fs);

var resolver = new ContentResolver(fs, Settings.Current.ContentRoot);
ServiceRegistry.Register<IContentResolver>(resolver);

// GameApp erstellen und starten
var app = new GameApp();
var host = new EngineHost(app, targetFps: 60);
host.Run();
```

## Akzeptanzkriterien (Erfüllt)

✅ **Engine kann Scene aus JSON-Datei laden und anzeigen**
   - SceneService lädt JSON mit vollständiger Fehlerbehandlung
   - ConsoleRenderer zeigt Szenen-Informationen an

✅ **Update- und Render-Loop sind getrennt implementiert**
   - EngineHost ruft Update() und Render() getrennt auf
   - Klare Trennung von Logik und Darstellung

✅ **Fehler beim Laden der Szene werden korrekt geloggt**
   - Umfassende Fehlerbehandlung in SceneService
   - Fehler werden mit ERROR-Level geloggt
   - Stacktraces werden erfasst

✅ **Szene-Preview im Editor lauffähig**
   - ScenePreview-Klasse für Editor-Integration
   - Kann einzelne Szenen laden und anzeigen
   - Unterstützt dynamisches Nachladen
