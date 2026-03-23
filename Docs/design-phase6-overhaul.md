# Phase 6 — Systems Overhaul & Final Mechanics

## Overview
Lock down all game systems before the class ability/content pass. These are structural changes that affect everything downstream.

---

## 1. Run Structure

### Before
- 3 acts × 7 floors = 21 floors
- Floor 7 = act boss

### After
- **4 acts × 20 floors = 80 floors**
- Floors 1-19: mix of Battle rooms, Bonus rooms, and Events
- Floor 20: act boss (grants 2 level-ups instead of 1)
- Combat is NOT mandatory every floor — but you can't avoid it either

### Biomes (2 options per act, chosen randomly per run — bifurcation designed later)
1. **Sewers/Catacombs** — underground, rats, undead, fungus
2. **Wilderness/Fortress** — surface, bandits, orcs, military
3. **Dark Citadel** (or Shattered Throne — 2nd option TBD) — corrupted seat of power, dark knights, liches
4. **The Abyss** — beyond reality, demons, eldritch horrors, void creatures

### Boss Fights
- Floor 20 of each act
- 2 boss options per act (randomized)
- Boss grants **2 level-ups** (pick twice)
- No bonus room after boss — straight to next act (or game over on Act 4)

---

## 2. Room Flow (Revised)

### Room Types
1. **Battle Room** — no choice, straight into combat. Inventory screen shows after.
2. **Bonus Room** — player chooses: take the bonus (Shop/Rest/Smithy) OR fight instead.
3. **Event** — narrative event with choices/outcomes. Some events grant a level-up. No battle alternative.

### Per-Floor Flow
```
Floor N → Room type determined by floor generation
  Battle Room:  Combat → Inventory/Continue
  Bonus Room:   Choose [Bonus] or [Battle] → Inventory/Continue
  Event:        Narrative → Outcome → Inventory/Continue
  Boss (floor 20): Boss Combat → Next Act
```

### Room Frequency (per act, 19 normal floors)
- **Battle rooms:** ~10-12 per act (pure combat, no choice)
- **Bonus rooms:** ~4-5 per act
  - Rest: 1-2
  - Shop: 1-2
  - Smithy: 1-2
- **Events:** 3-4 per act (some grant level-ups)
- **Boss:** floor 20 (always)

### Inventory Access
Always available between floors regardless of room type.

---

## 3. Leveling System (Revised)

### Before
- XP-based, all characters gain XP per battle
- Level up when XP threshold reached

### After
- **No XP.** After each combat, player picks 1 of 2 offered characters to level up
- Pseudo-random selection: avoids offering the same character repeatedly
- Boss fights grant **2 level-ups** (two separate picks)
- **Max level: 21**
- Once a character reaches 21, they leave the offering pool
- Fibonacci ability unlocks preserved: levels 1, 1, 2, 3, 5, 8, 13, 21
- Passive unlocks preserved: every 5 levels (5, 10, 15, 20)

### Math
- ~10-12 battle rooms per act × 4 acts = ~40-48 guaranteed combats
- ~4-5 bonus rooms per act where player can choose battle = ~8-12 optional combats
- 4 boss fights × 2 level-ups each = 8 boss level-ups
- ~3-4 events per act that sometimes grant level-ups = ~4-6 event level-ups
- **Total level-ups: ~60-74** (depending on player choices)
- 4 characters, avg ~15-18 each if spread evenly
- Focused play: 1-2 characters reach 21, others reach 12-16
- Last passive at level 20, last ability at level 21

---

## 4. Party Setup (Revised)

### Before
- Pick 2 classes, recruit up to 4 during run

### After
- **Pick 4 classes at run start** (from 6 available)
- No Recruit room type
- Full party from floor 1

---

## 5. Ability Tabs (Revised)

### Before (5 tabs)
Attacks | Skills | Spells | Items | Generic

### After (4 tabs)
1. **Generic** (first tab) — Attack, Pass, Swap, Hide + consumable Items
2. **Attacks** — weapon-based class abilities (blocked by Terror)
3. **Skills** — physical/energy class abilities
4. **Spells** — magical/mana class abilities (blocked by Silence)

### Starting Generic Abilities
Every character starts with: **Attack, Pass, Swap, Hide** (4 total)

### Combatant Manual (Generic Ability Books)
- Anticipate, React, Taunt, and future generic techniques are **no longer free**
- Learned via "Combatant Manual" items (like Books but for generic abilities)
- Found in: shops, loot drops, events
- Each Manual teaches 1 specific technique to 1 character
- Gives players control over party customization beyond class abilities

---

## 6. Passive System (Revised)

### Before
- String matching via `PassiveProcessor.HasPassive(character, "Name")`

### After
- `PassiveEffect` enum (like AbilityTag)
- Each passive AbilityData has a `PassiveEffect` value
- PassiveProcessor checks enum instead of string matching
- Scalable: adding a new passive = add enum value + handler, no string typos

---

## 7. Smithy (New Room Type)

Bonus room option. Crafting, upgrades, and salvage using a material system.

### Crafting Resources
- **Metal Scraps** — from dismantling weapons/armor, common drop
- **Leather Scraps** — from dismantling light armor/accessories, common drop
- **Gems** — Ruby, Sapphire, Diamond, etc. Rarer, found in loot/events
  - Gems can be **sold** for good gold value
  - OR **used in Smithy** to imbue magical properties

### Resource Sources
- **Battle loot:** small chance of material drops alongside equipment
- **Dismantling:** break down equipment into Metal/Leather scraps (not gems)
- **Events:** some events reward materials or gems
- **Shops:** can sell materials and gems (materials = low value, gems = high value)

### Smithy Actions
1. **Upgrade Equipment** — spend Metal/Leather scraps to boost an item's stats
2. **Craft Equipment** — random crafting options offered (like shop but costs resources instead of gold). Options refresh per visit
3. **Dismantle Equipment** — break down gear into Metal/Leather scraps (no gems returned)
4. **Imbue with Gem** — attach a gem to a weapon or armor piece for a fixed magical effect
   - Each gem type has **2 fixed effects**: one for weapons, one for armor
   - E.g., Ruby: +Burn on hit (weapon) / +Fire Resist (armor)
   - E.g., Sapphire: +Mana regen (weapon) / +Magic Resist (armor)
   - E.g., Diamond: +Crit chance (weapon) / +Armor (armor)

### Design Notes
- Smithy craft options are **randomized per visit** (like shop stock)
- Upgrade costs scale with item level/act
- Dismantling is the main resource loop: loot gear → dismantle what you don't need → upgrade/craft what you want
- Gems are the premium resource: sell for gold OR invest for permanent item enchant

---

## 8. Action Point Icons (New)

- **Long Action:** large filled pixel circle
- **Short Action:** smaller filled pixel circle
- Added to icon sprite sheet
- Future: colored variants for class-specific points (e.g., Blue = spells only)

---

## 9. Status Effect Icons

Audit all 39 status effects. Ensure every displayable effect has a proper icon in the sprite sheet. Fill gaps.

---

## Implementation Order

1. **Passive system refactor** (PassiveEffect enum) — foundational, affects everything
2. **Leveling rework** — remove XP, add pick-to-level, max 21, capped offering pool
3. **Run structure** — 4 acts × 20 floors, three room types + boss
4. **Party setup** — pick 4, remove Recruit
5. **Tab restructure** — 4 tabs, Generic first, Items merged in, Attack in Generic
6. **Combatant Manual** — new item type for generic ability teaching
7. **Room flow** — bonus room choice with Fight Again + inventory access
8. **Smithy** — design + implement
9. **Act 4 enemies** — The Abyss enemy roster (~14 types)
10. **Icon audit** — action point icons + fill status effect gaps
11. **Economy rebalance** — adjust for 64-floor run length

Then: **Class ability content pass** (per-class identity, ability design, unlock order)
