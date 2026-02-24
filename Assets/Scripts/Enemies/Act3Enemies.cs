namespace PixelWarriors
{
    public static class Act3Enemies
    {
        // =============================================
        // REGULAR FRONTLINERS
        // =============================================

        public static CharacterData CreateDarkKnight()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Dark Knight",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(9, 5, 3, 8, 4, 4, 10, 8, 3)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Dark Blade",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Sword,
                BaseDamage = 8
            };
            data.Equipment[(int)EquipmentSlot.Offhand] = new EquipmentData
            {
                Name = "Shadow Shield",
                Slot = EquipmentSlot.Offhand,
                WeaponType = WeaponType.Shield,
                BaseBlockChance = 0.16f,
                StatModifiers = new CharacterStats(0, 0, 0, 0, 0, 0, 3, 0, 0)
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Dark Slash", "A sweeping strike with dark energy.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.3f));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Counter Stance", "Braces for counterattack against physical hits.", basePower: 0, energyCost: 4,
                targetType: TargetType.Self));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Shadow Strike", "A powerful shadow-infused blow.", basePower: 0, energyCost: 3,
                range: AbilityRange.Close, damageMultiplier: 1.6f));

            return data;
        }

        public static CharacterData CreateAbyssalGolem()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Abyssal Golem",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(12, 3, 1, 10, 1, 1, 6, 20, 1)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Obsidian Fists",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.TwoHanded,
                BaseDamage = 10
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Crush", "Smashes down with massive obsidian fists.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.5f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Quake", "Slams the ground, shaking all enemies.", basePower: 0, energyCost: 5,
                targetType: TargetType.AllEnemies, range: AbilityRange.Close,
                damageMultiplier: 0.9f));

            return data;
        }

        public static CharacterData CreatePlagueBringer()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Plague Bringer",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(7, 4, 5, 5, 4, 6, 3, 8, 4)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Plague Flail",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Mace,
                BaseDamage = 6
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Plague Strike", "A diseased strike that spreads infection.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.0f));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Pestilence", "Unleashes a wave of plague upon all enemies.", basePower: 5, manaCost: 5,
                element: Element.Arcane, targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Rot", "Inflicts a concentrated rotting curse.", basePower: 8, manaCost: 3,
                element: Element.Arcane));

            return data;
        }

        public static CharacterData CreateDeathCultist()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Death Cultist",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(6, 3, 8, 3, 4, 7, 2, 6, 5)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Sacrificial Staff",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 4
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Dark Bolt", "Hurls a bolt of dark energy.", basePower: 8, manaCost: 3,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Blood Offering", "Sacrifices life force for a massive dark blast.", basePower: 16, manaCost: 6,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Staff Swing", "A desperate swing with the staff.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 0.6f));

            return data;
        }

        public static CharacterData CreateChainDevil()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Chain Devil",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(8, 6, 2, 8, 6, 3, 5, 5, 5)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Barbed Chains",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Sword,
                BaseDamage = 7
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Chain Lash", "Lashes out with barbed chains.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.1f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Hook", "Hurls a hooked chain to pull in a distant target.", basePower: 0, energyCost: 3,
                range: AbilityRange.Reach, damageMultiplier: 0.8f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Constrict", "Wraps chains tight, crushing the target.", basePower: 0, energyCost: 4,
                range: AbilityRange.Close, damageMultiplier: 1.5f));

            return data;
        }

        // =============================================
        // REGULAR BACKLINERS
        // =============================================

        public static CharacterData CreateShadowAssassin()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Shadow Assassin",
                Class = CharacterClass.Rogue,
                BaseStats = new CharacterStats(4, 6, 2, 7, 9, 3, 2, 5, 9)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Shadow Dagger",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Dagger,
                BaseDamage = 6
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Shadowstrike", "Strikes from the shadows with lethal precision.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.4f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Throat Slit", "A vicious cut aimed at the throat.", basePower: 0, energyCost: 4,
                range: AbilityRange.Close, damageMultiplier: 2.0f));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Fade", "Fades into the shadows, becoming harder to hit.", basePower: 0, energyCost: 3,
                targetType: TargetType.Self));

            return data;
        }

        public static CharacterData CreateLichAcolyte()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Lich Acolyte",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(4, 2, 8, 2, 3, 7, 1, 14, 4)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Dark Tome",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 3
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Shadow Bolt", "Launches a bolt of concentrated shadow.", basePower: 9, manaCost: 3,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Silence", "Seals a target's ability to cast spells.", basePower: 3, manaCost: 5,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Void Touch", "Channels void energy into a target.", basePower: 6, manaCost: 2,
                element: Element.Arcane));

            return data;
        }

        public static CharacterData CreateBloodMage()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Blood Mage",
                Class = CharacterClass.Elementalist,
                BaseStats = new CharacterStats(5, 2, 9, 2, 3, 8, 1, 10, 4)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Blood Staff",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 3
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Blood Bolt", "Fires a bolt of crystallized blood.", basePower: 8, manaCost: 3,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Hemorrhage", "Causes all enemies to bleed profusely.", basePower: 6, manaCost: 6,
                element: Element.Arcane, targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Life Tap", "Sacrifices vitality to restore mana.", basePower: 0, energyCost: 3,
                targetType: TargetType.Self));

            return data;
        }

        public static CharacterData CreateVoidSpeaker()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Void Speaker",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(4, 2, 8, 1, 3, 9, 1, 12, 3)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Void Staff",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 3
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Mind Blast", "Assaults the target's mind with void energy.", basePower: 7, manaCost: 3,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Void Scream", "Emits a psychic scream that damages all enemies.", basePower: 4, manaCost: 5,
                element: Element.Arcane, targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Psychic Rend", "Tears at the target's psyche with devastating force.", basePower: 10, manaCost: 5,
                element: Element.Arcane));

            return data;
        }

        // =============================================
        // ELITES
        // =============================================

        public static CharacterData CreateVampireLord()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Vampire Lord",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(11, 5, 8, 6, 5, 8, 4, 10, 5)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Vampiric Blade",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Sword,
                BaseDamage = 7
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Drain Strike", "A life-draining sword strike.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.3f));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Blood Feast", "Drains blood from a target to restore vitality.", basePower: 8, manaCost: 4,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Dark Embrace", "Wraps in dark energy, healing wounds.", basePower: 12, manaCost: 5,
                element: Element.Arcane, targetType: TargetType.Self));

            return data;
        }

        public static CharacterData CreateTwinWraith()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Twin Wraith",
                Class = CharacterClass.Rogue,
                BaseStats = new CharacterStats(8, 6, 4, 7, 8, 4, 3, 8, 7)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Spectral Blade",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Dagger,
                BaseDamage = 6
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Phase Strike", "Phases through defenses to strike.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.3f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Spectral Barrage", "A flurry of spectral blade strikes.", basePower: 0, energyCost: 3,
                range: AbilityRange.Close, damageMultiplier: 0.6f, hitCount: 3));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Shadow Bond", "Strengthens the bond with its twin.", basePower: 0, energyCost: 4,
                targetType: TargetType.Self));

            return data;
        }

        public static CharacterData CreateDemonChampion()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Demon Champion",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(14, 6, 7, 12, 5, 6, 8, 8, 4)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Demon Greatsword",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.TwoHanded,
                BaseDamage = 10
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Demon Cleave", "A sweeping cleave that hits all enemies.", basePower: 0,
                targetType: TargetType.AllEnemies, range: AbilityRange.Close,
                damageMultiplier: 1.2f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Hellfire Slash", "A devastating slash wreathed in hellfire.", basePower: 0, energyCost: 4,
                range: AbilityRange.Close, damageMultiplier: 1.8f));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Infernal Blast", "Unleashes a wave of infernal fire.", basePower: 10, manaCost: 5,
                element: Element.Fire, targetType: TargetType.AllEnemies));

            return data;
        }

        // =============================================
        // BOSSES
        // =============================================

        public static CharacterData CreateLich()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Lich",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(8, 3, 14, 3, 4, 12, 3, 20, 4)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Skull Staff",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 5
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Death Bolt", "A massive bolt of death energy.", basePower: 14, manaCost: 4,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Soul Drain", "Drains the souls of all enemies.", basePower: 8, manaCost: 6,
                element: Element.Arcane, targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Dark Pact", "Channels dark energy to prepare reinforcements.", basePower: 0, manaCost: 0,
                element: Element.Arcane, targetType: TargetType.Self));

            return data;
        }

        public static CharacterData CreateArchDemon()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Arch Demon",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(18, 8, 8, 14, 5, 7, 8, 10, 3)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Doom Blade",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.TwoHanded,
                BaseDamage = 12
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Cataclysm", "A catastrophic strike that hits all enemies.", basePower: 0,
                targetType: TargetType.AllEnemies, range: AbilityRange.Close,
                damageMultiplier: 1.3f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Doom Strike", "A single devastating blow of pure destruction.", basePower: 0, energyCost: 4,
                range: AbilityRange.Close, damageMultiplier: 2.2f));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Hellfire", "Engulfs all enemies in hellfire.", basePower: 12, manaCost: 6,
                element: Element.Fire, targetType: TargetType.AllEnemies));

            return data;
        }
    }
}
