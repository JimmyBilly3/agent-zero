# Quick Start Guide - Ashes & Sakura

## Immediate Setup (5 minutes)

### 1. Open in Unity
```
Unity Hub > Add > Select "AshesAndSakura" folder
Open with Unity 2021.3 LTS or newer
```

### 2. Create Main Scene

**Hierarchy Setup:**
1. Delete default objects if any
2. Right-click in Hierarchy → Create Empty → Name: "GameManager"
3. Attach script: `Assets/Scripts/Managers/GameManager.cs`
4. Create more empties under a "Managers" parent:
   - ResourceManager (+ script)
   - WaveManager (+ script)
   - UnitManager (+ script)
   - SkillManager (+ script)
   - EnemySpawner (+ script)
   - BattlefieldGrid (+ script)

### 3. Quick Prefab Creation

**Unit Prefab (2 minutes):**
```
1. Create GameObject → 2D Object → Sprite → Name: "Unit"
2. Add Component: "Unit" script
3. Add Component: Box Collider 2D
4. Set Tag: "Unit"
5. Drag to Assets/Prefabs/Units/
```

**Enemy Prefab (2 minutes):**
```
1. Create GameObject → 2D Object → Sprite → Name: "Enemy"
2. Add Component: "Enemy" script
3. Add Component: Box Collider 2D
4. Add Component: Rigidbody 2D (set Kinematic)
5. Set Tag: "Enemy"
6. Change sprite color to red
7. Drag to Assets/Prefabs/Enemies/
```

### 4. Wire Up UnitManager

1. Select "UnitManager" in Hierarchy
2. In Inspector:
   - Unit Data Json: Drag `Assets/Data/UnitData.json`
   - Unit Prefab: Drag your Unit prefab

### 5. Wire Up EnemySpawner

1. Select "EnemySpawner" in Hierarchy
2. In Inspector:
   - Enemy Prefabs: Add 1 element, drag Enemy prefab

### 6. Create Basic UI (Optional for now)

Skip for minimal test, or:
```
1. Right-click Hierarchy → UI → Canvas
2. Create → UI → Text - TextMeshPro (install if prompted)
3. Position text for CP display
4. Attach UIManager script to Canvas
5. Wire up text references
```

### 7. Press Play!

**What should happen:**
- Console shows: `[GameManager] Game initialized`
- Console shows: `[WaveManager] Wave 1 started!`
- Console shows: `[ResourceManager] Added 5 CP...` (every second)
- Enemies should spawn from the right after 5 seconds
- See green placement zone in Scene view

**Debug Controls:**
- F1: Start game
- F2: Restart game
- ESC: Pause

## Testing Unit Placement (Manual for now)

Since we don't have UI set up yet, test via script:

Add to Update() in GameManager.cs temporarily:
```csharp
// Test placement with Space key
if (Input.GetKeyDown(KeyCode.Space))
{
    Vector3 testPos = new Vector3(-5f, 0f, 0f);
    UnitManager.Instance.TryPlaceUnit("soldier_basic", testPos, 0);
}
```

Press SPACE in Play mode to place a unit!

## Common Issues

**"UnitData.json not found"**
- Make sure you assigned it in UnitManager Inspector

**"No enemies spawning"**
- Check EnemySpawner has enemy prefabs assigned
- Look for `[EnemySpawner]` logs in Console

**"Can't place units"**
- Check CP is available (Console logs)
- Make sure position is in green placement zone

## What's Working

✅ Wave system with difficulty scaling
✅ Enemy spawning with lane system
✅ CP generation and spending
✅ Unit placement logic
✅ Combat system (units auto-attack enemies)
✅ Skills system (airstrike, heal, summon)
✅ Debug logging for everything

## Next Steps

1. **Add UI** - Create proper unit selection buttons with DragDropHandler
2. **Add Art** - Replace placeholder sprites with your anime/manga assets
3. **Test Skills** - Wire up skill buttons in UI
4. **Polish** - Add health bars, VFX, sound

---

**You're ready to go!** The core game loop is complete. Just add your art and UI! 🎮
