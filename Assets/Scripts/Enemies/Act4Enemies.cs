namespace PixelWarriors
{
    public static class Act4Enemies
    {
        // =============================================
        // REGULAR FRONTLINERS
        // =============================================

        public static CharacterData CreateVoidStalker()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Void Stalker",
                Class = CharacterClass.Rogue,
                BaseStats = new CharacterStats(8, 7, 3, 8, 9, 4, 4, 8, 7)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Void Tendrils",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Dagger,
                BaseDamage = 8
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Tendril Lash", "Strikes with writhing void tendrils.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.2f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Phase Rend", "Shifts through reality to strike from behind.", basePower: 0, energyCost: 4,
                range: AbilityRange.Reach, damageMultiplier: 1.5f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Tendril Flurry", "A rapid barrage of tendril strikes.", basePower: 0, energyCost: 3,
                range: AbilityRange.Close, damageMultiplier: 0.5f, hitCount: 3));

            return data;
        }

        public static CharacterData CreateAbyssalKnight()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Abyssal Knight",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(11, 5, 4, 9, 4, 5, 12, 10, 3)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Abyssal Blade",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Sword,
                BaseDamage = 9
            };
            data.Equipment[(int)EquipmentSlot.Offhand] = new EquipmentData
            {
                Name = "Void Shield",
                Slot = EquipmentSlot.Offhand,
                WeaponType = WeaponType.Shield,
                BaseBlockChance = 0.18f,
                StatModifiers = new CharacterStats(0, 0, 0, 0, 0, 0, 4, 0, 0)
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Abyssal Cleave", "A wide swing of the abyssal blade.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.3f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Void Smash", "Channels void energy into a devastating blow.", basePower: 0, energyCost: 4,
                range: AbilityRange.Close, damageMultiplier: 1.7f));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Abyssal Guard", "Braces behind the void shield, bolstering defenses.", basePower: 0, energyCost: 3,
                targetType: TargetType.Self));

            return data;
        }

        public static CharacterData CreateFleshGolem()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Flesh Golem",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(16, 3, 1, 12, 1, 1, 8, 4, 1)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Stitched Fists",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.TwoHanded,
                BaseDamage = 11
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Pound", "Hammers down with massive fists.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.4f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Rend", "Tears into a target with stitched claws.", basePower: 0, energyCost: 4,
                range: AbilityRange.Close, damageMultiplier: 1.8f));

            return data;
        }

        public static CharacterData CreateMindFlayer()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Mind Flayer",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(8, 4, 9, 5, 5, 9, 3, 12, 5)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Psychic Tendrils",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 5
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Mind Crush", "Psychic force that damages and disorients.", basePower: 9, manaCost: 3,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Psychic Blast", "A wave of psychic energy that hits all enemies.", basePower: 5, manaCost: 5,
                element: Element.Arcane, targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Tentacle Grab", "Grabs a nearby target with slimy tentacles.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.0f));

            return data;
        }

        public static CharacterData CreateCrystalFiend()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Crystal Fiend",
                Class = CharacterClass.Elementalist,
                BaseStats = new CharacterStats(9, 3, 7, 6, 3, 7, 6, 16, 3)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Crystal Claws",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Sword,
                BaseDamage = 7
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Crystal Slash", "Rakes with razor-sharp crystalline claws.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.2f));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Prismatic Burst", "Shatters a crystal shard, blasting all enemies.", basePower: 6, manaCost: 5,
                element: Element.Arcane, targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Crystal Shell", "Encases in a protective crystal layer.", basePower: 0, manaCost: 4,
                element: Element.Earth, targetType: TargetType.Self));

            return data;
        }

        // =============================================
        // REGULAR BACKLINERS
        // =============================================

        public static CharacterData CreateVoidWeaver()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Void Weaver",
                Class = CharacterClass.Elementalist,
                BaseStats = new CharacterStats(5, 2, 10, 2, 4, 10, 1, 14, 4)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Void Focus",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 4
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Void Bolt", "A concentrated lance of void energy.", basePower: 11, manaCost: 3,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Unravel", "Tears at the fabric of reality around all enemies.", basePower: 7, manaCost: 6,
                element: Element.Arcane, targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Entropy Wave", "A destabilizing wave of chaotic energy.", basePower: 9, manaCost: 4,
                element: Element.Arcane));

            return data;
        }

        public static CharacterData CreateEyestalk()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Eyestalk",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(5, 2, 8, 1, 6, 10, 1, 12, 6)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Gaze",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 2
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Death Gaze", "Focuses a lethal beam from its central eye.", basePower: 10, manaCost: 4,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Fear Gaze", "A terrifying stare that paralyzes with fear.", basePower: 4, manaCost: 3,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Anti-Magic Ray", "Disrupts magical ability with a focused beam.", basePower: 5, manaCost: 4,
                element: Element.Arcane));

            return data;
        }

        public static CharacterData CreateEntropySeer()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Entropy Seer",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(4, 2, 9, 1, 3, 11, 1, 14, 3)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Entropic Orb",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 3
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Doom Prophecy", "Curses a target with a vision of their demise.", basePower: 12, manaCost: 4,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Entropy Cascade", "Accelerates decay in all enemies.", basePower: 6, manaCost: 6,
                element: Element.Arcane, targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Wither", "Drains life force from a single target.", basePower: 8, manaCost: 3,
                element: Element.Arcane));

            return data;
        }

        public static CharacterData CreateAbyssalChanter()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Abyssal Chanter",
                Class = CharacterClass.Priest,
                BaseStats = new CharacterStats(6, 3, 8, 2, 3, 9, 2, 12, 4)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Chanting Tome",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 3
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Dark Hymn", "A chant that damages all enemies with resonant void energy.", basePower: 5, manaCost: 5,
                element: Element.Arcane, targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Abyssal Mending", "Heals an ally with abyssal energy.", basePower: 10, manaCost: 4,
                element: Element.Arcane, targetType: TargetType.SingleAlly));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Void Ward", "Shields an ally with a barrier of void energy.", basePower: 8, manaCost: 4,
                element: Element.Arcane, targetType: TargetType.SingleAlly));

            return data;
        }

        // =============================================
        // ELITES
        // =============================================

        public static CharacterData CreateBehemoth()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Behemoth",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(18, 6, 2, 14, 3, 3, 10, 6, 2)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Massive Fists",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.TwoHanded,
                BaseDamage = 12
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Titanic Slam", "Slams the ground with earth-shattering force.", basePower: 0,
                targetType: TargetType.AllEnemies, range: AbilityRange.Close,
                damageMultiplier: 1.1f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Crushing Grip", "Grabs and crushes a single target.", basePower: 0, energyCost: 4,
                range: AbilityRange.Close, damageMultiplier: 2.0f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Rampage", "Goes on a wild rampage, hitting randomly.", basePower: 0, energyCost: 3,
                range: AbilityRange.Close, damageMultiplier: 0.7f, hitCount: 3));

            return data;
        }

        public static CharacterData CreateMindLord()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Mind Lord",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(10, 4, 12, 4, 5, 12, 3, 16, 5)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Psionic Crown",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 5
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Dominate", "Overwhelms a target's mind with psychic power.", basePower: 14, manaCost: 4,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Psychic Storm", "A devastating storm of psychic energy hits all enemies.", basePower: 8, manaCost: 6,
                element: Element.Arcane, targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Mind Shield", "Erects a psychic barrier, absorbing damage.", basePower: 12, manaCost: 4,
                element: Element.Arcane, targetType: TargetType.Self));

            return data;
        }

        public static CharacterData CreateVoidDragon()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Void Dragon",
                Class = CharacterClass.Elementalist,
                BaseStats = new CharacterStats(16, 6, 10, 12, 5, 8, 8, 10, 4)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Void Fangs",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.TwoHanded,
                BaseDamage = 11
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Void Bite", "Clamps down with jaws suffused with void energy.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.5f));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Void Breath", "Exhales a cone of annihilating void energy.", basePower: 10, manaCost: 6,
                element: Element.Arcane, targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Tail Sweep", "Sweeps its massive tail across all enemies.", basePower: 0, energyCost: 4,
                targetType: TargetType.AllEnemies, range: AbilityRange.Close,
                damageMultiplier: 1.0f));

            return data;
        }

        // =============================================
        // BOSSES
        // =============================================

        public static CharacterData CreateElderVoid()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Elder Void",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(12, 4, 16, 4, 5, 14, 4, 22, 4)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Void Nexus",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 6
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Annihilation Beam", "A focused beam of pure void that erases matter.", basePower: 16, manaCost: 5,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Reality Collapse", "Tears reality apart, damaging all enemies.", basePower: 10, manaCost: 7,
                element: Element.Arcane, targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Void Rebirth", "Channels void energy to regenerate.", basePower: 0, manaCost: 0,
                element: Element.Arcane, targetType: TargetType.Self));

            return data;
        }

        public static CharacterData CreateAbyssalSovereign()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Abyssal Sovereign",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(20, 8, 10, 16, 5, 8, 10, 12, 3)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Sovereign's Greatsword",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.TwoHanded,
                BaseDamage = 14
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Oblivion Strike", "A single strike that threatens to end existence.", basePower: 0, energyCost: 4,
                range: AbilityRange.Close, damageMultiplier: 2.3f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Sovereign's Wrath", "A sweeping attack that hits all enemies.", basePower: 0,
                targetType: TargetType.AllEnemies, range: AbilityRange.Close,
                damageMultiplier: 1.4f));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Abyssal Inferno", "Unleashes a storm of abyssal fire.", basePower: 14, manaCost: 7,
                element: Element.Fire, targetType: TargetType.AllEnemies));

            return data;
        }
    }
}
