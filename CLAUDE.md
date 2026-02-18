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
    Audio/          # Audio manager, SFX library, audio config
    Input/          # Input handling
  Prefabs/          # Only when visual complexity justifies it
  Fonts/            # Press Start 2P
  Resources/        # Runtime-loaded assets if needed
```

### Code-First Approach
All UI is built in code. No manual scene configuration. Style/config classes hold all visual properties.

---

## UI Layout (Battle Screen)

Landscape orientation. Three vertical zones:

```
|-----------------------------------------------------|
| R:18  |  Shade's Turn  |  Shade > Goblin > Aldric   |  ← TurnInfoPanel (~7%)
|-----------------------------------------------------|
|  |------------|------------|--------------------||  |
|  | ENEMY      | ENEMY      |                    |  |
|  | back-left  | back-right |  ABILITY PANEL     |  |
|  |------------|------------|  (tabbed,           |  |  ← MainArea (~73%)
|  | ENEMY      | ENEMY      |   scrollable list)  |  |
|  | front-left | front-right|                    |  |
|  |============|============|                    |  |
|  | PLAYER     | PLAYER     |                    |  |
|  | front-left | front-right|                    |  |
|  |------------|------------|                    |  |
|  | PLAYER     | PLAYER     |                    |  |
|  | back-left  | back-right |                    |  |
|  |------------|------------|--------------------||  |
|-----------------------------------------------------|
| > Shade attacks Goblin      |  [<<]  Confirm?  [OK] |  ← BottomArea (~20%)
| > Goblin misses!            | "Crushing Blow > Rat" |     Left = CombatLog
| > ...                       |                       |     Right = SelectionPanel
|-----------------------------------------------------|
```

- **Top strip (~7%):** TurnInfoPanel. Round number, active character name (colored by class), turn order sequence.
- **Left (~45%):** Battlefield. Two 2x2 grids — enemies top, players bottom. Frontlines face each other at the `===` divider. Each cell = character card (name/class, HP/energy/mana bars). Tap-hold for details popup, tap to select target.
- **Right (~55%):** Ability panel. Full height of main area. Tabs: Attacks, Skills, Spells, Items, Generic. Scrollable ability list.
- **Bottom-left (~55%):** Combat log. Scrollable sequential "Pokemon-style" text feedback. Always visible.
- **Bottom-right (~45%):** Selection panel. Cancel/Confirm buttons + contextual prompts ("Select an ability", "Crushing Blow > Ratman").
- **Popups:** Character details, ability details, turn order details. Centered overlay partially covering screen. All triggered by long-press (hold).

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

## Current State (2026-02-13)

### What's Built
- **Core layer:** Enums, CharacterStats, GameplayConfig, UIStyleConfig, GameEvents (event bus), StatCalculator (all stat/damage formulas)
- **Character system:** CharacterData, BattleCharacter (runtime combat state with action points, status effects, LastSpellElement), ClassDefinitions (6 classes with base stats, basic abilities, passives, generic abilities), EquipmentData
- **Ability system:** AbilityData with factory methods + AbilityTag enum for routing special abilities, ExcludeSelf support
- **Enemy system:** EnemyDefinitions (Ratman, Goblin Archer, Minotaur), EnemyType enum for type-safe enemy creation
- **Status effect system:**
  - StatusEffectInstance (lightweight: type, duration, value, source)
  - StatusEffectProcessor (static utility: turn start/end processing, shield absorption, mark bonus, aggro modifiers)
  - 6 effects: Shield, Mark, Protect, Hide, Anticipate, Prepare
- **Battle engine:**
  - BattleManager (~170 lines, orchestration: battle loop, turn cycling, state transitions, effect ticking, victory/defeat)
  - PlayerInputHandler (player input staging: SelectingAbility → SelectingTarget → AwaitingConfirmation, tap-to-confirm)
  - BattleVisualController (UI highlight management during battle: target selection, staged highlights)
  - TurnOrderCalculator (priority groups → initiative sorting)
  - ActionExecutor (tag-based ability routing, per-hit resolution with shield absorption + mark bonus, all special abilities implemented)
  - TargetSelector (valid targets by TargetType with ExcludeSelf, aggro-weighted selection with Protect/Hide modifiers)
  - EnemyAI (random ability pick + aggro targeting)
  - HitResult (readonly struct for hit outcomes)
- **UI framework (all code-first, TextMeshPro):**
  - PanelBuilder (panels, borders, text, bars, buttons, vertical scroll views)
  - BattleScreenUI (canvas + EventSystem + layout assembly)
  - BattleGridUI (2x2 enemy/player grids with positioning rules, RepositionCards for swap)
  - CharacterCardUI (numeric text display: name, class/level, HP/EN/MP, aggro %, status indicators [S][M][P][H], shield value, death greying)
  - AbilityPanelUI (5 tabs: ATK/SKL/SPL/ITM/GEN, scrollable dynamic ability buttons, staged ability highlight, long-press detail popup)
  - PopupBase (shared popup infrastructure: build, show, hide, content management)
  - CharacterPopupUI (extends PopupBase, character detail sheet: stats, resources, abilities)
  - AbilityPopupUI (extends PopupBase, ability detail: description, costs, properties)
  - UIFormatUtil (shared formatting: GetClassColor, FormatAbilityCost, FormatTargetType)
  - LongPressHandler (MonoBehaviour input component: distinguishes hold vs tap on UI elements)
  - TurnInfoPanelUI (top strip: round number, active character, action points display, turn order sequence)
  - TurnOrderPopupUI (extends PopupBase, full turn order with HP and class colors)
  - SelectionPanelUI (bottom-right: Cancel/Confirm buttons + phase prompts + staged action display)
  - CombatLogUI (scrollable combat log, bottom-left, always visible, manual scroll management)
  - FontManager (loads Press Start 2P TMP asset from Resources)
- **Audio system (procedural, no assets):**
  - AudioConfig (static config: volumes, sample rate, pool size, pitch variation)
  - SFXLibrary (generates and caches 16 chiptune AudioClips at startup via waveform math)
  - AudioManager (MonoBehaviour, subscribes to GameEvents, round-robin AudioSource pool, dedicated music source)
- **Bootstrap:** GameBootstrap creates everything in code, launches test battle (4 players vs 4 enemies)

### What Works
- Full battle loop: turns cycle based on initiative/priority
- **All 5 generic abilities functional:** Swap (repositions cards), Anticipate (act first, -1 short), Prepare (bonus regen, act last), Protect (draw aggro), Hide (reduce aggro)
- **All 4 class abilities functional:** Warrior Crushing Blow, Rogue Quick Stab, Ranger Mark (damage + team-wide 10% bonus, 2 turns), Priest Word of Protection (shield absorbs damage), Wizard Magic Bolt (inherits last spell element), Warlock Ritual (HP→Mana)
- **Status effects:** Shield absorption in damage pipeline, Mark damage bonus, Protect/Hide aggro modifiers visible in % display, effects tick/expire correctly
- **Death visuals:** Defeated characters greyed out, show "DEFEATED", disabled interaction, no aggro %, no long-press
- **Confirmation system:** Tap target again to confirm, tap ability again to confirm (auto-target)
- **Action points in top bar:** 1L+1S per turn, ● for long, • for short, dimmed when spent
- Confirmation-based action staging: tap ability → tap target → Confirm/Cancel
- Cancel at any point returns to previous selection step
- Ability re-selection mid-flow works (tap different ability restarts staging)
- Auto-target abilities (Self, All) skip target selection, go straight to confirm
- Staged ability highlighted yellow in ability panel, staged target highlighted yellow on grid
- Turn info panel shows round number, active character, action points, and turn order
- Enemies auto-act with random ability + aggro-weighted targeting (respects Protect/Hide)
- Damage resolution with accuracy, dodge, crit, armor (flat), magic resist (%)
- Scrollable combat log (always visible, never hidden by prompts)
- Scrollable ability list (handles overflow when many abilities exist)
- Active character highlighted green, targetable cards highlighted cyan
- Long-press popups for characters, abilities, and turn order
- Victory/defeat detection
- Procedural chiptune SFX and looping battle music

### Known Issues
- Enemies with no usable abilities pass by zeroing their action points (works but is a workaround)
- No visual feedback yet for status effect application/removal (no floating text, no animations)
- Shield value not shown in character detail popup (only on card HP line)

## Decisions Made
- All UI built in code (code-first)
- Landscape mobile orientation
- Terminal retro aesthetic (black/white + terminal accent colors)
- Press Start 2P font (TMP, RASTER_HINTED rendering)
- New Input System (`InputSystemUIInputModule`) for touch/click
- Coroutine-based battle loop with PlayerInputPhase enum for staged confirmation flow
- Static utility classes for ActionExecutor, TargetSelector, EnemyAI, TurnOrderCalculator, StatCalculator, StatusEffectProcessor
- AbilityTag enum for routing special abilities (no string matching)
- Lightweight StatusEffectInstance (type, duration, value, source) — no full buff/debuff framework
- Protect/Hide last until character's next turn start
- Mark is team-wide 10% damage bonus, 2-turn duration, ticks on target's turn end
- Shield has no duration (persists until absorbed or replaced)
- Healing determined by TargetType (ally-targeting + BasePower > 0 = healing), unless overridden by tag
- Event-driven communication between systems via static GameEvents
- BattleManager split into 3 focused classes: BattleManager (orchestration), PlayerInputHandler (input staging), BattleVisualController (visual feedback)
- Popup system uses inheritance: PopupBase → CharacterPopupUI / AbilityPopupUI
- UIFormatUtil centralizes shared formatting (class colors, ability costs, target types)
- Procedural audio via AudioClip.Create() with waveform math — no external audio assets
- 1 long + 1 short action points per turn (reduced from 2L+1S)

## Roadmap

### Phase 1: Complete Battle -- DONE
~~1. Death visuals~~ ~~2. Status effects system~~ ~~3. Generic ability implementations~~ ~~4. Class ability fixes~~ ~~5. Advanced abilities & gameplay~~
6. **Polish pass** — battle feel, edge cases, visual consistency

### Phase 2: Visual Feedback (DOTween)
7. Damage numbers — floating text on hit/heal/miss
8. Bar animations — smooth HP/energy/mana bar transitions
9. Hit flashes & transitions — screen shake on crit, flash on damage, turn transitions
10. **Polish pass** — timing, animation feel, mobile performance

### Phase 3: Progression
11. XP / Leveling — post-battle XP, stat growth, skill unlocks (Fibonacci + passives at 5s)
12. Inventory & Equipment UI — equip/unequip, inventory management, stat previews
13. **Polish pass** — stat balance, UI flow, equipment preview feel

### Phase 4: Roguelike Loop
14. Map system — node-based map generation, branching paths
15. Encounter types — standard battles, elite fights, bosses, shops, rest sites
16. Shop system — buy/sell equipment, consumables
17. **Polish pass** — map variety, encounter balance, run pacing

### Phase 5: Final Polish & Save
18. Save system — persist run state, party, inventory
19. Full visual/UI/balance polish pass

### Next Up
Phase 1, Step 6: Polish pass (battle feel, edge cases, visual consistency)

---

## Scene Setup
- SampleScene with a single empty GameObject named "Game" with the `GameBootstrap` component
- Everything else is created in code at runtime
