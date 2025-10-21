# Ashes & Sakura: Last Stand of the Lost Unit

A 2D tactical defense game built with Unity 2D and URP.

## Project Overview

This is a wave-based tactical defense game where players place units on a battlefield to defend against incoming enemies. The game features a resource management system (Command Points), special skills, and increasing difficulty with random events.

## Game Features

- **Wave-based Enemy Spawning**: Enemies spawn from the right side in increasing difficulty
- **Unit Placement System**: Drag and drop units onto a 4-lane battlefield
- **Resource Management**: Command Points (CP) for placing units and using skills
- **Special Skills**: Airstrike, Field Medic, Emergency Backup, etc.
- **Random Events**: Storm, Boss Wave, Double Spawn, etc. (every 3-5 waves)
- **Permadeath Mode**: Optional hardcore mode

## Project Structure

```
AshesAndSakura/
├── Assets/
│   ├── Scripts/
│   │   ├── Managers/
│   │   │   ├── GameManager.cs          # Main game state controller
│   │   │   ├── WaveManager.cs          # Wave spawning and difficulty
│   │   │   ├── UnitManager.cs          # Unit database and placement
│   │   │   ├── ResourceManager.cs      # CP tracking and management
│   │   │   ├── SkillManager.cs         # Special skills system
│   │   │   ├── EnemySpawner.cs         # Enemy spawning logic
│   │   │   └── BattlefieldGrid.cs      # Grid system with lanes
│   │   ├── Units/
│   │   │   └── Unit.cs                 # Player unit behavior
│   │   ├── Enemies/
│   │   │   └── Enemy.cs                # Enemy behavior
│   │   ├── UI/
│   │   │   ├── UIManager.cs            # Main UI controller
│   │   │   └── DragDropHandler.cs      # Drag & drop implementation
│   │   ├── Data/
│   │   │   └── UnitData.cs             # Unit data structures
│   │   └── Utils/
│   │       ├── CameraController.cs     # Camera pan and zoom
│   │       └── HealthBar.cs            # Health bar display
│   ├── Data/
│   │   └── UnitData.json               # Unit definitions
│   ├── Prefabs/
│   │   ├── Units/                      # Unit prefabs
│   │   ├── Enemies/                    # Enemy prefabs
│   │   ├── UI/                         # UI prefabs
│   │   └── Skills/                     # Skill effect prefabs
│   ├── Scenes/
│   │   └── MainScene.unity             # Main game scene
│   ├── Sprites/                        # Art assets (add your own)
│   └── Audio/                          # Sound effects and music
└── README.md
```

## Setup Instructions

### 1. Open in Unity

1. Install **Unity 2021.3 LTS** or newer
2. Make sure **Universal Render Pipeline (URP)** package is installed
3. Open the project folder in Unity Hub
4. Wait for Unity to import all assets

### 2. Create the Main Scene

1. Create a new scene: `File > New Scene`
2. Save it as `MainScene.unity` in `Assets/Scenes/`

### 3. Set Up Core GameObjects

Create an empty GameObject hierarchy:

```
Scene Hierarchy:
├── GameManager (attach GameManager.cs)
├── Managers
│   ├── ResourceManager (attach ResourceManager.cs)
│   ├── WaveManager (attach WaveManager.cs)
│   ├── UnitManager (attach UnitManager.cs)
│   ├── SkillManager (attach SkillManager.cs)
│   ├── EnemySpawner (attach EnemySpawner.cs)
│   └── BattlefieldGrid (attach BattlefieldGrid.cs)
├── Main Camera (attach CameraController.cs)
└── Canvas
    └── UIManager (attach UIManager.cs)
```

### 4. Configure Components

#### UnitManager Setup:
- Drag `UnitData.json` from `Assets/Data/` to the "Unit Data Json" field
- Create a Unit prefab and assign it to "Unit Prefab" field

#### EnemySpawner Setup:
- Create enemy prefabs and add them to the "Enemy Prefabs" array
- Adjust spawn area and lane settings

#### BattlefieldGrid Setup:
- Configure number of lanes (default: 4)
- Adjust lane spacing and positioning
- The grid will be visualized in Scene view

#### UIManager Setup:
- Create UI elements using TextMeshPro for:
  - Wave display (top left)
  - CP display (top center)
  - Timer (top right)
  - Unit slots (bottom)
  - Skill buttons (right side)

### 5. Create Unit Prefab

1. Create a new GameObject: "Unit"
2. Add components:
   - SpriteRenderer (for visuals)
   - Unit.cs script
   - BoxCollider2D or CircleCollider2D
3. Tag it as "Unit"
4. Save as prefab in `Assets/Prefabs/Units/`

### 6. Create Enemy Prefab

1. Create a new GameObject: "Enemy"
2. Add components:
   - SpriteRenderer
   - Enemy.cs script
   - BoxCollider2D or CircleCollider2D
   - Rigidbody2D (set to Kinematic)
3. Tag it as "Enemy"
4. Save as prefab in `Assets/Prefabs/Enemies/`

### 7. Create UI Prefabs

#### Unit Slot Prefab:
- Create a UI Button
- Add Image for unit icon
- Add TextMeshProUGUI for unit name and CP cost
- Attach DragDropHandler.cs
- Save as prefab

#### Skill Slot Prefab:
- Create a UI Button
- Add Image for skill icon
- Add TextMeshProUGUI for skill name and cooldown
- Save as prefab

## Unit Data Configuration

Edit `Assets/Data/UnitData.json` to add or modify units:

```json
{
  "unitId": "soldier_basic",
  "unitName": "Basic Soldier",
  "description": "Standard infantry unit",
  "maxHealth": 100,
  "attackDamage": 15,
  "attackRange": 3.0,
  "attackCooldown": 1.5,
  "moveSpeed": 0,
  "cpCost": 50,
  "unitType": 0,
  "attackType": 1,
  "spritePath": "Sprites/Units/soldier_basic"
}
```

### Unit Types:
- 0 = Infantry
- 1 = Tank
- 2 = Sniper
- 3 = Medic
- 4 = Support

### Attack Types:
- 0 = Melee
- 1 = Ranged
- 2 = Splash

## Controls

### Gameplay:
- **Left Click + Drag**: Place units on battlefield
- **WASD / Arrow Keys**: Pan camera
- **Mouse Scroll**: Zoom in/out
- **Right/Middle Click + Drag**: Pan camera
- **ESC**: Pause game

### Debug Controls:
- **F1**: Start game
- **F2**: Restart game

## Debug Console

All managers log important events to the Unity Console:
- CP changes and spending
- Wave starts and completions
- Unit/enemy spawning and deaths
- Skill activations
- Random events

Look for tags like:
- `[ResourceManager]`
- `[WaveManager]`
- `[UnitManager]`
- `[SkillManager]`
- `[EnemySpawner]`
- `[GameManager]`

## Next Steps

1. **Add Art Assets**: Replace placeholder sprites with your anime/manga style art
2. **Create UI Theme**: Design black/red comic-style UI elements
3. **Add Sound Effects**: Implement audio for attacks, skills, and events
4. **Polish Visual Effects**: Add particle effects for skills and attacks
5. **Create Main Menu**: Design title screen and game over screens
6. **Implement Save System**: Add progression and high scores
7. **Balance Gameplay**: Tune difficulty curve and unit stats

## Technical Notes

- **Coordinate System**: Enemies spawn on the right (positive X) and move left (negative X)
- **Lane System**: 4 horizontal lanes with customizable spacing
- **Placement Zone**: Left side of battlefield (green area in Scene view)
- **Unit System**: Static tower defense style (units don't move)
- **Combat**: Automatic targeting of nearest enemy in range
- **Resource Generation**: Passive CP generation + bonus per wave

## Dependencies

- Unity 2021.3 LTS or newer
- Universal Render Pipeline (URP)
- TextMeshPro (included with Unity)

## Troubleshooting

**Units won't place:**
- Check that ResourceManager has enough CP
- Verify position is in placement zone (green area)
- Make sure Unit prefab has Unit.cs component

**Enemies not spawning:**
- Check that Enemy prefabs are assigned in EnemySpawner
- Verify WaveManager is starting waves (check Console)
- Make sure Enemy prefab has Enemy.cs component

**UI not updating:**
- Verify all text fields are assigned in UIManager
- Check that managers are subscribed to events
- Make sure you're using TextMeshPro, not legacy Text

## License

This project is provided as-is for educational and development purposes.

---

**Ready to build your tactical defense game!** 🎮

Replace placeholder sprites with your art, tune the gameplay, and create an epic last stand experience!
