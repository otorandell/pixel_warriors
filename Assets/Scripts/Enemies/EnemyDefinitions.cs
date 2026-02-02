using System.Collections.Generic;

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
                BaseStats = new CharacterStats(3, 4, 2, 3, 5, 2, 1, 1, 6)
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Shoot", "Fires an arrow.", basePower: 5));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Poison Arrow", "Fires a poisoned arrow.", basePower: 4, energyCost: 2));

            return data;
        }

        private static CharacterData CreateRatman()
        {
            var data = new CharacterData
            {
                Name = "Ratman",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(5, 4, 1, 5, 4, 1, 3, 1, 4)
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Claw", "Slashes with claws.", basePower: 6));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Bite", "A vicious bite.", basePower: 8, energyCost: 2));

            return data;
        }

        private static CharacterData CreateMinotaur()
        {
            var data = new CharacterData
            {
                Name = "Minotaur",
                Class = CharacterClass.Warrior,
                BaseStats = new CharacterStats(12, 6, 2, 10, 3, 3, 8, 2, 2)
            };

            data.Abilities.Add(AbilityData.CreateAttack(
                "Gore", "Charges and gores the target.", basePower: 14));
            data.Abilities.Add(AbilityData.CreateAttack(
                "Stomp", "Stomps the ground, hitting all enemies.", basePower: 8,
                targetType: TargetType.AllEnemies));
            data.Abilities.Add(AbilityData.CreateSkill(
                "Enrage", "Increases strength temporarily.", basePower: 0, energyCost: 4,
                targetType: TargetType.Self));

            return data;
        }
    }
}
