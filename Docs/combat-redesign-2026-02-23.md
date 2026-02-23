# Combat Redesign Plan — 2026-02-23

Decisions made during the combat math audit session. This document defines the target formulas, stat roles, progression system, and open items for the combat rework.

---

## 1. Stat Roles (Final)

| Stat | Role | Scaling |
|------|------|---------|
| **Endurance** | Max HP | `20 + END * 5` |
| **Stamina** | Max Energy | `10 + STA * 3` |
| **Intellect** | Max Mana (+ possibly magical crit, TBD) | `8 + INT * 4` |
| **Strength** | Physical damage scaling | Part of physical formula |
| **Dexterity** | Accuracy, evasion, physical crit | Contest roll + crit formula |
| **Willpower** | Magical damage scaling (+ possibly effect chance, TBD) | Part of magical formula |
| **Armor** | Flat physical damage reduction | Subtracted after multiplier |
| **Magic Resist** | % magical damage reduction (raised values) | `damage * (1 - MR/100)` |
| **Initiative** | Turn order | Higher = acts first |

---

## 2. Physical Damage Formula

```
RawDamage = (WeaponDamage + STR * 1.0) * AbilityMultiplier
EffectiveArmor = targetArmor * (1 - armorPenetration%)
FinalDamage = max(1, RawDamage - EffectiveArmor)
```

- **WeaponDamage:** From equipped weapon's `BaseDamage` field
- **STR scaling:** 1.0 per point (clean, easy to reason about)
- **AbilityMultiplier:** Each physical ability defines a multiplier instead of BasePower
  - Generic Attack: 1.0x
  - Crushing Blow: ~1.5x
  - Quick Stab: ~0.8x
  - Cleave: ~0.7x (AoE, lower per target)
  - etc.
- **Armor Penetration:** % that ignores target armor. Can come from weapons, abilities, or both.
  - Example weapon: "Piercing Sword" — BaseDamage 8, ArmorPen 15%
  - Example ability: "Crush Armor" — might have built-in pen or debuff target's armor

### Level 1 → Level 25 Examples (Warrior)

```
Level 1:  Weapon=6, STR=7  → (6 + 7) * 1.0 = 13 raw
Level 25: Weapon=15, STR=18 → (15 + 18) * 1.0 = 33 raw

Crushing Blow (1.5x):
Level 1:  13 * 1.5 = 19.5 → 20 raw
Level 25: 33 * 1.5 = 49.5 → 50 raw

Growth: ~2.5x from level 1 to 25
```

### Level 1 → Level 25 Examples (Rogue)

```
Level 1:  Weapon=4, STR=5  → (4 + 5) * 1.0 = 9 raw
Level 25: Weapon=10, STR=12 → (10 + 12) * 1.0 = 22 raw

Quick Stab (0.8x):
Level 1:  9 * 0.8 = 7.2 → 7 raw
Level 25: 22 * 0.8 = 17.6 → 18 raw
```

---

## 3. Magical Damage Formula

```
RawDamage = AbilityBasePower + WIL * 1.0
EffectiveMR = max(0, targetMR - magicPenetration)
FinalDamage = max(1, RawDamage * (1 - EffectiveMR / 100))
```

- **WIL scaling:** 1.0 per point (mirrors STR for physical)
- **AbilityBasePower:** Spells keep their own BasePower (not weapon-dependent)
- **Magic Penetration:** Flat subtraction from target MR before percentage is applied
  - Example ability: "Meltdown" — has built-in magic pen
  - Example weapon: "Staff of Penetration" — MagicPen +5
- **MR values raised:** Characters/enemies should have MR in the 10-30 range for it to be noticeable

### Level 1 → Level 25 Examples (Elementalist)

```
Level 1:  WIL=8,  Energy Bolt (BP 10) → 10 + 8 = 18 raw
Level 25: WIL=19, Energy Bolt (BP 10) → 10 + 19 = 29 raw

vs target with MR 20: 29 * 0.80 = 23
vs target with MR 30: 29 * 0.70 = 20
vs target with MR 30, MagicPen 10: 29 * 0.80 = 23

Growth: ~1.6x from level 1 to 25
```

### Physical vs Magical Damage Identity

| | Physical | Magical |
|---|---------|---------|
| **Scales with** | STR + Weapon | WIL + Ability BasePower |
| **Defense** | Armor (flat subtraction) | MR (% reduction) |
| **Penetration** | % armor ignore | Flat MR subtraction |
| **Character** | Weapon-dependent, upgradeable | Innate, spell-dependent |

Armor is strong vs many small hits, weak vs one big hit. MR is proportionally constant regardless of hit size. This creates meaningful tactical difference.

---

## 4. Hit Resolution Pipeline

```
1. ACCURACY vs EVASION CONTEST
   hitChance = BaseHit + (attackerDEX - targetDEX) * 0.03
   Roll > hitChance? → MISS

2. BLOCK CHECK (physical only)
   blockChance = equipmentBlockChance + stanceBonus
   Roll < blockChance? → BLOCK (costs energy if in Defensive Stance)

3. DAMAGE CALCULATION
   Physical: (WeaponDmg + STR) * AbilityMult - EffectiveArmor
   Magical:  (AbilityBP + WIL) * (1 - EffectiveMR/100)

4. CRIT CHECK
   Physical crit: 0.03 + DEX * 0.015
   (Magical crit: TBD — possibly INT-based)
   Crit? → damage * 1.5

5. POST-DAMAGE MODIFIERS
   Imbue, Mark, Shield absorption, etc.
```

### Accuracy vs Evasion

Single contest roll: `hitChance = 0.75 + (attackerDEX - targetDEX) * 0.03`

| DEX Advantage | Hit Chance |
|--------------:|---------:|
| +15 | 100% (cap) |
| +10 | 100% (cap) |
| +5 | 90% |
| 0 | 75% |
| -5 | 60% |
| -10 | 45% |
| -15 | 30% |

Flat bonuses from equipment/skills/passives add directly to hit chance or evasion. Example: "Evasive" passive could grant +10% evasion (effectively -10% hit chance for attackers). A weapon could grant +5% accuracy.

### Block (Simplified)

One roll. Block chance = sum of all sources.

| Source | Example |
|--------|---------|
| Shield equipment | Wooden Shield: +10% |
| Defensive Stance | +10% (additive) |
| Block ability | Sets block to 100% for next hit |
| Equipment bonus | Heavy Shield: +15% |

Defensive Stance blocking costs energy per trigger. Stance drops when energy depleted.

### Physical Crit

`critChance = 0.03 + DEX * 0.015`

| DEX | Crit Chance |
|----:|----------:|
| 3 | 7.5% |
| 5 | 10.5% |
| 8 | 15% |
| 12 | 21% |
| 19 | 31.5% |

Rogue end-game critting ~1 in 3 hits. Priest barely critting at ~10%. Feels right.

---

## 5. Progression System

### Run Structure

- **3 Acts**, each in a different biome
- **~20 floors per act** (~15 battles + shops/events/rest)
- **~60 battles total** across a full run
- Characters reach **level 20-25** by end of run

### Skill Unlocks

- **Active abilities:** Learned at Fibonacci levels — 1, 1, 2, 3, 5, 8, 13, 21
- **Passive abilities:** Learned every 5 levels — 5, 10, 15, 20, 25

### Stat Growth (Fire Emblem Style)

Each level-up, each stat independently rolls for +1 growth.

```
growthChance = classBaseRate + personalModifier
```

- **classBaseRate:** Fixed per class (e.g., Warrior STR base = 40%)
- **personalModifier:** Random per run, per character. Small random offset (e.g., -10% to +10%) that makes each run's characters feel slightly different.
- **Growth per proc:** +1 per stat (no stat needs +2/+3 per proc currently, but the system allows it if needed later)

### Growth Rate Ranges

| Rate | Expected Gain (24 levels) | Typical Use |
|-----:|-------------------------:|-------------|
| 50% | +12 | Highest possible (rare primary) |
| 40% | +10 | Primary offensive/defensive stat |
| 30% | +7 | Good secondary |
| 20% | +5 | Weak area |
| 10% | +2 | Dump stat |

### Projected Class Growth Rates

```
                    END  STA  INT  STR  DEX  WIL  ARM  MRS  INI
Warrior             40%  30%  10%  45%  20%  10%  30%  10%  20%
Rogue               20%  40%  15%  30%  45%  15%  15%  15%  35%
Ranger              25%  35%  15%  20%  40%  20%  15%  15%  30%
Priest              35%  15%  35%  10%  15%  40%  25%  30%  15%
Elementalist        15%  10%  40%  10%  20%  45%  10%  30%  25%
Warlock             25%  15%  35%  15%  15%  40%  15%  25%  20%
```

### Projected End-Game Stats (Expected Values)

```
                     END  STA  INT  STR  DEX  WIL  ARM  MRS  INI
Warrior Lv.25        18   12    4   18    9    5   13    4    9
Rogue Lv.25           9   17    7   12   19    7    7    7   15
Ranger Lv.25         11   14    7   9    17   9    7    7   13
Priest Lv.25         15    7   15    5    7   17   11   12    7
Elementalist Lv.25    7    4   19    4    9   19    3   11   11
Warlock Lv.25        11    7   16    7    7   17    6   12    9
```

### Projected End-Game Resources

| Class | END | HP | STA | Energy | INT | Mana |
|-------|----:|---:|----:|-------:|----:|-----:|
| Warrior | 18 | 110 | 12 | 46 | 4 | 24 |
| Rogue | 9 | 65 | 17 | 61 | 7 | 36 |
| Ranger | 11 | 75 | 14 | 52 | 7 | 36 |
| Priest | 15 | 95 | 7 | 31 | 15 | 68 |
| Elementalist | 7 | 55 | 4 | 22 | 19 | 84 |
| Warlock | 11 | 75 | 7 | 31 | 16 | 72 |

### Equipment Scaling

Weapons roughly double-to-triple over a run:

| Tier | When | Weapon BaseDamage | Example |
|------|------|------------------:|---------|
| Starting | Act 1 start | 3-6 | Iron Sword, Sharp Dagger |
| Early | Act 1 mid | 5-8 | Steel Sword, Fine Bow |
| Mid | Act 2 | 8-11 | War Axe, Enchanted Staff |
| Late | Act 3 | 11-15 | Runic Blade, Dragonbone Bow |

Armor/shields scale similarly. Late-game shield might give 15-20% block + ARM 5-8.

---

## 6. Penetration System

### Armor Penetration (Physical)

Percentage-based: ignores a % of target's armor.

```
EffectiveArmor = targetArmor * (1 - armorPen%)
```

Sources:
- **Weapon property:** "Piercing Sword" — ArmorPen 15%
- **Ability property:** "Crush Armor" — built-in 30% pen, or debuffs target armor
- **Passives/buffs:** Could grant flat ArmorPen bonuses

Example: Warrior (STR 18, Weapon 15) vs enemy with ARM 12
- No pen: (15+18)*1.0 - 12 = 21 damage
- 25% pen: (15+18)*1.0 - 9 = 24 damage
- 50% pen: (15+18)*1.0 - 6 = 27 damage

### Magic Penetration (Magical)

Flat subtraction: removes points of MR before % reduction is applied.

```
EffectiveMR = max(0, targetMR - magicPen)
```

Sources:
- **Weapon property:** "Staff of Piercing" — MagicPen 5
- **Ability property:** "Meltdown" — built-in MagicPen 10
- **Passives/buffs:** Could grant flat MagicPen bonuses

Example: Elementalist (WIL 19, Energy Bolt BP 10) vs enemy with MR 25
- No pen: (10+19) * (1 - 25/100) = 29 * 0.75 = 22 damage
- MagicPen 10: (10+19) * (1 - 15/100) = 29 * 0.85 = 25 damage
- MagicPen 20: (10+19) * (1 - 5/100) = 29 * 0.95 = 28 damage

### Asymmetry Summary

| | Physical | Magical |
|---|---------|---------|
| Defense type | Flat subtraction | Percentage reduction |
| Penetration type | % armor ignore | Flat MR subtraction |
| Pen is strong vs | High armor targets | Any target |
| Pen source | Weapons + abilities | Weapons + abilities |

---

## 7. DoT Scaling

DoTs scale with the ability that applied them, not flat constants.

Each DoT-applying ability defines its DoT damage. The ability's power (and thus the caster's stats at time of application) determines the DoT strength.

Implementation: `StatusEffectInstance.Value` stores the per-tick damage set by the ability handler at cast time.

Example:
- Rogue's Envenom at level 1 might apply Poison with value = 3+STR/2
- Same ability at level 25 with higher STR = stronger poison
- A weak enemy's poison attack applies weaker poison than a boss's

This keeps the `StatusEffectInstance` structure unchanged — it already has a `Value` field. The change is just in how that value is calculated when the effect is created.

---

## 8. Ability Conversion Reference

Physical abilities need conversion from BasePower to multiplier. Rough mapping based on current values relative to generic Attack (BP 5):

| Ability | Current BP | Proposed Multiplier | Notes |
|---------|----------:|-------------------:|-------|
| Generic Attack | 5 | 1.0x | Baseline |
| Crushing Blow | 12 | 1.5x | Big hit |
| Quick Stab | 6 | 0.8x | Quick action, lower damage |
| Sucker Punch | 8 | 1.2x | + disable effect |
| Ambush | 16 | 2.0x | Requires concealment |
| Dagger Throw | 7 | 1.0x | Ranged |
| Assassination | 10 | 1.3x | + execute mechanic |
| Powder Bomb | 6 | 0.7x | AoE |
| Cleave | 8 | 0.8x | AoE |
| Powerful Bash | 18 | 2.0x | Requires 2H weapon |
| Bladedance | 8 | 1.0x | + combo scaling |
| Crush Armor | 6 | 0.8x | + armor shred |
| Mark | 5 | 0.7x | + mark debuff |

Magical abilities keep BasePower (WIL scaling is additive, not multiplicative on weapon).

---

## 9. MR Value Rebalancing

Current MR values (1-6) are meaningless. Need to raise to 10-30 range.

### Proposed MR Values

```
                     Current MRS  →  Proposed MRS
Warrior                   2              5
Rogue                     3              8
Ranger                    3              8
Priest                    5             15
Elementalist              4             12
Warlock                   6             18

Ratman                    1              5
Goblin Archer             1              3
Minotaur                  2             10
```

Growth rates bring end-game values to:
- Warrior: ~7 (barely any magic defense)
- Priest: ~22 (solid magic defense)
- Warlock: ~24 (strong magic defense)

At these values, MR 20 reduces magical damage by 20% — noticeable but not oppressive. MagicPen becomes meaningful too.

**Note:** Raising base MR values means the growth rate numbers and stat projections in Section 5 would need to be recalculated. The growth rates stay the same, but starting values change.

---

## 10. Block Rework Detail

### Current (Bugged)
Three-step cascade with double-roll issue. Defensive Stance and base block are separate rolls.

### New Design
Single roll. Block chance = sum of all additive sources.

```
blockChance = shieldBlock + stanceBonus + otherBonuses
```

- Only rolls on physical hits
- Defensive Stance adds to block chance, blocking costs energy
- When energy runs out, stance drops
- Berserker Stance: block chance forced to 0
- Block ability: sets guaranteed block on next physical hit (separate from the roll)

---

## 11. Open / TBD Items

| Item | Status | Notes |
|------|--------|-------|
| INT role beyond mana pool | TBD | Magical crit? Effect duration? Something else? |
| WIL secondary role | TBD | Effect chance for debuffs to land? Or just damage? |
| Magical crit formula | TBD | Could be INT-based or WIL-based. Needs to feel distinct from physical crit (DEX). |
| Personal modifier range | TBD | -5% to +5%? -10% to +10%? Determines run-to-run variance. |
| Exact MR base values | TBD | Rough range 5-18 defined (Section 9), exact numbers during implementation. |
| Enemy stat scaling per act | TBD | How fast enemy stats/HP/armor grow across 3 acts. Determines difficulty curve. |
| Enemy roster per act | TBD | Only 3 enemy types exist (Ratman, Goblin, Minotaur). Need enemies for Acts 2-3. |
| DoT formula per ability | TBD | Each DoT-applying ability needs its damage-per-tick formula (scales with caster stats at cast time). |
| Resource formula rebalancing | TBD | HP/Energy/Mana formulas may need retuning after MR base values change and stat growth is in. Current END/STA/INT multipliers (5/3/4) were designed for level 1 only. |
| Flat bonus system | TBD | Equipment/skills/passives granting flat bonuses (+5% dodge, +X armor pen, etc.). Need a clean way to aggregate these in BattleCharacter. |
| Accuracy vs Evasion tuning | TBD | Base 75% and 3% per DEX point are starting values. May need adjustment after playtesting with full level range (DEX 3-19). |
| Passive implementations | Deferred | Should be designed after stat rework is solid. Each passive needs specific hooks. |
| Ability multiplier tuning | Deferred | Rough values in Section 8. Will need playtesting — multipliers affect ALL weapon tiers. |
| Healing formula | TBD | Currently flat BasePower. Should healing scale with a stat? WIL? INT? Or stay flat? |

---

## 12. Implementation Order (Suggested)

1. **Stat growth system** — Add growth rates to ClassDefinitions, implement level-up with random rolls
2. **Physical damage formula** — Convert to weapon + multiplier system
3. **Magical damage formula** — Add WIL scaling
4. **Hit resolution rework** — Accuracy vs evasion contest, simplified block
5. **Penetration system** — Add ArmorPen and MagicPen to EquipmentData and AbilityData
6. **MR rebalancing** — Raise base values
7. **Crit rework** — Physical crit on DEX with new scaling
8. **DoT scaling** — Update ability handlers to set DoT value from stats
9. **Ability conversion** — Convert all physical abilities from BasePower to multiplier
10. **Passive implementations** — After everything else is stable

---

## Source: Previous Document

Full current-state audit with concrete examples at current values is in:
`Docs/combat-math-audit-2026-02-23.md`
