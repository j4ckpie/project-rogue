# PROJECT ROGUE
**A top-down pixel-art roguelike with procedurally generated dungeons, built in Godot 4 and C#.**

![Godot](https://img.shields.io/badge/Godot-4.3-478CBF?logo=godotengine&logoColor=white)
![C#](https://img.shields.io/badge/C%23-.NET%206-512BD4?logo=dotnet&logoColor=white)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
![Status](https://img.shields.io/badge/status-in%20development-orange)

*Individual Project - Warsaw University of Technology, summer semester 2026*

<!-- Add a gameplay GIF or screenshot here, e.g.: ![Gameplay](docs/gameplay.gif) -->

## About

Every run drops you into a brand-new dungeon. Fight your way through orcs using light and heavy melee attacks and a bow, manage your stamina, level up as you go, and find the exit before you get overwhelmed. At the end of a run you get a summary of your eliminations, XP, level, time and final score.

## Features

- **Procedural dungeon generation** – each run builds a unique layout of up to ~40 rooms from hand-designed room templates, with collision checks so rooms never overlap.
- **Three-way combat system** – light attack, heavy attack with knockback and a cooldown, and a ranged bow shot that slows and staggers enemies.
- **Stamina and regeneration** – attacks cost stamina; stamina and health regenerate after a short delay without taking damage.
- **Enemy AI** – enemies chase the player within range, pick between light and heavy attacks, respect attack cooldowns and can be knocked back or slowed.
- **Enemy spawners and traps** – some rooms contain spawners that release waves of enemies once the player walks in.
- **Progression on both sides** – the player levels up (more health, damage, stamina and speed), and enemies that land hits on the player grow stronger too.
- **Game feel** – camera that leans toward the cursor, screen shake, particle effects, dynamic 2D lighting, floating damage and XP numbers, and an arrow pointing toward the exit.
- **Full game loop** – HUD, pause menu, death screen and an end-of-run stats screen with score and the option to restart.

## Controls

| Action | Key |
|---|---|
| Move | `W` `A` `S` `D` / arrow keys |
| Sprint | `Shift` |
| Sneak | `Alt` |
| Light attack | `LMB` |
| Heavy attack | `RMB` |
| Shoot arrow | `Space` |
| Aim | `Mouse` |
| Pause | `Esc` |

## Getting started

### Requirements

- [Godot 4.3 – .NET version](https://godotengine.org/download)
- [.NET SDK 6.0](https://dotnet.microsoft.com/download) or newer

### Running the project

```bash
git clone https://github.com/j4ckpie/project-rogue.git
```

1. Open Godot (.NET) and import `godot/project.godot`.
2. Build the C# solution (**Build** button in the top-right corner, or `dotnet build` inside `godot/`).
3. Press **F5** to run. The main scene is `scenes/World.tscn`.

## Project structure

```
godot/
├── scenes/       # .tscn scenes: world, entities, rooms (start / mid / end), UI
├── src/          # C# source code
├── sprites/      # Pixel-art sprite sheets and light masks
├── shaders/      # Custom canvas_item shaders
└── fonts/        # Pixelify Sans, Jacquard 12
```

## Roadmap

- [ ] Upgrade choices on level-up instead of fixed stat increases
- [ ] Scaling XP thresholds per level
- [ ] More enemy types with distinct behaviour
- [ ] Sound effects and music
- [ ] Settings menu with key rebinding

## Developed by

**Jakub Pietrala** - [@j4ckpie](https://github.com/j4ckpie)
