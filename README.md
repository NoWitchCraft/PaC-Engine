# 🧩 PaC Engine  
### A modular C# framework & editor for building modern 2D point-and-click adventure games

![PaC Engine Logo](./Docs/Images/Logo.png)  
[Platzhalter: Screenshot des Editors mit Viewport + Hierarchy + Inspector]

---

## 🚀 Overview

**PaC Engine** (Point-and-Click Engine) is a standalone 2D game framework written in **C#**, designed for creating story-driven point-and-click adventures similar to *Deponia*, *Monkey Island* or *Thimbleweed Park*.  
It combines a clean runtime engine with a fully integrated **visual editor**, allowing creators to design, test and export complete games — all without depending on Unity or Unreal.

> ✨ Focus: Lightweight · Modular · Extensible · One-Click export

---

## 🧱 Core Architecture

| Module | Description |
|--------|--------------|
| **Engine** | The runtime framework. Handles rendering, input, scene management, pathfinding, audio, UI, savegames and logic. |
| **Editor** | Visual WPF-based scene editor for designing hotspots, dialogues, cutscenes, assets and world logic. |
| **Game** | Example implementation built on top of the engine (can be replaced by your own projects). |
| **Data Layer (DTOs)** | JSON-based data contracts shared between Editor and Engine. |
| **Plugins** | Optional extension system for custom nodes, scripts or inspector panels. |

---

## 🧰 Features

### 🎮 Runtime Engine
- Scene system with hotspots, items, NPCs, walkable areas  
- 2D rendering pipeline (Sprites, Layers, Parallax)  
- Pathfinding (WalkMesh or mask-based navigation)  
- Audio playback (BGM, SFX, voice lines)  
- Dialogue & event system (node-based)  
- Global variable/switch management for quests  
- Inventory & interaction framework  
- Save/Load system (JSON / Binary)  
- Multi-language localization  
- Config encryption & asset packaging  

### 🧱 Editor
- Hierarchy & Inspector system (Unity-like workflow)  
- Visual scene editing (backgrounds + hotspot rectangles)  
- Node editor for dialogue & logic events  
- Asset browser (import / drag-drop)  
- **Project Creation Hub** → create new game projects directly from the Editor UI  
- Live Preview mode (test scenes without export)  
- Validation tools (missing IDs, invalid bounds)  
- Undo/Redo and auto-save  
- Multi-project support (manage multiple games)  

### 🧩 Extensibility
- Plugin SDK (C# API)  
- Custom node types & editor panels  
- Script events (C# or Lua) [Platzhalter: Entscheidung]  
- Hot reload support for assets and scenes  
- Modular engine core — can be used independently from the Editor  

---

## 🧭 Typical Workflow

1. **Start the Editor** → Create or open a game project  
2. **Design your world** → Place hotspots, scenes, NPCs  
3. **Build logic** → Link dialogues, events, conditions  
4. **Preview in Editor** → Test directly without compiling  
5. **Export** → Build a distributable version of your game  
6. **Ship it!**

![Editor Screenshot](./Docs/Images/editor_viewport.png)  
[Platzhalter: Screenshot des Viewports mit Hotspot-Rects]

---

## 🧰 Project Structure
PaCEngine/
│
├─ Engine/ → Core runtime (framework)
│ ├─ Data/ → DTOs & serialization
│ ├─ Systems/ → Audio, Input, Scene, etc.
│ ├─ Components/ → ECS-style entities
│ └─ ...
│
├─ Editor/ → WPF-based visual editor
│ ├─ Views/ → Windows, Panels, Controls
│ ├─ Systems/ → Scene I/O, Inspector logic
│ └─ ...
│
├─ Game/ → Example adventure game project
│
├─ Docs/ → Documentation, images, changelogs
│
└─ ROADMAP.md → Development plan & progress

---

## 🧩 System Requirements

| Component | Minimum | Recommended |
|------------|-----------|-------------|
| **OS** | Windows 10 (x64) | Windows 11 |
| **.NET Runtime** | .NET 8.0 | .NET 8.0+ |
| **CPU** | Dual-Core 2.5 GHz | Quad-Core 3 GHz |
| **GPU** | DirectX 11 capable | DX12 / Vulkan compatible |
| **RAM** | 4 GB | 8 GB+ |
| **Storage** | 500 MB | SSD recommended |

> Linux/macOS not supportet

---

## 📦 Installation

### Option 1 – Prebuilt Editor (recommended)
1. Download the latest **PaCEngine Editor** release  
   → still in Developement 
2. Extract and run `PaCEngineEditor.exe`  
3. Create a new game project or open an existing one  

### Option 2 – Build from Source
```bash
git clone https://github.com/NoWitchCraft/PaCEngine.git
cd PaCEngine
dotnet build
```

---

## 🧑‍💻 For Developers

The Engine is written in C# (.NET 8)

Editor uses WPF (MVVM-lite architecture)

Data serialization via System.Text.Json

Rendering backend: [Platzhalter: SDL / MonoGame / Custom Renderer]

Script layer: [Platzhalter: C# Reflection / Lua / Node Execution System]

Audio backend: [Platzhalter: NAudio / FMOD / Custom]

## 🔒 Licensing

[Platzhalter: Lizenztyp z. B. MIT / Custom Commercial / Dual-License]

Engine and Editor are free for non-commercial use

## 🧭 Roadmap

See full development roadmap here:
👉 ROADMAP.md

## 🧑‍🎨 Credits

Engine Architecture: Michael Hamann

Documentation: 

Special Thanks: 

💬 Support & Community

Discord: 

Website: 

Email: michael-hamann@outlook.com

© 2025 PaC Engine — Made with C#, coffee ☕ and a lot of curiosity.
