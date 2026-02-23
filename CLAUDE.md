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
- **1 Long Action Point** — used for standard abilities
- **1 Short Action Point** — used for quick actions

Long points can be spent as short points. Short points cannot be spent as long points. Some abilities (e.g., Earthquake) consume both 1L + 1S.

---

## Ability Tabs

1. **Attacks** — Weapon-based attacks (Terror blocks this tab)
2. **Skills** — Non-magical abilities, generally consume Energy
3. **Spells** — Magical abilities, generally consume Mana (Silence blocks this tab)
4. **Items** — Usable items from inventory/equipment
5. **Generic** — Universal tactics all characters share:
   - Attack: basic weapon attack (Long)
   - Swap Position (Long)
   - Anticipate: act first next turn, -1 short action (Short)
   - React: act last next turn (Short)
   - Hide: lower aggro until next turn (Short)
   - Taunt: increase aggro 2 turns, frontline only (Short)
   - Pass: end activation (Short, 0 cost)

---

## Combat Mechanics

### Ability Range
- **Close:** Can only target frontline enemies (unless user has Levitate)
- **Reach:** Can target any enemy position
- **Any:** No range restriction (default)

### Targeting (Player Side)
- Frontline cells: 30% base chance to be hit
- Backline cells: 20% base chance to be hit
- Aggro modifiers: Taunt (2x), Conceal (0x), Levitate/Hide (0.25x)
- If all weights are 0 (all concealed), fallback to equal distribution

### Enemy Targeting AI
- Default: weighted by aggro percentages
- Mass Confusion: 33% chance to redirect attacks to fellow enemies (real damage/defense calcs)
- Keyword modifiers: "Assassin", "Ranged"

### Turn Order
1. Calculate initiative for all characters
2. Apply priority modifiers:
   - Positive priority: always acts first (Anticipate)
   - Normal priority: standard initiative order
   - Negative priority: always acts last (React, Chilled)

### Damage Model
- **Physical:** Armor provides flat subtraction. Block negates entire hit.
- **Magical:** Magic Resist provides percentage reduction.
- **Block:** Shield equipment gives base block %. Defensive Stance uses block chance (costs energy). Block ability = guaranteed next block. Berserker Stance cannot block.

### Status Effects
| Effect | Type | Description |
|--------|------|-------------|
| Shield | Buff | Absorbs damage, no duration |
| Mark | Debuff | +10% damage taken, 2 turns |
| Taunt | Aggro | 2x aggro, 2 turns |
| Hide | Aggro | 0.25x aggro, until next turn start |
| Conceal | Aggro | 0x aggro, indefinite (removed by attacking from concealment) |
| Bleed | DoT | Stackable, 3 damage/stack/turn, 3 turns |
| Poison | DoT | 4 damage/turn, halves healing, 5 turns |
| Burn | DoT | 6 damage/turn, 3 turns |
| Chilled | Debuff | Act last (negative priority), 2 turns |
| Stun | Debuff | Skip turn entirely, 1 turn |
| Silence | Debuff | Cannot use Spells tab |
| Terror | Debuff | Cannot use Attacks tab, 1 turn |
| Block | Buff | Guaranteed block on next physical hit |
| Levitate | Buff | Lower aggro, Close abilities hit any target, 3 turns |
| Imbue | Buff | +3 bonus damage on attacks, 3 turns |
| Envenom | Buff | Attacks apply Poison, 3 turns |
| Stances | Exclusive | Defensive/Brawling/Berserker, only one at a time, drain energy |
| Confusion | Debuff | 33% chance enemy targets own allies, 2 turns |
| CorpseExplosion | Mark | AoE damage on death |
| LeechLife | DoT | Drains HP to caster each turn |
| Caltrops | Buff | Enemies take damage when changing position |
| UltimateReflexes | Buff | Dodge all attacks, 1 turn |

---

## Classes

| Class | Role | Starting Ability |
|-------|------|-----------------|
| Warrior | Frontline tank/berserker | Crush Armor: damage + armor reduction |
| Rogue | Versatile trickster | Quick Stab: attack using only a short action |
| Ranger | Backline marksman | Mark: boosts accuracy against target (deferred) |
| Priest | Durable holy protector | Word of Protection: shield absorbing damage (deferred) |
| Elementalist | Elemental/arcane mage | Energy Bolt: inherits element of last spell used |
| Warlock | Dark magic, life-steal | Ritual: lose HP to gain Mana |

### Warrior Abilities (11 + 4 passives)
Crush Armor, Bulwark, Defensive/Brawling/Berserker Stances, Cleave, Second Wind, Block, Bodyguard, Bladedance

### Rogue Abilities (10 + 7 passives)
Quick Stab, Sucker Punch, Ambush, Vanish, Envenom, Ultimate Reflexes, Dagger Throw, Assassination, Powder Bomb, Caltrops

### Elementalist Abilities (14)
Energy Bolt, Ignite, Earthquake, Steam Beam, Wave Crash, Levitate, Seal of Elements, Arcane Burst, Splinters, Invisibility, Meltdown, Elemental Armor, Imbue Staff. Elements: Fire, Earth, Water, Air, Arcane.

### Warlock Abilities (8 + 3 passives)
Ritual, Terror, Curse, Hex, Consume, Mass Confusion, Corpse Explosion, Leech Life

---

## Equipment Slots

- Hand 1 (weapon)
- Offhand (shield, etc.)
- Head
- Body
- Trinket 1
- Trinket 2

### Weapon Types
Sword, Dagger, TwoHanded, Shield, Staff, Bow, Mace. Some abilities require specific weapon types (e.g., Ritual requires Dagger).

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

## Current State (2026-02-23, updated)

### What's Built
- **Core layer:** Enums (expanded with AbilityRange, WeaponType, 29 StatusEffects, 50+ AbilityTags), CharacterStats, GameplayConfig (DoT constants, stance constants, block/assassination/confusion params), UIStyleConfig, GameEvents (event bus), StatCalculator
- **Character system:** CharacterData (6 equipment slots, HasWeaponType/GetBaseBlockChance helpers), BattleCharacter (UsedOnceAbilities tracking, stackable/exclusive AddEffect, Close/Reach CanUseAbility checks, additive AP consumption, Terror/Silence blocking), ClassDefinitions (uses AbilityCatalog), EquipmentData (WeaponType, BaseBlockChance)
- **Ability system:**
  - AbilityData with Range, OncePerBattle, RequiresConcealed, RequiredWeapon, RequiresFrontline
  - AbilityCatalog (master catalog: all abilities per class, generic abilities)
  - PassiveProcessor (OnBattleStart, OnTurnStart, OnDamageTaken, OnCharacterDefeated hooks)
  - Per-class ability handlers: WarriorAbilityHandler, RogueAbilityHandler, ElementalistAbilityHandler, WarlockAbilityHandler
- **Enemy system:** EnemyDefinitions (Ratman/Goblin Archer/Minotaur with Close/Reach ranges), EnemyAI (Mass Confusion redirect)
- **Status effect system:**
  - StatusEffectInstance (type, duration, value, source)
  - StatusEffectProcessor (DoT processing: Bleed/Poison/Burn/LeechLife, Stun/Silence/Chilled, stance energy drain, aggro modifiers for Conceal/Taunt/Levitate/Hide)
  - 29 status effects fully implemented
- **Battle engine:**
  - BattleManager (~250 lines, orchestration + passive event hooks + caltrops)
  - PlayerInputHandler (staged confirmation flow)
  - BattleVisualController (highlight management)
  - TurnOrderCalculator, TargetSelector (Close/Reach filtering, concealment fallback)
  - ActionExecutor (tag-based routing to per-class handlers, block mechanic, ProcessPostHitEffects for stance triggers/Envenom, Imbue bonus damage)
  - EnemyAI (Mass Confusion: redirect targeting to own team)
  - HitResult (Miss/Dodge/Block/Hit)
- **UI framework (all code-first, TextMeshPro):**
  - PanelBuilder, BattleScreenUI, BattleGridUI
  - CharacterCardUI (expanded status indicators: [T][C][B][Po][Fi][Ch][!][X][Df][Br][Bk][Bl] etc.)
  - AbilityPanelUI (5 tabs, [C]/[R] range tags on buttons)
  - AbilityPopupUI (range, weapon requirements, once-per-battle, frontline-only, concealed requirement display)
  - UIFormatUtil (FormatAbilityRange, improved FormatAbilityCost with AP indicators)
  - PopupBase → CharacterPopupUI / AbilityPopupUI / TurnOrderPopupUI
  - TurnInfoPanelUI, SelectionPanelUI, CombatLogUI
  - LongPressHandler, FontManager
- **Audio system (procedural):** AudioConfig, SFXLibrary, AudioManager
- **Bootstrap:** GameBootstrap with `_useTestBattle` toggle: test battle (4v4) or full game loop
- **Screen system (Phase A):**
  - IScreen interface (Build/Show/Hide/Destroy)
  - ScreenManager (owns Canvas + EventSystem, manages screen transitions)
  - BattleScreenUI refactored from MonoBehaviour to plain IScreen class
  - MainMenuScreen (title, "New Run" button)
  - GameStateManager (coroutine-based game flow: menu → battle → post-battle loop)
  - RunData (persistent run state: act, floor, gold, party, fallen, inventory)
  - RunConfig (static run parameters)
  - BattleResult enum + BattleManager exposes Result property
- **Leveling + Post-Battle (Phase B):**
  - LevelingSystem (Fire Emblem growth rolls, Fibonacci ability unlocks, passive unlocks every 5 levels)
  - GrowthRates (per-class growth rates 0-100 per stat)
  - CharacterData with persistent HP/energy/mana, GrowthModifiers, ResourcesInitialized
  - BattleCharacter uses persistent resources from CharacterData, SyncToData() writes back
  - ClassDefinitions: CreateCharacter (1 ability + generics), CreateCharacterAllAbilities (test mode)
  - PostBattleProcessor (XP, gold, permadeath, partial heal, room type multipliers)
  - PostBattleScreen (victory screen: XP bars, level ups, stat gains, new abilities, fallen, gold)
- **Room Choice + Floor Progression (Phase C):**
  - FloorGenerator (weighted room pool, boss on floor 7, shop guaranteed on floor 4, no duplicate shops)
  - RoomChoiceScreen (act/floor header, gold, party summary, 2 room cards with symbols/descriptions)
  - EncounterGenerator (normal/elite/boss encounters, stat + weapon scaling per floor, party-size matching)
  - RunConfig expanded with encounter sizing, floor stat scaling, elite/boss multipliers
  - GameStateManager: room choice → branch by type → battle or stub. Rest heals party. PreviousRoom tracking.

### What Works
- Full battle loop with all new mechanics
- **7 generic abilities:** Attack, Swap (Long), Anticipate, React, Hide, Taunt (frontline only, 2 turns), Pass
- **43+ class abilities across 4 classes** (Warrior 11, Rogue 10, Elementalist 14, Warlock 8) + Ranger Mark, Priest Word of Protection
- **14 passives** (Warrior 4, Rogue 7, Warlock 3) — hooks wired into battle loop
- **Close/Reach targeting:** Close abilities only hit frontline, Reach can hit backline
- **Block mechanic:** Shield-based block chance, Defensive Stance block, guaranteed Block ability, Berserker cannot block
- **DoTs:** Bleed (stackable), Poison (reduces healing), Burn, LeechLife (heals caster)
- **Stun** (skip turn), **Silence** (no spells), **Terror** (no attacks), **Chilled** (act last)
- **Warrior stances:** Exclusive (Defensive/Brawling/Berserker), energy drain, triggered on damage
- **Concealment system:** Vanish→Ambush combo, 0 aggro, Strategist passive, Escape Plan passive
- **Envenom:** Attacks apply Poison while active
- **Imbue:** Bonus damage on all attacks while active
- **Caltrops:** Enemies take damage on position change (wired via OnPositionSwapped)
- **Mass Confusion:** Enemies redirect attacks to fellow enemies
- **Corpse Explosion / Soul Harvest:** Triggered on character death
- **Elementalist energy bolt:** Changes effect based on last spell element used
- Equipment with weapon types, block chances
- Ability popup shows range, weapon requirements, once-per-battle, frontline-only
- All previous UI features (staging, popups, combat log, etc.)
- Procedural chiptune SFX and looping battle music

### Known Issues
- Enemies with no usable abilities pass by zeroing action points (intentional workaround)
- No visual feedback for status effect application/removal (no floating text, no animations)
- Shield value not shown in character detail popup (only on card HP line)
- Custom ability handlers (CrushArmor, SuckerPunch, etc.) don't call ProcessPostHitEffects — stance triggers/Envenom only apply through ExecuteDamage path
- Ranger and Priest ability sets are minimal (deferred)

## Decisions Made
- All UI built in code (code-first)
- Landscape mobile orientation
- Terminal retro aesthetic (black/white + terminal accent colors)
- Press Start 2P font (TMP, RASTER_HINTED rendering)
- New Input System (`InputSystemUIInputModule`) for touch/click
- Coroutine-based battle loop with PlayerInputPhase enum for staged confirmation flow
- Static utility classes: ActionExecutor, TargetSelector, EnemyAI, TurnOrderCalculator, StatCalculator, StatusEffectProcessor, PassiveProcessor
- Per-class ability handlers to keep ActionExecutor under 300 lines
- AbilityTag enum for routing (no string matching). Passives use string matching via PassiveProcessor
- AbilityCatalog as master ability definition source
- Lightweight StatusEffectInstance (type, duration, value, source)
- Stackable effects (Bleed) always add new instances; stances are exclusive group
- Block = full damage negation (not reduction)
- Action point costs are additive (LongPointCost + ShortPointCost consumed together)
- Mass Confusion: enemies redirect attacks to fellow enemies with real damage/defense calcs
- Berserker Stance: gains energy when taking damage (being hit)
- Event-driven communication via static GameEvents
- BattleManager subscribes to OnDamageDealt/OnCharacterDefeated/OnPositionSwapped for passive/caltrops hooks
- Recursion guard on defeat processing (corpse explosion chain)
- 1 long + 1 short action points per turn
- Elements: Fire, Earth, Water, Air, Arcane, None
- Combat formulas: weapon-based physical (STR + weapon * multiplier - armor), WIL-scaled spells, DEX hit for weapons, INT hit for spells, armor/magic pen
- HitChanceModifier per ability for imprecise attacks (PowderBomb -0.10, Mass Confusion -0.15)
- Screen system: IScreen interface, ScreenManager owns canvas, GameStateManager drives flow
- BattleScreenUI is a plain class (not MonoBehaviour), built under ScreenManager's canvas
- GameBootstrap has `_useTestBattle` toggle for dev convenience
- Roguelike run: room-by-room choice, partial heal, permadeath, start with 2 recruit to 4, 3 acts x 7 floors

## Roadmap

### Phase 1: Complete Battle -- DONE
~~1. Death visuals~~ ~~2. Status effects~~ ~~3. Generic abilities~~ ~~4. Class abilities~~ ~~5. Advanced abilities~~
~~5b. Battle system rework~~ ~~5c. Combat formula rework (weapon damage, WIL scaling, unified hit, penetration)~~

### Phase 2: Roguelike Loop (current)
- **A. Foundation** — Screen system, game state machine, main menu ← **DONE**
- **B. Leveling + Post-Battle** — XP, Fire Emblem growth, ability unlocks, post-battle screen ← **DONE**
- **C. Room Choice + Floor Progression** — Room generation, floor/act advancement ← **DONE**
- **D. Enemy Roster + Scaling** — New enemies, scaling, bosses
- **E. Equipment + Loot** — Procedural equipment, loot tables
- **F. Inventory + Shop** — Equipment management, buy/sell
- **G. Events + Rest + Recruitment** — Random events, rest sites, party setup, recruitment

### Phase 3: Visual Feedback (DOTween) — deferred
### Phase 4: Final Polish & Save — deferred

### Next Up
Phase 2D: Enemy Roster + Scaling

---

## Scene Setup
- SampleScene with a single empty GameObject named "Game" with the `GameBootstrap` component
- Everything else is created in code at runtime
