using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelWarriors
{
    public static class EventCatalog
    {
        private static List<EventData> _all;
        private static Dictionary<string, EventData> _lookup;
        private static EventData _restEvent;

        private static readonly string[] EchoHints =
        {
            "\"The strong one will fall first.\"",
            "\"Gold is heavier than it looks.\"",
            "\"The fog hides more than it shows.\"",
            "\"One of you will not make it out.\"",
            "\"The next door opens inward.\"",
            "\"Trust the silence, not the noise.\"",
            "\"What you seek is already behind you.\""
        };

        public static List<EventData> GetAll()
        {
            if (_all == null) BuildCatalog();
            return _all;
        }

        public static EventData Get(string id)
        {
            if (_lookup == null) BuildCatalog();
            return _lookup.TryGetValue(id, out EventData data) ? data : null;
        }

        public static EventData GetRestEvent()
        {
            if (_restEvent == null) BuildRestEvent();
            return _restEvent;
        }

        public static EventData RollEvent(RunData runData)
        {
            if (_all == null) BuildCatalog();

            List<EventData> eligible = _all.Where(e =>
                e.MinAct <= runData.CurrentAct &&
                (!e.IsUnique || !runData.SeenEvents.Contains(e.Id))
            ).ToList();

            if (eligible.Count == 0)
            {
                // Fallback to a guaranteed non-unique event
                return _lookup["wandering_healer"];
            }

            return eligible[Random.Range(0, eligible.Count)];
        }

        private static void BuildRestEvent()
        {
            _restEvent = new EventData
            {
                Id = "rest_site", MinAct = 0, IsUnique = false,
                Title = "REST SITE",
                Narrative = "A safe clearing. The embers of an old fire still glow.\nYou could rest, study your surroundings, or search for supplies.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "REST BY THE FIRE",
                        Description = "Heal 50% missing HP, restore EN and MP",
                        OutcomeDescription = _ => "The party rests. Wounds close, energy returns.",
                        Apply = (run, _) => EventOutcomes.HealAndRestoreAll(run, RunConfig.RestHealPercent)
                    },
                    new EventChoice
                    {
                        Label = "STUDY THE AREA",
                        Description = "+20 XP to all",
                        OutcomeDescription = _ => "You study your surroundings. All gain 20 XP.",
                        Apply = (run, _) => EventOutcomes.GiveXPAll(run, 20)
                    },
                    new EventChoice
                    {
                        Label = "SEARCH FOR SUPPLIES",
                        Description = "60% chance: find a consumable",
                        OutcomeDescription = run =>
                        {
                            // outcome resolved in Apply, description reads cached result
                            return _lastSearchFound
                                ? "You find useful supplies!"
                                : "You search thoroughly but find nothing.";
                        },
                        Apply = (run, _) =>
                        {
                            _lastSearchFound = Random.value < 0.60f;
                            if (_lastSearchFound)
                                EventOutcomes.GiveRandomConsumable(run);
                        }
                    }
                }
            };
        }

        // Temp flag for search outcome display — resolved in Apply, read in OutcomeDescription
        private static bool _lastSearchFound;
        private static string _lastGambleResult;
        private static string _lastDoppelResult;

        private static void BuildCatalog()
        {
            _all = new List<EventData>();

            // ===== PURE POSITIVE =====

            // 1. Wandering Healer
            _all.Add(new EventData
            {
                Id = "wandering_healer", MinAct = 0, IsUnique = false,
                Title = "THE WANDERING HEALER",
                Narrative = "An old woman sits by the road with a basket of herbs.\nShe tends your wounds without a word.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "ACCEPT HER KINDNESS",
                        Description = "Heal all party 25% missing HP",
                        OutcomeDescription = _ => "The party's wounds close. You feel stronger.",
                        Apply = (run, _) => EventOutcomes.HealPartyPercent(run, 0.25f)
                    }
                }
            });

            // 2. Lucky Coin
            _all.Add(new EventData
            {
                Id = "lucky_coin", MinAct = 0, IsUnique = false,
                Title = "A LUCKY COIN",
                Narrative = "You find a pouch wedged beneath a stone.\nIt clinks with gold.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "POCKET THE GOLD",
                        Description = "Gain gold",
                        OutcomeDescription = run =>
                        {
                            int amount = 20 + 10 * run.CurrentAct;
                            return $"You found {amount} gold!";
                        },
                        Apply = (run, _) => EventOutcomes.GiveGold(run, 20 + 10 * run.CurrentAct)
                    }
                }
            });

            // 3. Shrine of Vigor
            _all.Add(new EventData
            {
                Id = "shrine_of_vigor", MinAct = 0, IsUnique = false,
                Title = "SHRINE OF VIGOR",
                Narrative = "A moss-covered shrine hums with faint energy.\nTouching it fills you with warmth.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "TOUCH THE SHRINE",
                        Description = "Fully heal one character",
                        NeedsCharacterPick = true,
                        CharacterPickPrompt = "Who touches the shrine?",
                        OutcomeDescription = run => "Fully healed!",
                        Apply = (run, idx) => EventOutcomes.HealCharacterFull(run, idx)
                    }
                }
            });

            // ===== PURE NEGATIVE =====

            // 4. Ambush
            _all.Add(new EventData
            {
                Id = "ambush", MinAct = 0, IsUnique = false,
                Title = "AMBUSH",
                Narrative = "You walk into a trap. Arrows fly before you can react.\nThe attackers vanish into the shadows.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "ENDURE",
                        Description = "All take 15% max HP damage",
                        OutcomeDescription = _ => "Everyone takes damage from the ambush.",
                        Apply = (run, _) => EventOutcomes.DamagePartyPercent(run, 0.15f, 5)
                    }
                }
            });

            // 5. Cursed Fog
            _all.Add(new EventData
            {
                Id = "cursed_fog", MinAct = 2, IsUnique = false,
                Title = "THE CURSED FOG",
                Narrative = "A thick fog rolls in from nowhere.\nIt clings to your skin and clouds your mind.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "PUSH THROUGH",
                        Description = "-5 Initiative to all (permanent)",
                        OutcomeDescription = _ => "The fog saps your reflexes. -5 INI to all.",
                        Apply = (run, _) =>
                            EventOutcomes.GiveStatBoostAll(run, new CharacterStats(0,0,0,0,0,0,0,0,-5))
                    }
                }
            });

            // 6. Tax Collector
            _all.Add(new EventData
            {
                Id = "tax_collector", MinAct = 0, IsUnique = false,
                Title = "THE TAX COLLECTOR",
                Narrative = "A kingdom official demands a toll.\nHe has armed guards. You're in no shape to argue.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "PAY THE TOLL",
                        Description = "Lose 30% of current gold",
                        OutcomeDescription = run =>
                        {
                            int loss = Mathf.Max(10, Mathf.RoundToInt(run.Gold * 0.30f));
                            if (run.Gold < 10) loss = run.Gold;
                            return $"You pay {loss} gold.";
                        },
                        Apply = (run, _) =>
                        {
                            int loss = Mathf.Max(10, Mathf.RoundToInt(run.Gold * 0.30f));
                            if (run.Gold < 10) loss = run.Gold;
                            EventOutcomes.TakeGold(run, loss);
                        }
                    }
                }
            });

            // ===== BINARY CHOICE =====

            // 7. Ancient Altar
            _all.Add(new EventData
            {
                Id = "ancient_altar", MinAct = 0, IsUnique = false,
                Title = "ANCIENT ALTAR",
                Narrative = "A cracked stone altar bears a bowl of dark liquid.\nThe inscription reads: 'Power at a price.'",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "DRINK",
                        Description = "+2 random stat, lose 20% max HP",
                        NeedsCharacterPick = true,
                        CharacterPickPrompt = "Who drinks from the altar?",
                        OutcomeDescription = run => _lastAltarStat != null
                            ? $"+2 {_lastAltarStat}, but lost HP from the ordeal."
                            : "The liquid burns but empowers.",
                        Apply = (run, idx) =>
                        {
                            _lastAltarStat = EventOutcomes.GiveRandomStat(run, idx, 2);
                            int maxHP = StatCalculator.CalculateMaxHP(run.Party[idx].GetTotalStats());
                            int dmg = Mathf.RoundToInt(maxHP * 0.20f);
                            EventOutcomes.DamageCharacter(run, idx, dmg);
                        }
                    },
                    new EventChoice
                    {
                        Label = "LEAVE IT",
                        Description = "No effect",
                        OutcomeDescription = _ => "You leave the altar undisturbed.",
                        Apply = (run, _) => { }
                    }
                }
            });

            // 8. Gambler's Chest
            _all.Add(new EventData
            {
                Id = "gamblers_chest", MinAct = 0, IsUnique = false,
                Title = "THE GAMBLER'S CHEST",
                Narrative = "A locked chest sits in the road. A note reads:\n'Open me. Or don't. But you'll always wonder.'",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "OPEN THE CHEST",
                        Description = "60%: random equipment. 40%: trap, 10 dmg to all",
                        OutcomeDescription = _ => _lastGambleResult,
                        Apply = (run, _) =>
                        {
                            if (Random.value < 0.60f)
                            {
                                EventOutcomes.GiveRandomEquipment(run);
                                _lastGambleResult = "Inside you find a useful piece of equipment!";
                            }
                            else
                            {
                                EventOutcomes.DamagePartyFlat(run, 10);
                                _lastGambleResult = "A trap fires! Everyone takes 10 damage.";
                            }
                        }
                    },
                    new EventChoice
                    {
                        Label = "LEAVE IT",
                        Description = "No effect",
                        OutcomeDescription = _ => "Probably for the best.",
                        Apply = (run, _) => { }
                    }
                }
            });

            // 9. Mysterious Merchant (Unique)
            _all.Add(new EventData
            {
                Id = "mysterious_merchant", MinAct = 0, IsUnique = true,
                Title = "MYSTERIOUS MERCHANT",
                Narrative = "A hooded figure extends a vial of shimmering liquid.\n'Not for sale,' they whisper. 'A gift. Use it wisely.'",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "ACCEPT THE GIFT",
                        Description = "+3 Willpower, +3 Intellect to one",
                        NeedsCharacterPick = true,
                        CharacterPickPrompt = "Who receives the gift?",
                        OutcomeDescription = _ => "+3 Willpower, +3 Intellect. A generous gift.",
                        Apply = (run, idx) =>
                            EventOutcomes.GiveStatBoost(run, idx, new CharacterStats(0,0,3,0,0,3,0,0,0))
                    },
                    new EventChoice
                    {
                        Label = "REFUSE",
                        Description = "No effect",
                        OutcomeDescription = _ => "The figure vanishes. You feel a faint regret.",
                        Apply = (run, _) => { }
                    }
                }
            });

            // 10. Trapped Survivor
            _all.Add(new EventData
            {
                Id = "trapped_survivor", MinAct = 0, IsUnique = false,
                Title = "THE TRAPPED SURVIVOR",
                Narrative = "A man is pinned under rubble. He gasps:\n'Help me... I have gold. Please.'",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "FREE HIM",
                        Description = "All lose 10% HP, gain gold",
                        OutcomeDescription = run =>
                        {
                            int gold = 25 + 15 * run.CurrentAct;
                            return $"The effort costs you, but he pays {gold} gold.";
                        },
                        Apply = (run, _) =>
                        {
                            EventOutcomes.DamagePartyPercent(run, 0.10f, 3);
                            EventOutcomes.GiveGold(run, 25 + 15 * run.CurrentAct);
                        }
                    },
                    new EventChoice
                    {
                        Label = "LEAVE HIM",
                        Description = "No effect",
                        OutcomeDescription = _ => "His cries fade behind you.",
                        Apply = (run, _) => { }
                    }
                }
            });

            // ===== 3-OPTION CHOICE =====

            // 11. Forked Path
            _all.Add(new EventData
            {
                Id = "forked_path", MinAct = 0, IsUnique = false,
                Title = "THE FORKED PATH",
                Narrative = "Three roads diverge. A crest on a stone suggests\neach leads to something different.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "LEFT — SOUNDS OF WATER",
                        Description = "Restore all EN and MP",
                        OutcomeDescription = _ => "A clear spring. Energy and Mana restored.",
                        Apply = (run, _) => EventOutcomes.RestoreResources(run)
                    },
                    new EventChoice
                    {
                        Label = "RIGHT — SMELL OF SMOKE",
                        Description = "Gain a random consumable",
                        OutcomeDescription = _ => "An abandoned camp. You find supplies.",
                        Apply = (run, _) => EventOutcomes.GiveRandomConsumable(run)
                    },
                    new EventChoice
                    {
                        Label = "MIDDLE — SILENCE",
                        Description = "+5 XP to all",
                        OutcomeDescription = _ => "The quiet road teaches patience. All gain 5 XP.",
                        Apply = (run, _) => EventOutcomes.GiveXPAll(run, 5)
                    }
                }
            });

            // 12. Old Campfire
            _all.Add(new EventData
            {
                Id = "old_campfire", MinAct = 0, IsUnique = false,
                Title = "OLD CAMPFIRE",
                Narrative = "The embers are still warm. Scraps of old maps\nsurround the site. You can rest or investigate.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "REST AROUND THE FIRE",
                        Description = "Heal all 10% missing HP",
                        OutcomeDescription = _ => "A brief rest. HP recovered.",
                        Apply = (run, _) => EventOutcomes.HealPartyPercent(run, 0.10f)
                    },
                    new EventChoice
                    {
                        Label = "STUDY THE MAPS",
                        Description = "+15 XP to all",
                        OutcomeDescription = _ => "The maps reveal patterns. All gain 15 XP.",
                        Apply = (run, _) => EventOutcomes.GiveXPAll(run, 15)
                    },
                    new EventChoice
                    {
                        Label = "SEARCH THE PACKS",
                        Description = "50% chance: find a consumable",
                        OutcomeDescription = _ => _lastSearchFound
                            ? "You find something useful."
                            : "The packs are empty.",
                        Apply = (run, _) =>
                        {
                            _lastSearchFound = Random.value < 0.50f;
                            if (_lastSearchFound)
                                EventOutcomes.GiveRandomConsumable(run);
                        }
                    }
                }
            });

            // ===== CLASS-CONDITIONAL =====

            // 13. Wounded Knight
            _all.Add(new EventData
            {
                Id = "wounded_knight", MinAct = 0, IsUnique = false,
                Title = "THE WOUNDED KNIGHT",
                Narrative = "A knight in broken armour slumps against the wall.\nBreath rattles in his chest. He won't last long.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "HELP HIM",
                        Description = "+5 XP to all",
                        OutcomeDescription = _ => "The knight nods weakly. All gain 5 XP.",
                        Apply = (run, _) => EventOutcomes.GiveXPAll(run, 5)
                    },
                    new EventChoice
                    {
                        Label = "TAKE HIS SWORD",
                        Description = "Gain a random equipment piece",
                        OutcomeDescription = _ => "You take the knight's weapon.",
                        Apply = (run, _) => EventOutcomes.GiveRandomEquipment(run)
                    },
                    new EventChoice
                    {
                        Label = "HEAL HIM",
                        Description = "+2 Endurance to Priest",
                        ConditionLabel = "Requires Priest",
                        Condition = run => EventOutcomes.FindClassIndex(run, CharacterClass.Priest) >= 0,
                        OutcomeDescription = _ => "Your Priest channels holy power. +2 END to Priest.",
                        Apply = (run, _) =>
                        {
                            int idx = EventOutcomes.FindClassIndex(run, CharacterClass.Priest);
                            if (idx >= 0)
                                EventOutcomes.GiveStatBoost(run, idx, new CharacterStats(2,0,0,0,0,0,0,0,0));
                        }
                    }
                }
            });

            // 14. Ritual Circle
            _all.Add(new EventData
            {
                Id = "ritual_circle", MinAct = 2, IsUnique = false,
                Title = "RITUAL CIRCLE",
                Narrative = "A dark circle of chalk glows faintly on the dungeon floor.\nPower pulses within it.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "STEP AROUND IT",
                        Description = "No effect",
                        OutcomeDescription = _ => "You leave the circle untouched.",
                        Apply = (run, _) => { }
                    },
                    new EventChoice
                    {
                        Label = "ABSORB THE ENERGY",
                        Description = "All: -8 HP, +5 Mana",
                        OutcomeDescription = _ => "Dark energy courses through you. -8 HP, +5 MP to all.",
                        Apply = (run, _) =>
                        {
                            EventOutcomes.DamagePartyFlat(run, 8);
                            foreach (CharacterData c in run.Party)
                            {
                                int maxMP = StatCalculator.CalculateMaxMana(c.GetTotalStats());
                                c.CurrentMana = Mathf.Min(c.CurrentMana + 5, maxMP);
                            }
                        }
                    },
                    new EventChoice
                    {
                        Label = "CHANNEL THE RITUAL",
                        Description = "+2 Willpower to one character",
                        ConditionLabel = "Requires Warlock",
                        Condition = run => EventOutcomes.FindClassIndex(run, CharacterClass.Warlock) >= 0,
                        NeedsCharacterPick = true,
                        CharacterPickPrompt = "Who receives the channeled power?",
                        OutcomeDescription = _ => "The Warlock channels the circle. +2 Willpower.",
                        Apply = (run, idx) =>
                            EventOutcomes.GiveStatBoost(run, idx, new CharacterStats(0,0,0,0,0,2,0,0,0))
                    }
                }
            });

            // 15. Shadow Cache
            _all.Add(new EventData
            {
                Id = "shadow_cache", MinAct = 0, IsUnique = false,
                Title = "SHADOW CACHE",
                Narrative = "A hidden panel reveals a thief's stash.\nThe lock looks complex but not impossible.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "FORCE IT OPEN",
                        Description = "50%: consumable. 50%: 8 dmg to one",
                        OutcomeDescription = _ => _lastSearchFound
                            ? "You smash it open and find supplies!"
                            : "The mechanism explodes! Someone takes damage.",
                        Apply = (run, _) =>
                        {
                            _lastSearchFound = Random.value < 0.50f;
                            if (_lastSearchFound)
                            {
                                EventOutcomes.GiveRandomConsumable(run);
                            }
                            else
                            {
                                int victim = EventOutcomes.RandomPartyIndex(run);
                                EventOutcomes.DamageCharacter(run, victim, 8);
                            }
                        }
                    },
                    new EventChoice
                    {
                        Label = "PICK THE LOCK",
                        Description = "Consumable + 10 gold (guaranteed)",
                        ConditionLabel = "Requires Rogue",
                        Condition = run => EventOutcomes.FindClassIndex(run, CharacterClass.Rogue) >= 0,
                        OutcomeDescription = _ => "Picked clean. Found supplies and 10 gold.",
                        Apply = (run, _) =>
                        {
                            EventOutcomes.GiveRandomConsumable(run);
                            EventOutcomes.GiveGold(run, 10);
                        }
                    }
                }
            });

            // 16. The Philosopher's Tome (Unique)
            _all.Add(new EventData
            {
                Id = "philosophers_tome", MinAct = 2, IsUnique = true,
                Title = "THE PHILOSOPHER'S TOME",
                Narrative = "A massive leather-bound tome rests on a pedestal.\nPages flutter despite no wind.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "READ IT",
                        Description = "+2 Intellect to one character",
                        NeedsCharacterPick = true,
                        CharacterPickPrompt = "Who reads the tome?",
                        OutcomeDescription = _ => "Knowledge flows. +2 Intellect.",
                        Apply = (run, idx) =>
                            EventOutcomes.GiveStatBoost(run, idx, new CharacterStats(0,0,2,0,0,0,0,0,0))
                    },
                    new EventChoice
                    {
                        Label = "DECODE IT DEEPLY",
                        Description = "+3 WIL, +2 INT to Elementalist",
                        ConditionLabel = "Requires Elementalist",
                        Condition = run => EventOutcomes.FindClassIndex(run, CharacterClass.Elementalist) >= 0,
                        OutcomeDescription = _ => "The Elementalist deciphers the arcane text. +3 WIL, +2 INT.",
                        Apply = (run, _) =>
                        {
                            int idx = EventOutcomes.FindClassIndex(run, CharacterClass.Elementalist);
                            if (idx >= 0)
                                EventOutcomes.GiveStatBoost(run, idx, new CharacterStats(0,0,2,0,0,3,0,0,0));
                        }
                    },
                    new EventChoice
                    {
                        Label = "LEAVE IT",
                        Description = "No effect",
                        OutcomeDescription = _ => "Some knowledge is best left alone.",
                        Apply = (run, _) => { }
                    }
                }
            });

            // ===== WEIRD/UNUSUAL =====

            // 17. The Echo
            _all.Add(new EventData
            {
                Id = "the_echo", MinAct = 0, IsUnique = false,
                Title = "THE ECHO",
                Narrative = "You hear your own voices ahead — but the words are wrong.\nWarnings. Things none of you have said yet.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "LISTEN CAREFULLY",
                        Description = "???",
                        OutcomeDescription = _ => EchoHints[Random.Range(0, EchoHints.Length)],
                        Apply = (run, _) => { }
                    }
                }
            });

            // 18. Doppelganger (Unique)
            _all.Add(new EventData
            {
                Id = "doppelganger", MinAct = 2, IsUnique = true,
                Title = "THE DOPPELGANGER",
                Narrative = "One of your party stands ten metres ahead — but they're\nalso right beside you. Both look real. Both look afraid.",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Label = "ATTACK THE ONE AHEAD",
                        Description = "50%: ally loses 20% HP. 50%: ally gains +2 stat",
                        OutcomeDescription = _ => _lastDoppelResult,
                        Apply = (run, _) =>
                        {
                            int victim = EventOutcomes.RandomPartyIndex(run);
                            if (victim < 0) { _lastDoppelResult = "Nothing happens."; return; }
                            string name = run.Party[victim].Name;

                            if (Random.value < 0.50f)
                            {
                                int maxHP = StatCalculator.CalculateMaxHP(run.Party[victim].GetTotalStats());
                                int dmg = Mathf.RoundToInt(maxHP * 0.20f);
                                EventOutcomes.DamageCharacter(run, victim, dmg);
                                _lastDoppelResult = $"You attacked the real {name}! They lose {dmg} HP.";
                            }
                            else
                            {
                                string stat = EventOutcomes.GiveRandomStat(run, victim, 2);
                                _lastDoppelResult = $"The impostor shatters! {name} gains +2 {stat}.";
                            }
                        }
                    },
                    new EventChoice
                    {
                        Label = "DO NOTHING",
                        Description = "-1 random stat to one character",
                        NeedsCharacterPick = true,
                        CharacterPickPrompt = "Who was most shaken by the encounter?",
                        OutcomeDescription = _ => _lastDoppelResult,
                        Apply = (run, idx) =>
                        {
                            if (idx < 0 || idx >= run.Party.Count) { _lastDoppelResult = ""; return; }
                            string stat = EventOutcomes.TakeRandomStat(run, idx, 1);
                            _lastDoppelResult = $"{run.Party[idx].Name} is deeply unsettled. -1 {stat}.";
                        }
                    }
                }
            });

            // Build lookup
            _lookup = new Dictionary<string, EventData>();
            foreach (EventData e in _all)
                _lookup[e.Id] = e;
        }

        private static string _lastAltarStat;
    }
}
