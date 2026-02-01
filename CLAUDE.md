# Pixel Warriors

## Overview
Dungeon crawler roguelike. The player directs a party of 4 heroes of different classes through battles across different biomes. Leveling up, equipment, shops, elite fights, bosses — standard roguelike progression loop.

**Platform:** Mobile (landscape orientation)
**Visual Style:** Minimalistic retro. Black background, white text and panel borders. Terminal aesthetic with accent colors (magenta, cyan, green). 9-slice bordered panels with rounded corners.
**Font:** Press Start 2P
**Engine:** Unity (URP 2D), purely UI-driven — no scene-based level design

## Architecture

### Namespace
`PixelWarriors`

### Folder Structure
```
Assets/
  Scripts/
    Core/           # Game state, config, enums, events
    Battle/         # Combat engine, turn order, targeting, damage calc
    Characters/     # Character data, stats, classes, leveling
    Abilities/      # Ability definitions, tabs, effects
    Equipment/      # Items, gear, inventory
    Enemies/        # Enemy definitions, AI behavior
    UI/             # All UI construction, panels, popups
    Audio/          # Sound manager (future)
    Input/          # Input handling
  Prefabs/          # Only when visual complexity justifies it
  Fonts/            # Press Start 2P
  Resources/        # Runtime-loaded assets if needed
```

### Code-First Approach
All UI is built in code. No manual scene configuration. Style/config classes hold all visual properties.

---

## UI Layout (Battle Screen)

Landscape orientation. Two major vertical sections:

```
|-----------------------------------------------------|
|  |------------|------------|--------------------||  |
|  | ENEMY      | ENEMY      |                    |  |
|  | back-left  | back-right |                    |  |
|  |------------|------------|                    |  |
|  | ENEMY      | ENEMY      |   ABILITY PANEL    |  |
|  | front-left | front-right|   (tabbed)         |  |
|  |============|============|                    |  |
|  | PLAYER     | PLAYER     |                    |  |
|  | front-left | front-right|                    |  |
|  |------------|------------|                    |  |
|  | PLAYER     | PLAYER     |                    |  |
|  | back-left  | back-right |                    |  |
|  |------------|------------|--------------------||  |
|-----------------------------------------------------|
|  {COMBAT LOG}                                       |
|-----------------------------------------------------|
```

- **Left (~40-50%):** Battlefield. Two 2x2 grids — enemies top, players bottom. Frontlines face each other at the `===` divider. Each cell = character card (name/class, HP/energy/mana bars). Tap-hold for details popup, tap to select target.
- **Right (~50-60%):** Ability panel. Full height of battle area. Tabs: Attacks, Skills, Spells, Items, Generic.
- **Bottom strip:** Combat log. Sequential "Pokemon-style" text feedback.
- **Popups:** Inventory, character details, etc. Centered overlay partially covering screen.
- **Minor panels:** Turn order, titles — minimal footprint, integrated into core layout.

### Grid Positioning Rules
- 4 characters: fill all 4 cells
- 3 characters: 1 in one row, 2 in the other (2-char row spans full width)
- 2 characters: center, span full width
- 1 character: center of entire grid

---

## Character Stats

| Stat | Effect |
|------|--------|
| Endurance | Modifies max HP |
| Stamina | Modifies max Energy (fast-cycling physical resource) |
| Intellect | Modifies max Mana (slow-cycling magical resource) |
| Strength | Modifier for physical attacks |
| Dexterity | Physical accuracy, physical crit, dodge |
| Willpower | Magical effect chance modifier ("magical crit") |
| Armor | Flat physical damage reduction |
| Magic Resist | Percentage magical damage reduction |
| Initiative | Determines turn order |

---

## Action Point System

Each character per turn has:
- **2 Long Action Points** — used for standard abilities
- **1 Short Action Point** — used for quick actions (potions, swap position)

Long points can be spent as short points. Short points cannot be spent as long points.

---

## Ability Tabs

1. **Attacks** — Weapon-based attacks
2. **Skills** — Non-magical abilities, generally consume Energy
3. **Spells** — Magical abilities, generally consume Mana
4. **Items** — Usable items from inventory/equipment
5. **Generic** — Universal tactics all characters share:
   - Swap Position
   - Anticipate: act with priority next turn, but -1 short action
   - Prepare: recover small resources, act last next turn
   - Protect/Hide: modify chance of being hit
   - Use Item

---

## Combat Mechanics

### Targeting (Player Side)
- Frontline cells: 30% base chance to be hit
- Backline cells: 20% base chance to be hit
- Modifiers apply (e.g., "Vanguard": +10% aggro from backline if in frontline)

### Enemy Targeting AI
- Default: weighted by aggro percentages
- Keyword modifiers:
  - "Assassin": never targets highest aggro character
  - "Ranged": fixed 25% per target (unmodifiable)
  - More to come

### Turn Order
1. Calculate initiative for all characters
2. Apply priority modifiers:
   - Positive priority: always acts first (ordered by initiative within group)
   - Normal priority: standard initiative order
   - Negative priority: always acts last (ordered by initiative within group)

### Damage Model
- **Physical:** Armor provides flat subtraction. Multi-hit vs big-hit matters.
- **Magical:** Magic Resist provides percentage reduction.

---

## Classes

| Class | Role | Basic Ability |
|-------|------|---------------|
| Warrior | Frontline tank/berserker | Crushing Blow (Energy): bonus damage |
| Rogue | Versatile trickster, minor shadow magic | Quick Stab: attack using only a short action |
| Ranger | Backline marksman, traps, survival | Mark: marks target, boosting attacks against it |
| Priest | Durable holy warrior, protector | Word of Protection: shield absorbing damage for ally |
| Wizard | Pure magic, elemental/arcane, squishy | Magic Bolt: inherits element of last spell used |
| Warlock | Dark magic, life-steal, rituals, risky | Ritual: lose HP to gain Mana |

---

## Equipment Slots

- Hand 1
- Hand 2
- Armor (Chest)
- Armor (Pants)
- Armor (Helmet)
- Trinket 1
- Trinket 2

---

## Progression

- Characters gain XP after each battle
- Level up increases stats
- Learn new skill every Fibonacci level (1, 1, 2, 3, 5, 8, 13, 21...)
- Learn passive skill every multiple of 5 (5, 10, 15, 20...)
- Each character starts with 1 basic ability + 1 basic passive

---

## Enemies (Initial Set)

| Enemy | Role |
|-------|------|
| Goblin Archer | Weak backliner |
| Ratman | Weak frontliner |
| Minotaur | Powerful elite |

Enemy AI: 1-3 abilities per enemy, randomized selection and targeting for now.

---

## Current State (2026-02-01)

### What's Built
- **Core layer:** Enums, CharacterStats, GameplayConfig, UIStyleConfig, GameEvents (event bus), StatCalculator (all stat/damage formulas)
- **Character system:** CharacterData, BattleCharacter (runtime combat state with action points), ClassDefinitions (6 classes with base stats, basic abilities, passives, generic abilities), EquipmentData
- **Ability system:** AbilityData with factory methods (CreateAttack, CreateSkill, CreateSpell, CreateQuickAction)
- **Enemy system:** EnemyDefinitions (Ratman, Goblin Archer, Minotaur)
- **Battle engine:**
  - BattleManager (MonoBehaviour, coroutine-driven state machine: Setup → TurnStart → AwaitingInput/ExecutingAction → TurnEnd → Victory/Defeat)
  - TurnOrderCalculator (priority groups → initiative sorting)
  - ActionExecutor (per-hit resolution: accuracy → dodge → damage → crit, multi-hit, healing, combat logging)
  - TargetSelector (valid targets by TargetType, aggro-weighted selection for AI)
  - EnemyAI (random ability pick + aggro targeting)
  - HitResult (readonly struct for hit outcomes)
- **UI framework (all code-first, TextMeshPro):**
  - PanelBuilder (panels, borders, text, bars, buttons)
  - BattleScreenUI (canvas + EventSystem + layout assembly)
  - BattleGridUI (2x2 enemy/player grids with positioning rules)
  - CharacterCardUI (name, HP/energy/mana bars, clickable for targeting, highlight support)
  - AbilityPanelUI (5 tabs: ATK/SKL/SPL/ITM/GEN, dynamic ability buttons)
  - CombatLogUI (scrolling combat log)
  - FontManager (loads Press Start 2P TMP asset from Resources)
- **Bootstrap:** GameBootstrap creates everything in code, launches test battle (4 players vs 4 enemies)

### What Works
- Full battle loop: turns cycle based on initiative/priority
- Player can select abilities and tap enemy/ally cards as targets
- Enemies auto-act with random ability + aggro-weighted targeting
- Damage resolution with accuracy, dodge, crit, armor (flat), magic resist (%)
- Combat log shows all actions
- Active character highlighted green, targetable cards highlighted cyan
- Victory/defeat detection

### What's NOT Built Yet
- DOTween integration (bar animations, damage feedback, transitions)
- Cancel target selection (once ability is chosen, must pick target)
- Turn order display panel
- Death visuals (defeated characters still show in grid)
- XP/leveling system (formulas exist but not wired)
- Roguelike loop (map, nodes, shops, encounters)
- Equipment system (data exists but no equip/unequip/inventory UI)
- Status effects / buffs / debuffs
- Ability effects beyond raw damage/healing (Mark, shields, Anticipate/Prepare behavior)
- Sound/audio
- Save system

### Known Issues
- No cancel during target selection
- Generic abilities (Swap, Anticipate, Prepare, Protect, Hide) don't have behavior implementations yet — they execute but do nothing
- Warlock's Ritual (HP→Mana) not implemented in ActionExecutor
- Wizard's Magic Bolt element-tracking not implemented
- Enemies with no usable abilities pass by zeroing their action points (works but is a workaround)

## Decisions Made
- All UI built in code (code-first)
- Landscape mobile orientation
- Terminal retro aesthetic (black/white + terminal accent colors)
- Press Start 2P font (TMP, RASTER_HINTED rendering)
- New Input System (`InputSystemUIInputModule`) for touch/click
- Coroutine-based battle loop with boolean guards for input waiting
- Static utility classes for ActionExecutor, TargetSelector, EnemyAI, TurnOrderCalculator, StatCalculator
- Healing determined by TargetType (ally-targeting + BasePower > 0 = healing)
- Event-driven communication between systems via static GameEvents

## Scene Setup
- SampleScene with a single empty GameObject named "Game" with the `GameBootstrap` component
- Everything else is created in code at runtime
