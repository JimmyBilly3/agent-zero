# Architecture Overview - Ashes & Sakura

## System Design

This document explains the technical architecture and how all systems work together.

## Core Systems

### 1. Manager Pattern

All major systems use the Singleton pattern for easy access:

```csharp
GameManager.Instance
ResourceManager.Instance
WaveManager.Instance
UnitManager.Instance
SkillManager.Instance
EnemySpawner.Instance
BattlefieldGrid.Instance
```

### 2. Event-Driven Architecture

Systems communicate via C# events to reduce coupling:

```csharp
// ResourceManager
OnCPChanged?.Invoke(currentCP);
OnCPSpent?.Invoke(amount, remaining);

// WaveManager
OnWaveStarted?.Invoke(waveNumber);
OnWaveCompleted?.Invoke(waveNumber);
OnRandomEvent?.Invoke(eventName);

// Unit
OnDeath?.Invoke(this);
OnHealthChanged?.Invoke(currentHealth, maxHealth);
```

### 3. Data-Driven Design

Units are defined in JSON, allowing easy modification without code changes:

```json
{
  "units": [
    {
      "unitId": "soldier_basic",
      "maxHealth": 100,
      "attackDamage": 15,
      ...
    }
  ]
}
```

## System Flow Diagrams

### Game Initialization Flow

```
GameManager.Awake()
  ↓
All Managers.Awake() (Singleton setup)
  ↓
UnitManager loads UnitData.json
  ↓
WaveManager starts wave routine
  ↓
ResourceManager starts CP generation
  ↓
GameManager.StartGame()
```

### Wave Lifecycle

```
WaveManager starts wave
  ↓
Triggers OnWaveStarted event
  ↓
EnemySpawner.SpawnWave(count, multipliers)
  ↓
Enemies spawn over time
  ↓
Each enemy subscribes to OnDeath event
  ↓
When enemy dies → WaveManager.OnEnemyDefeated()
  ↓
When all enemies dead → WaveManager completes wave
  ↓
Triggers OnWaveCompleted event
  ↓
ResourceManager adds bonus CP
  ↓
Check for random events (every 3 waves)
  ↓
Wait timeBetweenWaves → Start next wave
```

### Unit Placement Flow

```
Player drags unit from UI (DragDropHandler)
  ↓
OnBeginDrag: Check CP availability
  ↓
OnDrag: Show preview, update color based on validity
  ↓
OnEndDrag: Get world position from mouse
  ↓
BattlefieldGrid.SnapToGrid(position)
  ↓
Check if in placement zone
  ↓
Check if cell occupied
  ↓
UnitManager.TryPlaceUnit(unitId, position, lane)
  ↓
ResourceManager.TrySpendCP(cost)
  ↓
Instantiate unit prefab
  ↓
Unit.Initialize(data, lane)
  ↓
Add to UnitManager.ActiveUnits list
```

### Combat System Flow

```
Unit.Update()
  ↓
attackTimer counting down
  ↓
If no target → FindTarget()
  ↓
Search EnemySpawner.ActiveEnemies
  ↓
Filter by same lane
  ↓
Find closest in range
  ↓
Set currentTarget
  ↓
If target in range and attackTimer ready
  ↓
Attack() → target.TakeDamage(damage)
  ↓
Reset attackTimer
```

### Skill Activation Flow

```
Player clicks skill button
  ↓
UIManager.OnSkillButtonClicked(skillId)
  ↓
SkillManager.TryActivateSkill(skillId, position)
  ↓
Get SkillData
  ↓
Check cooldown (IsOnCooldown)
  ↓
ResourceManager.TrySpendCP(cost)
  ↓
Execute skill logic:
  - Airstrike: Find enemies in radius, damage them
  - Heal: Find units in radius, heal them
  - Summon: Spawn free unit
  ↓
Set cooldown timer
  ↓
Trigger OnSkillActivated event
```

## Key Design Decisions

### 1. Why Singleton Pattern?

**Pros:**
- Easy global access for managers
- Guaranteed single instance
- Simple to use: `ResourceManager.Instance.AddCP(50)`

**Cons:**
- Can lead to tight coupling (mitigated by events)
- Harder to unit test (can be refactored later)

**Alternatives considered:** Dependency injection, ScriptableObject architecture

### 2. Why Event System?

**Pros:**
- Decouples systems (WaveManager doesn't need to know about UI)
- Easy to add new listeners
- Clean separation of concerns

**Example:**
```csharp
// ResourceManager doesn't know about UI
OnCPChanged?.Invoke(currentCP);

// UIManager subscribes and updates display
ResourceManager.Instance.OnCPChanged += UpdateCPDisplay;
```

### 3. Why JSON for Unit Data?

**Pros:**
- Easy to edit without recompiling
- Can be modded by players
- Clear structure
- Version control friendly

**Cons:**
- Requires parsing at runtime
- No compile-time validation

**Alternatives:** ScriptableObjects (better for Unity), XML, custom editor

### 4. Lane-Based System

**Design:**
- Units and enemies locked to specific lanes (Y position)
- Simplifies pathfinding (no A* needed)
- Clear tactical decisions

**Implementation:**
```csharp
public float GetLaneYPosition(int laneIndex)
{
    return laneYStart + (laneIndex * laneSpacing);
}
```

### 5. Static Tower Defense

**Units don't move** - they're placed and stay put

**Pros:**
- Simpler implementation
- Clear strategy (placement is key)
- Better performance

**If you want moving units:**
1. Add NavMeshAgent or simple waypoint movement
2. Update combat to track while moving
3. Add formation system

## Performance Considerations

### Optimization Strategies

1. **Object Pooling** (TODO):
   - Pool enemy instances instead of Instantiate/Destroy
   - Pool projectiles and VFX

2. **Spatial Partitioning**:
   - Grid system already in place
   - Can be used for faster neighbor queries

3. **Efficient Target Finding**:
```csharp
// Current: O(n) search through all enemies
// Better: Maintain per-lane enemy lists

// Future optimization:
Dictionary<int, List<Enemy>> enemiesByLane;
```

4. **Update Optimization**:
```csharp
// Don't update every frame for some systems
if (Time.frameCount % 5 == 0) // Every 5 frames
{
    FindTarget(); // Expensive operation
}
```

## Extension Points

### Adding New Unit Types

1. Add to `UnitData.json`
2. Add sprite to `Assets/Sprites/Units/`
3. (Optional) Create specialized behavior script inheriting from `Unit.cs`

### Adding New Skills

1. Add `SkillData` to `SkillManager` inspector
2. Implement logic in `SkillManager.ActivateSkill()`
3. Create VFX prefab

### Adding New Enemy Types

1. Create prefab with `Enemy.cs`
2. Add to `EnemySpawner.enemyPrefabs[]`
3. (Optional) Override behavior in derived class

### Adding Random Events

Add to `WaveManager.TriggerRandomEvent()`:

```csharp
string[] events = new string[]
{
    "Storm",         // Reduce unit attack speed
    "BossWave",      // Spawn boss enemy
    "DoubleSpawn",   // 2x enemies
    "FastEnemies",   // Enemies move faster
    "ArmoredEnemies",// Enemies have more HP
    "CPBonus",       // Extra CP this wave
    "TimeWarp"       // Slow motion
};
```

## Testing Strategy

### Manual Testing Checklist

- [ ] Units can be placed in all lanes
- [ ] Units attack enemies in their lane
- [ ] CP generates passively
- [ ] CP is spent on placement
- [ ] Waves increase in difficulty
- [ ] Skills can be activated
- [ ] Random events trigger
- [ ] Game over when base health = 0

### Debug Features

All systems log to console:
```
[ResourceManager] Spent 50 CP. Remaining: 150/999
[WaveManager] Wave 3 started! Enemies: 9
[Unit] Basic Soldier attacked for 15 damage!
[Enemy] Took 15 damage. HP: 35
```

### Debug Controls

```csharp
// GameManager.Update()
if (Input.GetKeyDown(KeyCode.F1)) StartGame();
if (Input.GetKeyDown(KeyCode.F2)) RestartGame();
if (Input.GetKeyDown(KeyCode.F3)) AddDebugCP();
```

## Future Architecture Improvements

### 1. Command Pattern for Actions

```csharp
interface ICommand
{
    void Execute();
    void Undo();
}

class PlaceUnitCommand : ICommand
{
    // Allows undo/redo system
}
```

### 2. State Machine for Game States

```csharp
class GameStateMachine
{
    IState currentState;
    // Better than enum-based state
}
```

### 3. Service Locator Pattern

```csharp
ServiceLocator.Get<ResourceManager>()
// Instead of singleton access
```

### 4. Object Pooling System

```csharp
class ObjectPool<T>
{
    Queue<T> pool;
    T Get();
    void Return(T obj);
}
```

## Code Style Guidelines

### Naming Conventions

- **Classes**: PascalCase (`WaveManager`)
- **Methods**: PascalCase (`TryPlaceUnit`)
- **Fields**: camelCase (`currentHealth`)
- **SerializeFields**: camelCase (`[SerializeField] private int maxCP`)
- **Events**: OnPascalCase (`OnWaveStarted`)

### Documentation

- Use XML comments for public APIs
- Use `//` comments for complex logic
- Use `Debug.Log` with system tags: `[SystemName] message`

### Error Handling

```csharp
// Always check for null
if (ResourceManager.Instance == null)
{
    Debug.LogError("[UnitManager] ResourceManager not found!");
    return false;
}

// Use TryX pattern for operations that can fail
public bool TrySpendCP(int amount)
{
    if (currentCP >= amount)
    {
        // Success path
        return true;
    }
    return false; // Failure
}
```

---

**This architecture is designed to be:**
- ✅ Easy to understand
- ✅ Easy to extend
- ✅ Easy to debug
- ✅ Performant for mobile/web
