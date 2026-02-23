# Combat Math Audit — 2026-02-23

Full audit of every stat, formula, and combat roll in the current codebase. Includes concrete examples, stat range analysis, and identified design issues.

**Equipment model (simplified):** Weapons have BaseDamage only (no stat modifiers). Shields have block chance + armor. The damage formula does NOT yet use weapon BaseDamage — that redesign is pending.

---

## Table of Contents

1. [Character Stats Reference](#1-character-stats-reference)
2. [Base Stats by Class](#2-base-stats-by-class)
3. [Starting Equipment](#3-starting-equipment)
4. [Effective Stats](#4-effective-stats)
5. [Enemy Stats](#5-enemy-stats)
6. [Resource Formulas & Ranges](#6-resource-formulas--ranges)
7. [Hit Resolution Pipeline](#7-hit-resolution-pipeline)
8. [Accuracy — Formula, Examples, Range](#8-accuracy)
9. [Dodge — Formula, Examples, Range](#9-dodge)
10. [Block — Formula, Examples, Range](#10-block)
11. [Physical Damage — Formula, Examples, Range](#11-physical-damage)
12. [Magical Damage — Formula, Examples, Range](#12-magical-damage)
13. [Critical Hits — Formula, Examples, Range](#13-critical-hits)
14. [Armor — Range Analysis](#14-armor)
15. [Magic Resist — Range Analysis](#15-magic-resist)
16. [Initiative — Range Analysis](#16-initiative)
17. [Post-Damage Modifiers](#17-post-damage-modifiers)
18. [DoTs & Status Effects](#18-dots--status-effects)
19. [Resource Regeneration](#19-resource-regeneration)
20. [Weapon Damage (Not Yet Used)](#20-weapon-damage-not-yet-used)
21. [Full Range Summary](#21-full-range-summary)
22. [Identified Issues](#22-identified-issues)

---

## 1. Character Stats Reference

| Stat | What It Does | What It Doesn't Do |
|------|-------------|-------------------|
| **Endurance** | Max HP: `20 + END * 5` | Nothing else |
| **Stamina** | Max Energy: `10 + STA * 3` | Nothing else |
| **Intellect** | Max Mana: `8 + INT * 4` | Does NOT increase spell damage |
| **Strength** | Physical damage: `BasePower + STR * 1.5` | Does NOT affect magical damage |
| **Dexterity** | Accuracy + Dodge + Crit (all three) | Nothing else |
| **Willpower** | Formula exists but is **never called** | Completely unused |
| **Armor** | Flat physical damage subtraction | Does NOT reduce magical damage |
| **Magic Resist** | % magical reduction: `dmg * (1 - MR/100)` | Does NOT reduce physical damage |
| **Initiative** | Turn order (higher = earlier) | Nothing else |

---

## 2. Base Stats by Class

```
                     END  STA  INT  STR  DEX  WIL  ARM  MRS  INI
Warrior               8    5    2    7    4    3    6    2    4
Rogue                 4    7    3    5    8    3    3    3    7
Ranger                5    6    3    4    7    4    3    3    6
Priest                7    3    7    3    3    7    5    5    3
Elementalist          3    2    9    2    4    8    1    4    5
Warlock               5    3    8    3    3    7    2    6    4
```

---

## 3. Starting Equipment

Weapons provide BaseDamage only (no stat bonuses). Shield provides block chance + armor.

| Class | Weapon | BaseDamage | Block | Stat Bonus |
|-------|--------|-----------|-------|-----------|
| Warrior | Iron Sword + Wooden Shield | 6 | 15% | ARM+2 (shield only) |
| Rogue | Sharp Dagger | 4 | — | — |
| Ranger | Hunting Bow | 5 | — | — |
| Priest | Oak Staff | 3 | — | — |
| Elementalist | Crystal Staff | 3 | — | — |
| Warlock | Ritual Dagger | 4 | — | — |

---

## 4. Effective Stats

Base stats + equipment. Only Warrior gets any stat bonus (ARM+2 from shield).

```
                     END  STA  INT  STR  DEX  WIL  ARM  MRS  INI
Warrior               8    5    2    7    4    3    8    2    4
Rogue                 4    7    3    5    8    3    3    3    7
Ranger                5    6    3    4    7    4    3    3    6
Priest                7    3    7    3    3    7    5    5    3
Elementalist          3    2    9    2    4    8    1    4    5
Warlock               5    3    8    3    3    7    2    6    4
```

---

## 5. Enemy Stats

```
                     END  STA  INT  STR  DEX  WIL  ARM  MRS  INI
Ratman                5    4    1    5    4    1    3    1    4
Goblin Archer         3    4    2    3    5    2    1    1    6
Minotaur             12    6    2   10    3    3    8    2    2
```

---

## 6. Resource Formulas & Ranges

### Formulas
```
Max HP     = 20 + Endurance * 5     →  +5 HP per point
Max Energy = 10 + Stamina * 3       →  +3 EN per point
Max Mana   =  8 + Intellect * 4     →  +4 MP per point
```

### Player Resources

| Class | END | HP | STA | Energy | INT | Mana |
|-------|----:|---:|----:|-------:|----:|-----:|
| Warrior | 8 | **60** | 5 | 25 | 2 | 16 |
| Rogue | 4 | **40** | 7 | 31 | 3 | 20 |
| Ranger | 5 | **45** | 6 | 28 | 3 | 20 |
| Priest | 7 | **55** | 3 | 19 | 7 | 36 |
| Elementalist | 3 | **35** | 2 | 16 | 9 | 44 |
| Warlock | 5 | **45** | 3 | 19 | 8 | 40 |

### Enemy Resources

| Enemy | HP | Energy | Mana |
|-------|---:|-------:|-----:|
| Ratman | 45 | 22 | 12 |
| Goblin Archer | 35 | 22 | 16 |
| Minotaur | 80 | 28 | 16 |

### HP Range

```
Squishiest → Elementalist (END 3):  35 HP
Tankiest   → Warrior (END 8):      60 HP
Spread:     25 HP difference (Warrior has 71% more HP)
Per point:  +5 HP per Endurance
```

A Ratman Claw deals ~11 to an unarmored target. That's **3 hits to kill Elementalist, 5 hits to kill Warrior**.

### Energy Range

```
Lowest → Elementalist (STA 2):  16 Energy
Highest → Rogue (STA 7):       31 Energy
Spread:   15 Energy difference (Rogue has 94% more)
Per point: +3 Energy per Stamina
```

Most physical abilities cost 2-4 Energy. Rogue gets ~8-15 ability uses from full. Elementalist gets ~4-8.

### Mana Range

```
Lowest → Warrior (INT 2):       16 Mana
Highest → Elementalist (INT 9): 44 Mana
Spread:   28 Mana difference (Elementalist has 175% more)
Per point: +4 Mana per Intellect
```

Most spells cost 3-6 Mana. Elementalist gets ~7-14 spell casts. Warrior gets ~3-5.

---

## 7. Hit Resolution Pipeline

Defined in `ActionExecutor.ResolveHit()`. Each hit follows this sequence:

```
1. ACCURACY CHECK
   → Roll > accuracy? → MISS

2. DODGE CHECK
   → UltimateReflexes active? → auto-DODGE
   → Roll < dodgeChance? → DODGE

3. BLOCK CHECK (physical only, skip if attacker has Berserker Stance)
   → Block status effect? → BLOCK (consumes effect)
   → Berserker Stance on target? → skip
   → Defensive Stance + equipment block? → BLOCK (costs 2 energy)
   → Base equipment block chance? → BLOCK

4. DAMAGE CALCULATION
   → Physical: BasePower + STR * 1.5 - targetArmor (min 1)
   → Magical:  BasePower * (1 - MR/100) (min 1)

5. CRIT CHECK
   → Roll < critChance? → damage * 1.5

6. POST-DAMAGE MODIFIERS
   → +3 flat if attacker has Imbue
   → * 1.10 if target has Mark
   → Shield absorption
```

---

## 8. Accuracy

**Formula:** `0.90 + DEX * 0.01`

| Character | DEX | Accuracy |
|-----------|----:|--------:|
| Warrior | 4 | 94% |
| Rogue | 8 | 98% |
| Ranger | 7 | 97% |
| Priest | 3 | 93% |
| Elementalist | 4 | 94% |
| Warlock | 3 | 93% |
| Ratman | 4 | 94% |
| Goblin Archer | 5 | 95% |
| Minotaur | 3 | 93% |

### Range

```
Lowest DEX in game  (3): 93% accuracy
Highest DEX in game (8): 98% accuracy
Spread:  5 percentage points across 5 DEX points
Per point: +1% accuracy per DEX
```

**Over 100 attacks at 93% vs 98%:** 7 misses vs 2 misses. Functionally everyone almost always hits.

---

## 9. Dodge

**Formula:** `0.03 + DEX * 0.005`

| Character | DEX | Dodge |
|-----------|----:|------:|
| Warrior | 4 | 5.0% |
| Rogue | 8 | 7.0% |
| Ranger | 7 | 6.5% |
| Priest | 3 | 4.5% |
| Elementalist | 4 | 5.0% |
| Warlock | 3 | 4.5% |
| Ratman | 4 | 5.0% |
| Goblin Archer | 5 | 5.5% |
| Minotaur | 3 | 4.5% |

### Range

```
Lowest DEX (3):  4.5% dodge
Highest DEX (8): 7.0% dodge
Spread:  2.5 percentage points across 5 DEX
Per point: +0.5% dodge per DEX
```

**Over 100 attacks received:** Lowest dodges ~4-5, highest dodges ~7. The difference is ~3 dodges per 100 attacks. Nearly irrelevant.

---

## 10. Block

**Formula:** Multi-step pipeline, physical attacks only.

```
1. Block status effect (Block ability) → guaranteed, consumes effect
2. Defensive Stance → roll equipment block chance, costs 2 EN per trigger
3. Base equipment block chance (shield)
```

### Range

```
No shield (all classes except Warrior): 0% block
Warrior with Wooden Shield:             15% block
Warrior in Defensive Stance:            15% (same, but costs 2 EN per trigger)
                                        + ANOTHER 15% base roll = effectively ~28%
Warrior in Berserker Stance:            0% (can't block)
Using Block ability:                    100% (next hit guaranteed blocked)
```

**Over 100 physical hits vs Warrior with shield (no stance):** ~15 blocked. Every other class: 0 blocked.

**Bug:** Defensive Stance checks block chance, then base block chance is checked again. This gives two independent 15% rolls = 27.75% effective block rate. Likely unintended.

---

## 11. Physical Damage

**Formula:** `BasePower + STR * 1.5 - targetArmor` (minimum 1)

### Player vs Enemies

**Generic Attack (BasePower 5):**

| Attacker | STR | vs Ratman (ARM 3) | vs Goblin (ARM 1) | vs Minotaur (ARM 8) |
|----------|----:|------------------:|-------------------:|--------------------:|
| Warrior | 7 | **13** | **15** | **8** |
| Rogue | 5 | **10** | **12** | **5** |
| Ranger | 4 | **8** | **10** | **3** |
| Priest | 3 | **7** | **9** | **2** |
| Elementalist | 2 | **5** | **7** | **1** |
| Warlock | 3 | **7** | **9** | **2** |

**Crushing Blow (BasePower 12, Warrior only):**

| vs Ratman (ARM 3) | vs Goblin (ARM 1) | vs Minotaur (ARM 8) |
|-------------------:|-------------------:|--------------------:|
| **20** | **22** | **15** |

**Quick Stab (BasePower 6, Rogue only):**

| vs Ratman (ARM 3) | vs Goblin (ARM 1) | vs Minotaur (ARM 8) |
|-------------------:|-------------------:|--------------------:|
| **11** | **13** | **6** |

### Enemies vs Players

**Ratman Claw (STR 5, BasePower 6):**

| vs Warrior (ARM 8) | vs Rogue (ARM 3) | vs Ranger (ARM 3) | vs Priest (ARM 5) | vs Elementalist (ARM 1) | vs Warlock (ARM 2) |
|--------------------:|------------------:|-------------------:|-------------------:|------------------------:|-------------------:|
| **6** | **11** | **11** | **9** | **13** | **12** |

**Ratman Bite (STR 5, BasePower 8):**

| vs Warrior (ARM 8) | vs Rogue (ARM 3) | vs Elementalist (ARM 1) |
|--------------------:|------------------:|------------------------:|
| **8** | **13** | **15** |

**Minotaur Gore (STR 10, BasePower 14):**

| vs Warrior (ARM 8) | vs Rogue (ARM 3) | vs Elementalist (ARM 1) |
|--------------------:|------------------:|------------------------:|
| **21** | **26** | **28** |

### STR Range

```
Lowest STR (2, Elementalist) using Attack vs Ratman: 5 damage
Highest STR (7, Warrior) using Attack vs Ratman:     13 damage
Spread:  8 damage across 5 STR points
Per point: +1.5 damage per STR

Lowest STR (2) using Attack vs Minotaur: 1 damage (min)
Highest STR (7) using Attack vs Minotaur: 8 damage
```

**Turns to kill Ratman (45 HP) with generic Attack:**
- Warrior (STR 7): 13 damage → ~4 turns
- Rogue (STR 5): 10 damage → ~5 turns
- Elementalist (STR 2): 5 damage → ~9 turns (nearly double!)

**Turns to kill Minotaur (80 HP) with generic Attack:**
- Warrior (STR 7): 8 damage → ~10 turns
- Elementalist (STR 2): 1 damage → **80 turns** (effectively impossible)

---

## 12. Magical Damage

**Formula:** `BasePower * (1 - MR/100)` (minimum 1)

**There is no scaling stat.** Intellect only affects mana pool. Willpower is defined but never used. Every caster deals the same damage per spell regardless of their stats.

### Examples

**Energy Bolt (BasePower 10):**

| Target | MR | Damage |
|--------|---:|-------:|
| Ratman | 1 | **10** |
| Goblin Archer | 1 | **10** |
| Minotaur | 2 | **10** |
| Warrior | 2 | **10** |
| Rogue | 3 | **10** |
| Priest | 5 | **10** |
| Warlock | 6 | **9** |

**Terror (BasePower 6):**

| Target | MR | Damage |
|--------|---:|-------:|
| Ratman | 1 | **6** |
| Warlock | 6 | **6** |

**Consume (BasePower 10):**

| Target | MR | Damage |
|--------|---:|-------:|
| Ratman | 1 | **10** |
| Warlock | 6 | **9** |

### Comparison: Who deals more damage per action?

| Character | Best single-target ability | Damage vs Ratman |
|-----------|--------------------------|----------------:|
| Warrior | Crushing Blow (BP 12, Physical) | **20** |
| Rogue | Sucker Punch (BP 8, Physical) | **11** |
| Elementalist | Energy Bolt (BP 10, Magical) | **10** |
| Warlock | Consume (BP 10, Magical) | **10** |

Warrior does 2x the damage of casters with the same action cost. And STR makes the gap wider over time — weapons/equipment can boost STR and physical damage, but nothing boosts magical damage.

---

## 13. Critical Hits

**Formula:** `0.05 + DEX * 0.01`, multiplier = **1.5x**

| Character | DEX | Crit |
|-----------|----:|-----:|
| Warrior | 4 | 9% |
| Rogue | 8 | 13% |
| Ranger | 7 | 12% |
| Priest | 3 | 8% |
| Elementalist | 4 | 9% |
| Warlock | 3 | 8% |
| Ratman | 4 | 9% |
| Goblin Archer | 5 | 10% |
| Minotaur | 3 | 8% |

### Range

```
Lowest DEX (3):  8% crit
Highest DEX (8): 13% crit
Spread:  5 percentage points across 5 DEX
Per point: +1% crit per DEX
```

**Over 100 attacks:** Lowest crits ~8 times, highest crits ~13. The difference is ~5 extra crits per 100 attacks.

**Crit damage examples (Warrior Attack vs Ratman, 13 base):**
- Normal: 13 damage
- Crit: 13 * 1.5 = 20 damage (+54% bonus)

**Note:** DEX governs crit for both physical AND magical. A Rogue crits spells at 13% while an Elementalist crits at 9%.

---

## 14. Armor

**Effect:** Flat subtraction from physical damage.

### Range

```
Lowest ARM (1, Elementalist):  -1 physical damage reduction
Highest ARM (8, Warrior):      -8 physical damage reduction
Spread:  7 points of flat reduction across 7 ARM
Per point: -1 damage per ARM
```

### Impact: Ratman Claw (13.5 raw damage) vs each class

| Target | ARM | Damage Taken | % of Raw |
|--------|----:|-----------:|--------:|
| Elementalist | 1 | 13 | 96% |
| Warlock | 2 | 12 | 89% |
| Rogue | 3 | 11 | 81% |
| Ranger | 3 | 11 | 81% |
| Priest | 5 | 9 | 67% |
| Warrior | 8 | 6 | 44% |

Warrior takes less than half the physical damage that Elementalist takes. Armor is the most impactful defensive stat at current values.

### Armor vs multi-hit abilities

Flat reduction per hit means armor is proportionally stronger against multi-hit attacks:
- 1 hit of 14 raw vs 8 ARM = 6 damage
- 2 hits of 7 raw vs 8 ARM = 1 + 1 = 2 damage (armor floor of 1 per hit)

---

## 15. Magic Resist

**Effect:** Percentage reduction of magical damage: `damage * (1 - MR/100)`

### Range

```
Lowest MRS (1, Ratman/Goblin): 1% reduction
Highest MRS (6, Warlock):      6% reduction
Spread:  5 percentage points across 5 MRS
Per point: -1% magical damage per MRS
```

### Impact: Energy Bolt (BasePower 10) vs each class

| Target | MRS | Damage | Reduced By |
|--------|----:|-------:|---------:|
| Ratman | 1 | 10 | 0 |
| Warrior | 2 | 10 | 0 |
| Rogue | 3 | 10 | 0 |
| Ranger | 3 | 10 | 0 |
| Elementalist | 4 | 10 | 0 |
| Priest | 5 | 10 | 1 |
| Warlock | 6 | 9 | 1 |

MRS 1-4 all round to 0 reduction on a 10-power spell. Even at MRS 6, only 1 damage is absorbed. **Magic Resist is effectively non-functional** at current values.

To reduce a 10-damage spell by even 2 damage, you'd need MRS 15+. To halve it, MRS 50.

---

## 16. Initiative

**Effect:** Determines turn order (higher = acts first).

### Range

```
Lowest INI (2, Minotaur): always acts late
Highest INI (7, Rogue):   acts first

Player range: 3-7
Full range:   2-7
```

| Turn Position | Character | INI |
|:-------------|-----------|----:|
| 1st | Rogue | 7 |
| 2nd | Goblin Archer | 6 |
| 3rd | Ranger | 6 |
| 4th | Elementalist | 5 |
| 5th | Warrior | 4 |
| 5th | Ratman | 4 |
| 5th | Warlock | 4 |
| 8th | Priest | 3 |
| 9th | Minotaur | 2 |

Initiative is purely ordinal — it determines sequence but has no other combat effect. Ties are resolved by list order (implementation detail).

---

## 17. Post-Damage Modifiers

Applied in `ApplyDamageModifiers()` after base damage and crit:

### Mark Bonus
- +10% damage if target has Mark effect
- Example: 13 damage → 14 (13 * 1.10 = 14.3, rounds to 14)

### Imbue Bonus
- +3 flat damage if attacker has Imbue status
- Applied BEFORE Mark multiplier
- Example: 13 base + 3 Imbue = 16, then if Marked: 16 * 1.1 = 18

### Shield Absorption
- Shield absorbs damage point-for-point, remaining damage passes through
- Shield persists until fully absorbed or replaced
- Example: 13 damage vs 10 shield → 3 damage through, shield destroyed

---

## 18. DoTs & Status Effects

All DoT values are flat constants with zero stat scaling.

| Effect | Damage/Turn | Duration | Notes |
|--------|----------:|--------:|-------|
| Bleed | 3 per stack | 3 turns | Stackable, physical |
| Poison | 4 | 5 turns | Also reduces healing by 50% |
| Burn | 6 | 3 turns | Highest DoT |
| Caltrops | 5 (one-time) | — | Triggers on position swap |
| Imbue | +3 per hit | 3 turns | Flat bonus to each attack |

### DoT vs HP Pools

| DoT | Total Damage | % of Elementalist HP (35) | % of Warrior HP (60) |
|-----|------------:|-------------------------:|--------------------:|
| 1 Bleed stack | 9 over 3 turns | 26% | 15% |
| 3 Bleed stacks | 27 over 3 turns | 77% | 45% |
| Poison | 20 over 5 turns | 57% | 33% |
| Burn | 18 over 3 turns | 51% | 30% |

### Aggro Modifiers

| Effect | Multiplier | Duration |
|--------|----------:|---------|
| Taunt | 2.0x aggro | 2 turns |
| Hide | 0.25x aggro | Until next turn |
| Conceal | 0.0x aggro (untargetable) | — |

### Other Status Effects

| Effect | Duration | Description |
|--------|---------|-------------|
| Shield | Permanent | Absorbs damage until depleted |
| Mark | 2 turns | +10% damage taken |
| Anticipate | Until next turn | Act first |
| React | Until next turn | Act last |
| Terror | 1 turn | Cannot use Attacks tab |
| Silence | — | Cannot use Spells tab |
| Block (status) | Until triggered | Next physical hit blocked |
| UltimateReflexes | — | Auto-dodge all attacks |

---

## 19. Resource Regeneration

**Per turn:**
- Energy: 20% of max (minimum 1)
- Mana: 5% of max (minimum 1)

| Class | Max EN | EN Regen | Full in | Max MP | MP Regen | Full in |
|-------|-------:|---------:|--------:|-------:|---------:|--------:|
| Warrior | 25 | 5 | 5 turns | 16 | 1 | 16 turns |
| Rogue | 31 | 6 | ~5 turns | 20 | 1 | 20 turns |
| Ranger | 28 | 6 | ~5 turns | 20 | 1 | 20 turns |
| Priest | 19 | 4 | ~5 turns | 36 | 2 | 18 turns |
| Elementalist | 16 | 3 | ~5 turns | 44 | 2 | 22 turns |
| Warlock | 19 | 4 | ~5 turns | 40 | 2 | 20 turns |

**Observation:** Energy regenerates fast (~5 turns to full regardless of class). Mana regenerates extremely slowly (16-22 turns). Mana is intended to be scarce — Warlock has Ritual (HP→Mana), and React helps via delayed turn.

---

## 20. Weapon Damage (Not Yet Used)

Weapons now have a `BaseDamage` field, but the damage formula does NOT use it yet. Currently, `BasePower + STR * 1.5 - Armor` is the entire physical damage formula. Weapon BaseDamage is stored data only.

| Weapon | Type | BaseDamage | Weapon Range |
|--------|------|-----------|-------------|
| Iron Sword | Sword | 6 | Close |
| Sharp Dagger | Dagger | 4 | Close |
| Hunting Bow | Bow | 5 | Reach |
| Oak Staff | Staff | 3 | Reach |
| Crystal Staff | Staff | 3 | Reach |
| Ritual Dagger | Dagger | 4 | Close |
| Wooden Shield | Shield | — | — |

**Future intent:** Damage formula should probably incorporate weapon damage, e.g. `WeaponDamage * Modifier + STR * X` or similar. This would make weapon upgrades meaningful and differentiate "Rusty Dagger" from "Epic Sword."

---

## 21. Full Range Summary

How much does each stat differentiate characters? Using the actual min/max values currently in the game.

| Stat | Low | High | Low Value | High Value | Spread | Impact |
|------|-----|------|-----------|-----------|--------|--------|
| **Endurance** | 3 (Elem) | 8 (Warr) | 35 HP | 60 HP | 25 HP | **High** — doubles survivability |
| **Stamina** | 2 (Elem) | 7 (Rogue) | 16 EN | 31 EN | 15 EN | **Medium** — more ability uses |
| **Intellect** | 2 (Warr) | 9 (Elem) | 16 MP | 44 MP | 28 MP | **Medium** — more spell casts |
| **Strength** | 2 (Elem) | 7 (Warr) | 5 dmg | 13 dmg | 8 dmg | **High** — massive damage gap |
| **Dexterity** | 3 (Priest) | 8 (Rogue) | 93%/4.5%/8% | 98%/7%/13% | 5%/2.5%/5% | **Low** — barely noticeable per roll |
| **Willpower** | 3 (Warr/Rogue) | 8 (Elem) | nothing | nothing | 0 | **None** — stat is dead |
| **Armor** | 1 (Elem) | 8 (Warr) | -1 phys | -8 phys | 7 flat | **High** — halves physical damage |
| **Magic Resist** | 1 (Rat) | 6 (Warlk) | -1% mag | -6% mag | 5% | **Negligible** — rounds to 0 |
| **Initiative** | 3 (Priest) | 7 (Rogue) | acts late | acts first | 4 spots | **Medium** — turn order only |

### Stat Effectiveness Ranking (Current)

1. **Armor** — Each point is -1 damage per physical hit. Highly impactful.
2. **Endurance** — +5 HP per point. Tank vs squishy is life or death.
3. **Strength** — +1.5 physical damage per point. Only scaling damage stat.
4. **Stamina/Intellect** — More resources. Important but indirect.
5. **Initiative** — Turn order matters but effect is ordinal.
6. **Dexterity** — Controls 3 things but each scales too weakly to matter.
7. **Magic Resist** — At current values, does essentially nothing.
8. **Willpower** — Literally does nothing.

---

## 22. Identified Issues

### Issue 1: Weapon BaseDamage Not Used in Formula

`EquipmentData.BaseDamage` exists but the damage formula is `BasePower + STR * 1.5 - Armor`. Weapon damage is never referenced. All weapons produce identical damage regardless of their BaseDamage value. A "Rusty Hammer" and a "Purple Epic Melting Hammer" deal the same damage if they give the same STR bonus.

**Status:** BaseDamage field added, formula change pending.

### Issue 2: Magical Damage Has No Scaling Stat

Physical: `BasePower + STR * 1.5 - ARM`. Magical: `BasePower * (1 - MR/100)`. No stat scales magical damage. Elementalist (INT 9) does the same spell damage as Warrior (INT 2). Intellect only gives more mana casts, not harder-hitting spells.

**Impact:** Casters can never out-damage physical attackers. Warrior Crushing Blow does 20 vs Ratman. Elementalist Energy Bolt does 10. And the gap only grows with better weapons/STR.

### Issue 3: Willpower Is Completely Unused

`StatCalculator.CalculateEffectChance(willpower)` exists but zero code paths call it. Characters with Willpower 7-8 (Priest, Elementalist, Warlock) get nothing from it.

### Issue 4: Magic Resist Is Non-Functional

MRS values are 1-6. Formula is percentage-based: `damage * (1 - MRS/100)`. At MRS 6, a 10-damage spell is reduced to 9. At MRS 1-4, rounding means zero reduction on typical spells. The stat exists but provides no meaningful protection.

### Issue 5: Accuracy Barely Differentiates

93-98% range. Even the worst attacker (DEX 3) hits 93% of the time. The 5-point spread across all characters amounts to ~5 more misses per 100 attacks for the worst vs best.

### Issue 6: Dodge Is Nearly Irrelevant

4.5-7.0% range. The best dodger (Rogue, DEX 8) avoids 7 of 100 attacks. The spread between worst and best is ~2.5 percentage points. The "Evasive" passive claims increased dodge but isn't implemented.

### Issue 7: Dexterity Controls Three Things Poorly

DEX governs accuracy, dodge, and crit — all three for both physical AND magical. But each scales so weakly (+1%/+0.5%/+1% per point) that none feel impactful. Meanwhile, no other stat has any effect on hit outcomes.

### Issue 8: Class Passives Not Implemented

Basic passives (Tough, Evasive, Keen Eye, Faith, Arcane Affinity, Dark Pact) have names and descriptions but zero mechanical effect. They don't modify stats or hook into systems.

### Issue 9: Block Double-Roll Bug

In `CheckBlock()`, Defensive Stance checks equipment block chance, and then base block chance is checked again as a separate step. A Warrior in Defensive Stance with a 15% shield gets two independent 15% rolls per hit (~27.75% effective rate). Likely unintended.

### Issue 10: DoTs Don't Scale

Bleed (3), Poison (4), Burn (6) are flat constants. A level 1 Bleed and a level 20 Bleed deal the same 3 damage/turn. No stat, level, or equipment affects DoT values.

### Issue 11: STR Difference Between Classes Is Extreme for Physical

With generic Attack (BP 5) vs Ratman: Warrior deals 13, Elementalist deals 5. That's a 2.6x difference. Against armored targets it's worse — vs Minotaur: Warrior 8, Elementalist 1. Armor being flat subtraction means low-STR characters bounce off armored enemies.

---

## Source Files

| File | Contains |
|------|---------|
| `Assets/Scripts/Core/StatCalculator.cs` | All stat-to-value formulas |
| `Assets/Scripts/Core/GameplayConfig.cs` | All constants and multipliers |
| `Assets/Scripts/Core/CharacterStats.cs` | Stat data structure (9 stats) |
| `Assets/Scripts/Battle/ActionExecutor.cs` | Hit resolution, damage pipeline |
| `Assets/Scripts/Characters/ClassDefinitions.cs` | Base stats per class |
| `Assets/Scripts/Characters/CharacterData.cs` | Equipment stat stacking, GetTotalStats() |
| `Assets/Scripts/Characters/BattleCharacter.cs` | Runtime combat state, CanUseAbility |
| `Assets/Scripts/Core/GameBootstrap.cs` | Starting equipment (BaseDamage, block) |
| `Assets/Scripts/Enemies/EnemyDefinitions.cs` | Enemy stats and abilities |
| `Assets/Scripts/Battle/TargetSelector.cs` | Aggro weights, targeting |
| `Assets/Scripts/Equipment/EquipmentData.cs` | Equipment model, BaseDamage field |
| `Assets/Scripts/Abilities/AbilityCatalog.cs` | All ability definitions with BasePower |
| `Assets/Scripts/Abilities/AbilityData.cs` | Ability data structure |
