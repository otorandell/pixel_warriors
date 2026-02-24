namespace PixelWarriors
{
    public static class Act1Enemies
    {
        // =============================================
        // REGULAR FRONTLINERS
        // =============================================

        public static CharacterData CreateRatman()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Ratman",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(5, 4, 1, 5, 4, 1, 3, 5, 4)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Claws",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Sword,
                BaseDamage = 5
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Claw", "Slashes with claws.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.0f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Bite", "A vicious bite.", basePower: 0, energyCost: 2,
                range: AbilityRange.Close, damageMultiplier: 1.3f));

            return data;
        }

        public static CharacterData CreateSkeleton()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Skeleton",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(6, 3, 1, 6, 3, 2, 5, 3, 3)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Rusty Sword",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Sword,
                BaseDamage = 5
            };
            data.Equipment[(int)EquipmentSlot.Offhand] = new EquipmentData
            {
                Name = "Bone Shield",
                Slot = EquipmentSlot.Offhand,
                WeaponType = WeaponType.Shield,
                BaseBlockChance = 0.12f,
                StatModifiers = new CharacterStats(0, 0, 0, 0, 0, 0, 2, 0, 0)
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Slash", "A slow sword swing.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.0f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Shield Bash", "Bashes with the shield.", basePower: 0, energyCost: 2,
                range: AbilityRange.Close, damageMultiplier: 0.8f));

            return data;
        }

        public static CharacterData CreateZombieShambler()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Zombie",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(10, 2, 1, 7, 1, 1, 4, 2, 1)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Rotting Fists",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Mace,
                BaseDamage = 6
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Slam", "A heavy, clumsy slam.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.2f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Grab", "Grabs and crushes.", basePower: 0, energyCost: 3,
                range: AbilityRange.Close, damageMultiplier: 1.6f));

            return data;
        }

        public static CharacterData CreateFungusCreeper()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Fungus Creeper",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(4, 3, 3, 3, 3, 4, 2, 6, 3)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Toxic Tendrils",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Sword,
                BaseDamage = 3
            };

            // Low damage but applies poison via Envenom-like effect
            data.Abilities.Add(AbilityData.CreateAttack(
                "Toxic Touch", "Spreads spores on contact.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 0.7f));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Spore Cloud", "Releases a cloud of toxic spores.", basePower: 4, manaCost: 3,
                element: Element.Earth, targetType: TargetType.AllEnemies));

            return data;
        }

        // =============================================
        // REGULAR BACKLINERS
        // =============================================

        public static CharacterData CreateGoblinArcher()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Goblin Archer",
                Class = CharacterClass.Ranger,
                BaseStats = new CharacterStats(3, 4, 2, 3, 5, 2, 1, 5, 6)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Crude Bow",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Bow,
                BaseDamage = 4
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Shoot", "Fires an arrow.", basePower: 0,
                range: AbilityRange.Reach, damageMultiplier: 1.0f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Poison Arrow", "Fires a poisoned arrow.", basePower: 0, energyCost: 2,
                range: AbilityRange.Reach, damageMultiplier: 0.8f));

            return data;
        }

        public static CharacterData CreateSwarmBat()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Swarm Bat",
                Class = CharacterClass.Rogue,
                BaseStats = new CharacterStats(2, 5, 1, 2, 6, 1, 0, 3, 8)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Fangs",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Dagger,
                BaseDamage = 2
            };

            // Low damage per hit but multi-hit chips through shields
            data.Abilities.Add(AbilityData.CreateAttack(
                "Frenzy", "A flurry of bites.", basePower: 0,
                range: AbilityRange.Reach, damageMultiplier: 0.4f, hitCount: 3));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Dive", "Swoops down on a target.", basePower: 0, energyCost: 2,
                range: AbilityRange.Reach, damageMultiplier: 1.0f));

            return data;
        }

        public static CharacterData CreateTunnelRat()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Tunnel Rat",
                Class = CharacterClass.Priest,
                BaseStats = new CharacterStats(3, 3, 4, 2, 4, 4, 1, 5, 5)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Tiny Claws",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Dagger,
                BaseDamage = 2
            };

            // Healer: priority kill target
            data.Abilities.Add(AbilityData.CreateAttack(
                "Scratch", "A feeble scratch.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 0.6f));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Mend", "Patches up an ally.", basePower: 8, manaCost: 3,
                element: Element.None, targetType: TargetType.SingleAlly));

            return data;
        }

        // =============================================
        // ELITES
        // =============================================

        public static CharacterData CreateMinotaur()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Minotaur",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(12, 6, 2, 10, 3, 3, 8, 8, 2)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Horns",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.TwoHanded,
                BaseDamage = 8
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Gore", "Charges and gores the target.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.8f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Stomp", "Stomps the ground, hitting all enemies.", basePower: 0,
                targetType: TargetType.AllEnemies, range: AbilityRange.Close,
                damageMultiplier: 1.0f));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Enrage", "Increases strength temporarily.", basePower: 0, energyCost: 4,
                targetType: TargetType.Self));

            return data;
        }

        public static CharacterData CreateGiantSpider()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Giant Spider",
                Class = CharacterClass.Rogue,
                BaseStats = new CharacterStats(9, 5, 3, 6, 6, 4, 4, 6, 5)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Venomous Fangs",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Dagger,
                BaseDamage = 6
            };

            // CC + poison: crowd control elite
            data.Abilities.Add(AbilityData.CreateAttack(
                "Venom Strike", "Bites with venomous fangs.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.2f));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Web Trap", "Ensnares a target in webbing.", basePower: 3, manaCost: 4,
                element: Element.Earth));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Acid Spit", "Sprays acid at all enemies.", basePower: 0, energyCost: 4,
                targetType: TargetType.AllEnemies, range: AbilityRange.Reach,
                damageMultiplier: 0.7f));

            return data;
        }

        public static CharacterData CreateBoneLord()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Bone Lord",
                Class = CharacterClass.Warlock,
                BaseStats = new CharacterStats(10, 3, 8, 4, 3, 7, 6, 10, 3)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Bone Staff",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Staff,
                BaseDamage = 4
            };

            // Gang leader: summons skeleton adds
            data.Abilities.Add(AbilityData.CreateSpell(
                "Bone Lance", "Hurls a shard of bone.", basePower: 7, manaCost: 3,
                element: Element.Arcane));
            data.Abilities.Add(AbilityData.CreateSpell(
                "Dark Shield", "Wraps in a shield of dark energy.", basePower: 10, manaCost: 4,
                element: Element.Arcane, targetType: TargetType.Self));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Staff Strike", "A desperate swing.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 0.6f));

            return data;
        }

        // =============================================
        // BOSSES
        // =============================================

        public static CharacterData CreateGoblinKing()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Goblin King",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(10, 8, 3, 8, 6, 4, 6, 8, 5)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Gilded Mace",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.Mace,
                BaseDamage = 7
            };
            data.Equipment[(int)EquipmentSlot.Offhand] = new EquipmentData
            {
                Name = "Royal Shield",
                Slot = EquipmentSlot.Offhand,
                WeaponType = WeaponType.Shield,
                BaseBlockChance = 0.18f,
                StatModifiers = new CharacterStats(0, 0, 0, 0, 0, 0, 3, 0, 0)
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Royal Smash", "A crowned authority strike.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.5f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Rally", "Rallies nearby allies.", basePower: 0,
                energyCost: 4, targetType: TargetType.Self, range: AbilityRange.Any));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Sweeping Strike", "Hits all enemies in reach.", basePower: 0,
                energyCost: 3, targetType: TargetType.AllEnemies,
                range: AbilityRange.Close, damageMultiplier: 1.0f));

            return data;
        }

        public static CharacterData CreateCatacombGuardian()
        {
            //                                    END STA INT STR DEX WIL ARM MRS INI
            var data = new CharacterData
            {
                Name = "Catacomb Guardian",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(14, 5, 2, 9, 2, 3, 12, 6, 1)
            };

            data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
            {
                Name = "Stone Fists",
                Slot = EquipmentSlot.Hand1,
                WeaponType = WeaponType.TwoHanded,
                BaseDamage = 9
            };

            // High armor boss: tests armor penetration and magic damage
            data.Abilities.Add(AbilityData.CreateAttack(
                "Crushing Blow", "A devastating overhead slam.", basePower: 0,
                range: AbilityRange.Close, damageMultiplier: 1.6f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Ground Slam", "Shakes the ground violently.", basePower: 0,
                energyCost: 4, targetType: TargetType.AllEnemies,
                range: AbilityRange.Close, damageMultiplier: 1.0f));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Stone Skin", "Hardens its stony exterior.", basePower: 0, energyCost: 5,
                targetType: TargetType.Self));

            return data;
        }
    }
}
