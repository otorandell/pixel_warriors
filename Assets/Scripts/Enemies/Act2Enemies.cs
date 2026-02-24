namespace PixelWarriors
{
    public static class Act2Enemies
    {
        // =============================================
        // REGULAR FRONTLINERS
        // =============================================

        public static CharacterData CreateSpider()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Spider",
                Class = CharacterClass.Rogue,
                BaseStats = new CharacterStats(3, 5, 1, 4, 7, 2, 1, 5, 8)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Fangs",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Dagger,
                BaseDamage = 4
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Poison Bite", "Sinks venomous fangs into the target.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 0.9f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Web Spit", "Spits a glob of sticky webbing.", basePower: 0, energyCost: 3,
                range: AbilityRange.Reach, damageMultiplier: 0.5f));

            return data;
        }

        public static CharacterData CreateBandit()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Bandit",
                Class = CharacterClass.Rogue,
                BaseStats = new CharacterStats(5, 5, 2, 5, 6, 2, 3, 5, 6)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Bandit Dagger",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Dagger,
                BaseDamage = 5
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Stab", "A quick dagger thrust.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.0f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Cheap Shot", "A dirty blow to a vulnerable spot.", basePower: 0, energyCost: 2,
                range: AbilityRange.Close, damageMultiplier: 1.4f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Knife Throw", "Hurls a knife at a distant target.", basePower: 0, energyCost: 2,
                range: AbilityRange.Reach, damageMultiplier: 0.8f));

            return data;
        }

        public static CharacterData CreateOrcWarrior()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Orc Warrior",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(8, 5, 1, 7, 3, 2, 6, 4, 3)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Orc Blade",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Sword,
                BaseDamage = 7
            };
            data.Equipment[(int)EquipmentSlot.Offhand] = new EquipmentData
            {
                Name = "Orc Shield",
                Slot = EquipmentSlot.Offhand,
                WeaponType = WeaponType.Shield,
                BaseBlockChance = 0.14f,
                StatModifiers = new CharacterStats(0, 0, 0, 0, 0, 0, 2, 0, 0)
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Hack", "A brutal hack with the blade.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.1f));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Shield Wall", "Raises shield to absorb incoming blows.", basePower: 0, energyCost: 3,
                targetType: TargetType.Self));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Reckless Swing", "A powerful but reckless swing.", basePower: 0, energyCost: 3,
                range: AbilityRange.Close, damageMultiplier: 1.5f));

            return data;
        }

        public static CharacterData CreateStoneSentinel()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Stone Sentinel",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(5, 2, 1, 8, 1, 1, 14, 3, 1)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Stone Fists",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.TwoHanded,
                BaseDamage = 8
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Pound", "Slams down with a stone fist.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.3f));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Stone Guard", "Hardens its rocky exterior.", basePower: 0, energyCost: 4,
                targetType: TargetType.Self));

            return data;
        }

        public static CharacterData CreateBerserkerCultist()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Berserker Cultist",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(6, 6, 1, 7, 5, 2, 2, 3, 5)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Ritual Axe",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.TwoHanded,
                BaseDamage = 7
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Wild Swing", "A frenzied swing of the axe.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.2f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Frenzy", "Lashes out wildly at all nearby foes.", basePower: 0, energyCost: 4,
                targetType: TargetType.AllEnemies, range: AbilityRange.Close,
                damageMultiplier: 0.9f));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Blood Rage", "Channels pain into fury.", basePower: 0, energyCost: 3,
                targetType: TargetType.Self));

            return data;
        }

        // =============================================
        // REGULAR BACKLINERS
        // =============================================

        public static CharacterData CreateDarkMage()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Dark Mage",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(3, 2, 7, 2, 3, 6, 1, 12, 4)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Dark Staff",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 3
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Shadow Bolt", "Hurls a bolt of shadow energy.", basePower: 8, manaCost: 3,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Curse", "Places a weakening curse on the target.", basePower: 5, manaCost: 4,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Staff Strike", "A desperate swing of the staff.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 0.6f));

            return data;
        }

        public static CharacterData CreateHerbalistShaman()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Herbalist Shaman",
                Class = CharacterClass.Priest,
                BaseStats = new CharacterStats(4, 3, 6, 2, 3, 5, 1, 8, 4)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Herb Staff",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 3
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Herbal Remedy", "Mends an ally's wounds with herbal magic.", basePower: 10, manaCost: 3,
                element: Element.None, targetType: TargetType.SingleAlly));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Nature's Wrath", "Calls upon nature to strike a foe.", basePower: 5, manaCost: 2,
                element: Element.Earth));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Purifying Brew", "Purifies an ally with a cleansing brew.", basePower: 6, manaCost: 4,
                element: Element.None, targetType: TargetType.SingleAlly));

            return data;
        }

        public static CharacterData CreateCrossbowBandit()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Crossbow Bandit",
                Class = CharacterClass.Ranger,
                BaseStats = new CharacterStats(4, 5, 1, 5, 6, 2, 2, 4, 5)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Heavy Crossbow",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Bow,
                BaseDamage = 7
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Crossbow Bolt", "Fires a heavy crossbow bolt.", basePower: 0,
                range: AbilityRange.Reach, damageMultiplier: 1.2f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Aimed Shot", "Takes careful aim for a devastating shot.", basePower: 0, energyCost: 3,
                range: AbilityRange.Reach, damageMultiplier: 1.6f));

            return data;
        }

        public static CharacterData CreateFireImp()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Fire Imp",
                Class = CharacterClass.Elementalist,
                BaseStats = new CharacterStats(2, 3, 6, 1, 5, 5, 0, 10, 7)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Fire Staff",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 2
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Fireball", "Hurls a ball of fire at the target.", basePower: 7, manaCost: 3,
                element: Element.Fire));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Inferno", "Engulfs all enemies in flames.", basePower: 4, manaCost: 5,
                element: Element.Fire, targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Ember Toss", "Flicks a small ember at a foe.", basePower: 3, manaCost: 1,
                element: Element.Fire));

            return data;
        }

        // =============================================
        // ELITES
        // =============================================

        public static CharacterData CreateOrcBrute()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Orc Brute",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(10, 5, 1, 9, 2, 2, 7, 5, 2)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Great Axe",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.TwoHanded,
                BaseDamage = 9
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Cleave", "Sweeps the axe through all nearby enemies.", basePower: 0,
                targetType: TargetType.AllEnemies, range: AbilityRange.Close,
                damageMultiplier: 1.2f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Overhead Smash", "Brings the axe crashing down.", basePower: 0, energyCost: 3,
                range: AbilityRange.Close, damageMultiplier: 2.0f));
            data.Abilities.Add(AbilityData.CreateSkill(
                "War Cry", "Lets out a terrifying war cry.", basePower: 0, energyCost: 3,
                targetType: TargetType.Self));

            return data;
        }

        public static CharacterData CreateWyvernKnight()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Wyvern Knight",
                Class = CharacterClass.Ranger,
                BaseStats = new CharacterStats(8, 6, 2, 7, 7, 3, 5, 6, 7)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Wyvern Lance",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Sword,
                BaseDamage = 7
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Dive Attack", "Swoops down from above for a devastating strike.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.5f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Aerial Strike", "Strikes from the air at range.", basePower: 0, energyCost: 3,
                range: AbilityRange.Reach, damageMultiplier: 1.3f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Wing Gust", "Buffets all enemies with a powerful gust.", basePower: 0, energyCost: 4,
                targetType: TargetType.AllEnemies, range: AbilityRange.Close,
                damageMultiplier: 0.8f));

            return data;
        }

        public static CharacterData CreateNecromancerAdept()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Necromancer Adept",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(7, 2, 9, 2, 3, 8, 2, 12, 3)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Necro Staff",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 3
            };

            data.Abilities.Add(AbilityData.CreateSpell(
                "Death Bolt", "Fires a bolt of necrotic energy.", basePower: 9, manaCost: 3,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Drain Life", "Siphons life force from the target.", basePower: 6, manaCost: 4,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Dark Ritual", "Channels dark power to strengthen itself.", basePower: 0, energyCost: 5,
                targetType: TargetType.Self));

            return data;
        }

        public static CharacterData CreateBladeDancer()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Blade Dancer",
                Class = CharacterClass.Rogue,
                BaseStats = new CharacterStats(7, 7, 1, 6, 8, 2, 3, 4, 8)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Twin Blades",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Dagger,
                BaseDamage = 5
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Blade Flurry", "A whirlwind of slashing blades.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 0.5f, hitCount: 3));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Precise Strike", "A perfectly aimed strike at a weak point.", basePower: 0, energyCost: 3,
                range: AbilityRange.Close, damageMultiplier: 1.8f));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Evasive Dance", "Enters a flowing dance that makes attacks hard to land.", basePower: 0, energyCost: 3,
                targetType: TargetType.Self));

            return data;
        }

        // =============================================
        // BOSSES
        // =============================================

        public static CharacterData CreateMinotaurLord()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Minotaur Lord",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(16, 8, 3, 14, 4, 4, 10, 10, 3)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "War Horns",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.TwoHanded,
                BaseDamage = 11
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Devastating Gore", "Gores the target with massive horns.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 2.0f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Earthquake Stomp", "Stomps the ground, shaking all enemies.", basePower: 0, energyCost: 4,
                targetType: TargetType.AllEnemies, range: AbilityRange.Close,
                damageMultiplier: 1.3f));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Blood Frenzy", "Enters a blood-fueled rage.", basePower: 0, energyCost: 5,
                targetType: TargetType.Self));

            return data;
        }

        public static CharacterData CreateBanditWarlord()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Bandit Warlord",
                Class = CharacterClass.Rogue,
                BaseStats = new CharacterStats(12, 8, 3, 10, 8, 4, 6, 7, 6)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Warlord's Blade",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Sword,
                BaseDamage = 9
            };
            data.Equipment[(int)EquipmentSlot.Offhand] = new EquipmentData
            {
                Name = "Commander's Shield",
                Slot = EquipmentSlot.Offhand,
                WeaponType = WeaponType.Shield,
                BaseBlockChance = 0.15f,
                StatModifiers = new CharacterStats(0, 0, 0, 0, 0, 0, 2, 0, 0)
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Power Strike", "A commanding strike with full force.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.5f));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Rally the Troops", "Rallies allies with a commanding shout.", basePower: 0, energyCost: 5,
                targetType: TargetType.Self));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Backstab", "Exploits an opening for a devastating blow.", basePower: 0, energyCost: 3,
                range: AbilityRange.Close, damageMultiplier: 1.8f));

            return data;
        }
    }
}
