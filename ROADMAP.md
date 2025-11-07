# 🧭 PaC Engine – Entwicklungs-Roadmap

> Ziel: Eine eigenständige 2D-Adventure-Engine (Framework + Editor),  
> mit der mehrere Spiele unabhängig voneinander erstellt werden können.  
> Der Editor ist der zentrale Hub zum Anlegen, Verwalten und Bearbeiten von Game-Projekten.

---

## 🧱 I. ENGINE CORE (Framework & Runtime)

### 1. Architektur & Grundsysteme
- [x] Projektstruktur (`Engine`, `Game`, `Editor`)
- [x] Settings-System (`settings.json`, Loader)
- [x] Logging-System (Debug, Info, Warn, Error mit Timestamp)
- [ ] Utility-Klassen (`Vector2`, `RectF`, `MathHelper`, etc.)
- [ ] Core-Loop: Update / Render-Pipeline
- [ ] ECS- / Entity-System (Actors, Components, Systems)
- [ ] Event-System (globale Variablen, Switches, Trigger)
- [ ] Resource-Manager (Asset-Caching, Lazy-Loading, Cleanup)
- [ ] Scene-Runtime-Loader (JSON → Runtime-Scene)

### 2. Subsysteme
- [ ] Audio-System (SoundBank, Music, SFX, Volume)
- [ ] Input-System (Keyboard / Mouse / Controller)
- [ ] UI-System (Overlays, Inventar, Dialog-Boxen)
- [ ] Pathfinding / WalkMesh (2D-Navigationsnetz)
- [ ] Animation (Sprites, Frame-Animation, später Timeline)
- [ ] Save / Load-System (JSON oder Binär)

---

## 🧰 II. EDITOR CORE (Tooling & Projektverwaltung)

### 1. Projekt-Management
- [ ] **Start-Hub:** Beim Start der compiled Version kann der Editor:
  - [ ] Neues Game-Projekt anlegen (z. B. Pfad + Name wählen)
  - [ ] Bestehendes Game-Projekt öffnen
  - [ ] Zuletzt geöffnete Projekte anzeigen
- [ ] Projekt-Templates (Standard-Ordnerstruktur, Default-Scenes)
- [ ] Engine-Version / Projekt-Version Tracking
- [ ] Einstellungen pro Projekt (ContentRoot, Sprache, Auflösung, etc.)

### 2. Szene- & Asset-Management
- [x] Scene-Load / Save (JSON)
- [x] Hierarchie + Inspector
- [x] Kontext-Menüs Add / Delete
- [x] Scene-Validation (IDs, Bounds, fehlende Referenzen)
- [x] Auto-Fix für häufige Validierungsfehler
- [x] Status-Feedback für Validierungsergebnisse
- [ ] Asset-Browser (ContentRoot durchsuchen)
- [ ] Auto-Reload bei Dateiänderungen
- [ ] Multi-Projekt-Support (Engine-Version + Game-Pfad getrennt)

### 3. Visual Editing (Viewport)
- [ ] Background-Render + Zoom / Pan
- [ ] Hotspot-Rects zeichnen, verschieben, resizen
- [ ] Auswahl-Sync: Klick im Viewport ↔ Inspector
- [ ] Snap-to-Grid / Pixelmaßstab
- [ ] Scene-Preview-Mode („Test Run“ im Editor)

### 4. Erweiterbare Panels
- [ ] Audio-Cue-Inspector
- [ ] Dialogue-Graph-Editor (NodeGraph)
- [ ] Variable-Inspector (Switches / Vars)
- [ ] Timeline-Editor (Cutscene-Sequenzen)
- [ ] Log & Console-Panel (Engine-Output)

---

## ⚙️ III. ENGINE / EDITOR INTEGRATION

### 1. Laufzeit & Kommunikation
- [ ] Gemeinsames Scene-Format (DTO ↔ Runtime)
- [ ] Live-Preview: Editor kann laufende Engine-Instanz starten
- [ ] Hot-Reload von Assets (on save → update running scene)
- [ ] Engine-Debugger-Bridge (Play / Pause / Step)

### 2. Erweiterbarkeit
- [ ] Plug-in-System (eigene Node-Typen, Custom-Tools)
- [ ] Skript-Schnittstelle (C#-Skripte / externe Logic-Hooks)
- [ ] Config-Encryption (geschützte Game-Daten)
- [ ] Mod-Support (externe Scene-Packs laden)

---

## 🧭 IV. TOOLING / META / DEPLOYMENT

### 1. Build & Struktur
- [ ] Build-Pipeline (Game + Engine als Runtime)
- [ ] Asset-Packaging / Kompression
- [ ] Versioning (EngineVersion.json)
- [ ] Installer / Auto-Updater für den Editor

### 2. Qualität & Workflow
- [ ] Unit-Tests / Smoke-Tests
- [ ] Debug-Overlay (FPS, Entities, Memory)
- [ ] In-Engine Profiler
- [ ] Crash-Reporter (Logs + Szenenname)
- [ ] Editor-Undo / Redo + History
- [ ] Lokale Engine-Dokumentation / Wiki-Generator

### 3. Veröffentlichung & Nutzung
- [ ] Exporter (Windows / Linux / Web)
- [ ] Lizenzmodell (Open-Source / Engine-Branding)
- [ ] Engine-SDK für Third-Party-Games

---

## 🔒 V. SPÄTERE ERWEITERUNGEN (optional / R&D)

- [ ] 3D-Layer (2.5D-Version)
- [ ] Shader-Pipeline (Lighting, Color-Grading)
- [ ] Multiplayer-Proof-of-Concept
- [ ] Physik-Integration (2D-Collision / Simple Dynamics)
- [ ] Cloud-Sync (Editor-Settings & Projekte)
- [ ] Plugin-Store / Community-Module

---

## 🧩 NÄCHSTE SCHRITTE (nach aktuellem Stand)
1. ✅ Scene-Validation & Status-Feedback (SC2.3) **[COMPLETED]**
2. 🔲 Viewport-System (SC3)  
3. 🔲 Editor-Start-Hub / Game-Projekt-Anlage (SC4)  
4. 🔲 Logging & Debug-System (für Engine & Editor gemeinsam)  
5. 🔲 Erste Engine-Runtime: Scene-Load & Render-Loop  

---

> 💡 Diese Roadmap wird schrittweise verfeinert.  
> Jeder Abschnitt kann eigene `TODO.md`-Dateien oder Issues im Repo bekommen.  
> Ziel: Eine vollständig eigenständige, modulare Engine für 2D-Adventure-Games.

---

