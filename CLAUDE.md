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
| Regeneration | HoT | Heals 4/turn, 3 turns (Prayer of Mending) |
| Blessing | Buff | +20% damage, 3 turns |
| DivineIntervention | Buff | Immune to all damage, 1 turn |
| Pin | Debuff | Cannot swap/reposition, 2 turns |
| HuntersFocus | Buff | +3 bonus damage on attacks, 3 turns |
| IronWill | Buff | Blocks all negative status effects, 2 turns |
| FrozenTomb | Debuff | Stunned + immune to damage, 2 turns |
| SoulLink | Debuff | 30% of damage splashes to other enemies, 3 turns |
| DrainSoul | DoT | Escalating damage (3→5→7), heals caster, 3 turns |
| Trap | Buff | Next enemy that moves takes 8 dmg + stun (single-use) |

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

### Warrior Abilities (13 + 4 passives)
Crushing Blow, Powerful Bash, Crush Armor, Bulwark, Defensive/Brawling/Berserker Stances, Cleave, Second Wind, Block, Bodyguard, Bladedance, Rally Cry (team +1S action), Iron Will (status immunity 2 turns)

### Rogue Abilities (12 + 7 passives)
Quick Stab, Sucker Punch, Ambush, Vanish, Envenom, Ultimate Reflexes, Dagger Throw, Assassination, Powder Bomb, Caltrops, Fan of Knives (AoE + poison), Shadow Step (reach dagger attack)

### Ranger Abilities (8 + 3 passives)
Mark, Volley (AoE ranged), Snipe (1.8x +crit), Barrage (3-hit), Hunter's Focus (+3 dmg buff), Trap (dmg+stun on move), Pin (block movement), Tracking Shot (scales with enemy count)
Passives: Survivalist (+2EN/turn), Predator (+15% vs marked), Steady Aim (+5% hit/crit ranged)

### Priest Abilities (8 + 3 passives)
Word of Protection, Smite, Prayer of Mending (HoT), Holy Ward (group shield), Purify (cleanse), Resurrect (revive dead ally, once/battle), Blessing (+20% dmg buff), Divine Intervention (1-turn immunity, once/battle)
Passives: Inner Light (+2MP/turn), Martyr (prevent ally death once/battle), Devotion (healing grants shield)
Basic passive: Faith (+20% healing)

### Elementalist Abilities (15)
Energy Bolt, Ignite, Earthquake, Steam Beam, Wave Crash, Levitate, Seal of Elements, Arcane Burst, Splinters, Invisibility, Meltdown, Elemental Armor, Imbue Staff, Chain Lightning (bounces to 2 others at 50%), Frozen Tomb (stun + immune). Elements: Fire, Earth, Water, Air, Arcane.

### Warlock Abilities (10 + 3 passives)
Ritual, Terror, Curse, Hex, Consume, Mass Confusion, Corpse Explosion, Leech Life, Soul Link (30% damage splash), Drain Soul (escalating DoT + heals caster)

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

## Enemies (~46 total)

### Act 1 (Sewers/Catacombs) — 12 enemies
| Type | Role | Identity |
|------|------|----------|
| Ratman | Frontline | Basic physical, claws/bite |
| Skeleton | Frontline | Sword + shield, can block |
| Zombie | Frontline | High HP, very slow, hits hard |
| Fungus Creeper | Frontline | Poison on hit, spore cloud AoE |
| Goblin Archer | Backline | Bow, poison arrow |
| Swarm Bat | Backline | Multi-hit (3x), chips shields |
| Tunnel Rat | Backline | Healer, priority kill target |
| Minotaur | Elite | AoE stomp + gore, enrage |
| Giant Spider | Elite | Stun (web) + poison, crowd control |
| Bone Lord | Elite | Gang leader, dark magic |
| Goblin King | Boss | Mace + shield, sweeping strike |
| Catacomb Guardian | Boss | Very high armor, AoE slam, enrages at low HP |

### Act 2 (Wilderness/Fortress) — 15 enemies
| Type | Role | Identity |
|------|------|----------|
| Spider | Frontline | Poison bite + web spit |
| Bandit | Frontline | Fast dagger, cheap shot, knife throw |
| Orc Warrior | Frontline | Tank, shield wall, reckless swing |
| Stone Sentinel | Frontline | Extreme armor, low HP — test magic/pen |
| Berserker Cultist | Frontline | Gets stronger, frenzy AoE |
| Dark Mage | Backline | Shadow bolt, curse |
| Herbalist Shaman | Backline | Heals allies, purifies |
| Crossbow Bandit | Backline | High single-target ranged physical |
| Fire Imp | Backline | Burn DoT, low HP |
| Orc Brute | Elite | Cleave AoE + overhead smash |
| Wyvern Knight | Elite | High mobility, aerial strikes |
| Necromancer Adept | Elite | DoTs + death bolt + dark ritual |
| Blade Dancer | Elite | Multi-hit (3x), precise strike |
| Minotaur Lord | Boss | Devastating gore, earthquake AoE |
| Bandit Warlord | Boss | Power strike, rallies troops |

### Act 3 (Dark Citadel) — 14 enemies
| Type | Role | Identity |
|------|------|----------|
| Dark Knight | Frontline | High armor+MR, counter stance |
| Abyssal Golem | Frontline | Very high MRS, slow, must use physical |
| Plague Bringer | Frontline | AoE poison, rot |
| Death Cultist | Frontline | Sacrifices HP for massive damage |
| Chain Devil | Frontline | Hook (reach), constrict |
| Shadow Assassin | Backline | High DEX/INI, throat slit (2x), fade |
| Lich Acolyte | Backline | Silence + shadow bolt |
| Blood Mage | Backline | Self-damages for powerful AoE hemorrhage |
| Void Speaker | Backline | AoE void scream + mind blast |
| Vampire Lord | Elite | Drain strike, blood feast, self-heal |
| Twin Wraith | Elite | Multi-hit spectral barrage, shadow bond |
| Demon Champion | Elite | Mixed physical/magical, AoE + single target |
| The Lich | Boss | Death bolt, soul drain AoE, dark pact |
| Arch Demon | Boss | Cataclysm AoE, doom strike (2.2x), hellfire |

Enemy AI: 1-3 abilities per enemy, randomized selection and targeting. Bosses are flagged with `IsBoss` for HP-threshold reinforcement triggers.

### Reinforcement System
Battles can have reinforcement waves that spawn mid-battle:
- **Normal battles**: 40% chance of 1 wave (1-2 enemies) when alive enemies <= 1
- **Elite battles**: 1-2 waves of regular adds when alive enemies <= 1 (gang leader pattern)
- **Boss battles**: Waves at 75% and 40% boss HP thresholds (+ 20% for Act 3)
- Triggers: `OnEnemyCount`, `OnRoundNumber`, `OnBossHPPercent`
- Reinforcements join next round's turn order (not current round)
- UI shows wave count indicator `[+N]` in turn info panel
- Dead card positions are reused by spawning reinforcements

---

## Current State (2026-02-26, updated)

### What's Built
- **Core layer:** Enums (expanded with AbilityRange, WeaponType, 29 StatusEffects, 50+ AbilityTags), CharacterStats, GameplayConfig (DoT constants, stance constants, block/assassination/confusion params), UIStyleConfig, GameEvents (event bus), StatCalculator
- **Character system:** CharacterData (6 equipment slots, HasWeaponType/GetBaseBlockChance helpers), BattleCharacter (UsedOnceAbilities tracking, stackable/exclusive AddEffect, Close/Reach CanUseAbility checks, additive AP consumption, Terror/Silence blocking), ClassDefinitions (uses AbilityCatalog), EquipmentData (WeaponType, BaseBlockChance)
- **Ability system:**
  - AbilityData with Range, OncePerBattle, RequiresConcealed, RequiredWeapon, RequiresFrontline
  - AbilityCatalog (master catalog: all abilities per class, generic abilities)
  - PassiveProcessor (OnBattleStart, OnTurnStart, OnDamageTaken, OnCharacterDefeated hooks)
  - Per-class ability handlers: WarriorAbilityHandler, RogueAbilityHandler, RangerAbilityHandler, PriestAbilityHandler, ElementalistAbilityHandler, WarlockAbilityHandler
- **Enemy system:** EnemyDefinitions (router) → Act1Enemies/Act2Enemies/Act3Enemies (~46 types), EncounterGenerator (act-based pools, scaling, reinforcement waves), EncounterData + ReinforcementWave, EnemyAI (Mass Confusion redirect)
- **Status effect system:**
  - StatusEffectInstance (type, duration, value, source)
  - StatusEffectProcessor (DoT processing: Bleed/Poison/Burn/LeechLife, Stun/Silence/Chilled, stance energy drain, aggro modifiers for Conceal/Taunt/Levitate/Hide)
  - 39 status effects fully implemented (10 new: Regeneration, Blessing, DivineIntervention, Pin, HuntersFocus, IronWill, FrozenTomb, SoulLink, DrainSoul, Trap)
- **Battle engine:**
  - BattleManager (~350 lines, orchestration + passive event hooks + caltrops/trap processing + reinforcement spawning)
  - PlayerInputHandler (staged confirmation flow)
  - BattleVisualController (highlight management)
  - TurnOrderCalculator, TargetSelector (Close/Reach filtering, concealment fallback, SingleDeadAlly for Resurrect)
  - ActionExecutor (tag-based routing to 6 class handlers, block mechanic, ProcessPostHitEffects for stance triggers/Envenom/SoulLink splash, damage modifiers: Imbue/HuntersFocus/Blessing/Predator, battle context for cross-system access, Martyr passive in CheckDefeated)
  - EnemyAI (Mass Confusion: redirect targeting to own team)
  - HitResult (Miss/Dodge/Block/Hit)
- **UI framework (all code-first, TextMeshPro):**
  - PanelBuilder, BattleScreenUI, BattleGridUI (AddEnemy for mid-battle spawning)
  - CharacterCardUI (expanded status indicators: [T][C][B][Po][Fi][Ch][!][X][Df][Br][Bk][Bl][Re][+][DI][Pi][HF][IW][FT][SL][DS][Tr], SetResurrectable for dead-ally targeting, revive visual reset)
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
- **Phase D — Reinforcement Mechanic + Expanded Enemy Roster:**
  - EncounterData + ReinforcementWave classes (trigger types: OnEnemyCount, OnRoundNumber, OnBossHPPercent)
  - GridSlotUtil (grid slot management, character placement)
  - BattleGridUI.AddEnemy (mid-battle card creation, dead card replacement)
  - EncounterGenerator returns EncounterData with waves, ScaleStats is public
  - BattleManager: CheckAndSpawnReinforcements, wave tracking, modified victory (all dead + no pending waves)
  - GameStateManager uses EncounterData + GridSlotUtil flow
  - TurnInfoPanelUI shows wave count indicator [+N]
  - SFXType.Reinforcements + AudioManager subscription
  - EnemyDefinitions split into Act1/Act2/Act3Enemies (~46 enemy types total)
  - 2 bosses per act for randomization (BossPools array)
- **Phase F — Shop + Consumables:**
  - ConsumableData, ConsumableStack, ConsumableCatalog (~22 consumables: 3 potions, 2 curatives, 3 bombs, 2 utility, 6 scrolls, 6 books)
  - ShopConfig (economy constants), ShopStock (shop state), ShopGenerator (procedural stock: 4 equipment + 5 consumables + 20% book chance)
  - ShopScreen (IScreen: Buy Gear / Buy Items / Sell tabs, reroll button, inventory access)
  - AbilityPanelUI Items tab reads from RunData.Consumables via ConsumableCatalog.GetBattleAbility()
  - ActionExecutor: 6 consumable handlers (ConsumableHeal, ConsumableEnergyRestore, ConsumableManaRestore, ConsumableAntidote, ConsumableBandage, ConsumableSmokeBomb)
  - BattleManager: decrements consumable after use, refreshes ability panel
  - InventoryScreen: Equipment/Items mode toggle, consumable stacks display, book teaching (per-character Teach buttons, duplicate prevention)
  - GameStateManager.ShopPhase() with shop-inventory loop

### What Works
- Full battle loop with all new mechanics
- **7 generic abilities:** Attack, Swap (Long), Anticipate, React, Hide, Taunt (frontline only, 2 turns), Pass
- **66 class abilities across 6 classes** (Warrior 13, Rogue 12, Ranger 8, Priest 8, Elementalist 15, Warlock 10)
- **20 passives** (Warrior 4, Rogue 7, Ranger 3, Priest 3, Warlock 3) + 6 basic passives (Tough, Evasive, Keen Eye, Faith, Arcane Affinity, Dark Pact)
- **Close/Reach targeting:** Close abilities only hit frontline, Reach can hit backline
- **Block mechanic:** Shield-based block chance, Defensive Stance block, guaranteed Block ability, Berserker cannot block
- **DoTs:** Bleed (stackable), Poison (reduces healing), Burn, LeechLife (heals caster)
- **Stun** (skip turn), **Silence** (no spells), **Terror** (no attacks), **Chilled** (act last)
- **Warrior stances:** Exclusive (Defensive/Brawling/Berserker), energy drain, triggered on damage
- **Concealment system:** Vanish→Ambush combo, 0 aggro, Strategist passive, Escape Plan passive
- **Envenom:** Attacks apply Poison while active
- **Imbue:** Bonus damage on all attacks while active
- **Caltrops + Trap:** Enemies take damage on position change. Trap also stuns (single-use).
- **Mass Confusion:** Enemies redirect attacks to fellow enemies
- **Corpse Explosion / Soul Harvest:** Triggered on character death
- **Elementalist energy bolt:** Changes effect based on last spell element used
- **Chain Lightning:** Bounces to 1-2 other enemies at 50% damage
- **Frozen Tomb:** Target stunned + immune to damage for 2 turns
- **Soul Link:** 30% of damage to linked target splashes to other enemies
- **Drain Soul:** Escalating DoT (3→5→7) that heals caster
- **Priest healing kit:** Prayer of Mending (HoT), Holy Ward (group shield), Purify (cleanse), Resurrect (revive dead), Blessing (+dmg buff), Divine Intervention (full immunity)
- **Resurrect dead-ally targeting:** Dead character cards become selectable, revived characters restore visuals
- **Ranger combat kit:** Snipe (+crit), Barrage (3-hit), Hunter's Focus (+dmg buff), Pin (block movement), Tracking Shot (scales with enemy count)
- **Iron Will:** Blocks all negative status effect application for 2 turns
- **Rally Cry:** All allies gain +1 short action
- **Martyr passive:** Prevents ally death once per battle (heals to 1 HP)
- **Devotion passive:** Healing spells grant bonus shield
- **Faith passive:** +20% healing effectiveness
- **Survivalist/Inner Light passives:** Energy/mana regen at turn start
- **Predator/Steady Aim/Keen Eye passives:** Damage and accuracy bonuses wired into damage pipeline
- **Reinforcement system:** Enemies spawn mid-battle based on triggers (enemy count, round, boss HP%). Waves join next round. UI shows [+N] indicator. SFX on spawn.
- **~46 enemy types** across 3 acts with diverse stat profiles and mechanics
- **2 bosses per act** for encounter randomization
- **Equipment + Loot system:** Procedural item generation (8 stat profiles: Tank/Agile/Mighty/Arcane/Cunning/Resilient/Swift/Balanced), ~15 hand-crafted unique items with flavor text, 1-2 drops per battle, post-battle equip/stash UI, full inventory screen from room choice
- Equipment with weapon types, block chances
- Ability popup shows range, weapon requirements, once-per-battle, frontline-only
- **Shop system:** Buy equipment (procedural, act-scaled), buy consumables (potions/bombs/scrolls), buy books (20% chance), sell equipment (50% value), reroll stock (15g/act), inventory access from shop
- **Consumables in battle:** Items tab shows consumables with quantities ("Health Potion x3"), costs 1 Long action, quantity decremented on use, removed when empty
- **22 consumable types:** Health/Energy/Mana Potions (act-scaled), Antidote (cleanse negative effects), Bandages (heal + remove Bleed), Fire/Ice/Poison Bombs (AoE damage), Smoke Bomb (all allies Hide), Throwing Knives, 6 scrolls (cast class spells), 6 books (teach abilities)
- **Scrolls:** Execute actual class abilities (cloned from AbilityCatalog), any character can use, no resource cost, Range=Any
- **Books:** Consumed from inventory Items tab, per-character Teach buttons, permanently adds ability, duplicate prevention
- **Inventory Items tab:** Equipment/Items mode toggle, shows consumable stacks with quantities, book rows with per-character Teach buttons
- All previous UI features (staging, popups, combat log, etc.)
- Procedural chiptune SFX and looping battle music

### Known Issues
- Enemies with no usable abilities pass by zeroing action points (intentional workaround)
- No visual feedback for status effect application/removal (no floating text, no animations)
- Shield value not shown in character detail popup (only on card HP line)
- Custom ability handlers (CrushArmor, SuckerPunch, etc.) don't call ProcessPostHitEffects — stance triggers/Envenom only apply through ExecuteDamage path
- RangerAbilityHandler has its own ApplyModifiersAndDeal helper that duplicates the ActionExecutor pipeline

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
- BattleManager subscribes to OnDamageDealt/OnCharacterDefeated/OnPositionSwapped for passive/caltrops/trap hooks
- ActionExecutor.SetBattleContext provides static access to player/enemy lists for cross-system features (SoulLink splash, Martyr, TrackingShot, ChainLightning)
- Martyr uses dedicated AbilityTag.Martyr (not AbilityTag.Resurrect) to avoid once-per-battle flag collision
- Recursion guard on defeat processing (corpse explosion chain)
- 1 long + 1 short action points per turn
- Elements: Fire, Earth, Water, Air, Arcane, None
- Combat formulas: weapon-based physical (STR + weapon * multiplier - armor), WIL-scaled spells, DEX hit for weapons, INT hit for spells, armor/magic pen
- HitChanceModifier per ability for imprecise attacks (PowderBomb -0.10, Mass Confusion -0.15)
- Screen system: IScreen interface, ScreenManager owns canvas, GameStateManager drives flow
- BattleScreenUI is a plain class (not MonoBehaviour), built under ScreenManager's canvas
- GameBootstrap has `_useTestBattle` toggle for dev convenience
- Roguelike run: room-by-room choice, partial heal, permadeath, start with 2 recruit to 4, 3 acts x 7 floors
- Reinforcements join next round (not mid-turn) — simpler + gives player a round to react
- Dead enemy cards replaced by reinforcements at same grid position
- EncounterData wraps CharacterData (not BattleCharacter) — grid placement deferred to spawn time
- EnemyDefinitions split by act (Act1/Act2/Act3Enemies) to keep files under 300 lines
- Consumables are separate from equipment: stackable, consumed on use, live in RunData.Consumables
- In battle, consumables create temporary AbilityData instances via ConsumableCatalog.GetBattleAbility() — executed through existing ability pipeline
- Scrolls clone the real class ability from AbilityCatalog, removing resource costs (scroll IS the cost), setting Range=Any
- Books consumed from inventory (not battle), permanently add AbilityData to CharacterData.Abilities
- All consumables cost 1 Long action (future Rogue passive will make it Short)
- Shop economy: equipment priced by slot base + stat points * 3 + weapon damage * 2, act-scaled. Sell at 50%. Reroll = 15g * act
- Consumable pricing: flat BuyPrice per type defined in catalog
- Antidote cleanses all negative statuses (Bleed, Poison, Burn, Chilled, Stun, Silence, Terror, Confusion, Mark, SteamBeamDebuff, Pin)
- Smithy planned as separate future room type (crafting + upgrades + materials), not implemented in Phase F
- CharacterData.IsBoss flag for HP-threshold reinforcement triggers
- Boss pools: 2 per act for randomization
- No rarity tiers — items differ by purpose/specialization (ItemProfile enum), not color-coded tiers
- Hybrid loot: procedural base items (random stats within act ranges) + hand-crafted unique items (IsUnique flag, FlavorText)
- Unique items: ~15 with unusual stat combos, can drop from any battle (15% chance), each drops once per run (RunData.DroppedUniques)
- Inventory cap: 20 items (LootConfig.MaxInventorySize). Excess auto-discarded on Continue
- Trinkets: Trinket1/Trinket2 slots interchangeable. Equip prefers empty slot
- Post-battle: loot cards with per-party-member equip buttons + stash. Unhandled items auto-stash
- Inventory screen accessible from room choice via INVENTORY button. In-battle equipping deferred

## Roadmap

### Phase 1: Complete Battle -- DONE
~~1. Death visuals~~ ~~2. Status effects~~ ~~3. Generic abilities~~ ~~4. Class abilities~~ ~~5. Advanced abilities~~
~~5b. Battle system rework~~ ~~5c. Combat formula rework (weapon damage, WIL scaling, unified hit, penetration)~~

### Phase 2: Roguelike Loop (current)
- **A. Foundation** — Screen system, game state machine, main menu ← **DONE**
- **B. Leveling + Post-Battle** — XP, Fire Emblem growth, ability unlocks, post-battle screen ← **DONE**
- **C. Room Choice + Floor Progression** — Room generation, floor/act advancement ← **DONE**
- **D. Enemy Roster + Scaling + Reinforcements** — 46 enemies, reinforcement mechanic, 2 bosses/act ← **DONE**
- **D2. Full Ability Kits** — 22 new abilities + 6 new passives across all 6 classes ← **DONE**
- **E. Equipment + Loot** — Procedural equipment, loot tables, post-battle equip, inventory screen ← **DONE**
- **F. Shop + Consumables** — Shop (buy/sell/reroll), consumable items (potions/bombs/scrolls/books), battle item usage, book teaching ← **DONE**
- **G. Events + Rest + Recruitment** — Random events, rest sites, party setup, recruitment

### Phase 3: Visual Feedback (DOTween) — deferred
### Phase 4: Final Polish & Save — deferred

### Future Rooms
- **Smithy** — Separate room type (RoomType.Smithy). Upgrade equipment stats, craft items from materials. Material drops from battles (future). Not implemented yet.

### Next Up
Phase 2G: Events + Rest + Recruitment

---

## Scene Setup
- SampleScene with a single empty GameObject named "Game" with the `GameBootstrap` component
- Everything else is created in code at runtime
