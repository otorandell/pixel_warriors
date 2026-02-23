namespace PixelWarriors
{
    public static class EnemyDefinitions
    {
        public static CharacterData CreateEnemy(EnemyType enemyType)
        {
            return enemyType switch
            {
                EnemyType.GoblinArcher => CreateGoblinArcher(),
                EnemyType.Ratman => CreateRatman(),
                EnemyType.Minotaur => CreateMinotaur(),
                _ => CreateRatman()
            };
        }

        private static CharacterData CreateGoblinArcher()
        {
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
                "Shoot", "Fires an arrow.", basePower: 0, range: AbilityRange.Reach,
                damageMultiplier: 1.0f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Poison Arrow", "Fires a poisoned arrow.", basePower: 0, energyCost: 2,
                range: AbilityRange.Reach, damageMultiplier: 0.8f));

            return data;
        }

        private static CharacterData CreateRatman()
        {
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
                "Claw", "Slashes with claws.", basePower: 0, range: AbilityRange.Close,
                damageMultiplier: 1.0f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Bite", "A vicious bite.", basePower: 0, energyCost: 2, range: AbilityRange.Close,
                damageMultiplier: 1.3f));

            return data;
        }

        private static CharacterData CreateMinotaur()
        {
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
                "Gore", "Charges and gores the target.", basePower: 0, range: AbilityRange.Close,
                damageMultiplier: 1.8f));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Stomp", "Stomps the ground, hitting all enemies.", basePower: 0,
                targetType: TargetType.AllEnemies, range: AbilityRange.Close,
                damageMultiplier: 1.0f));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Enrage", "Increases strength temporarily.", basePower: 0, energyCost: 4,
                targetType: TargetType.Self));

            return data;
        }
    }
}
