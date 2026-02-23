using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public class GameBootstrap : MonoBehaviour
    {
        private BattleScreenUI _battleScreen;
        private BattleManager _battleManager;

        private void Start()
        {
            SetupCamera();
            InitializeAudio();
            BuildBattleScreen();
            LoadTestBattle();
        }

        private void SetupCamera()
        {
            Camera.main.backgroundColor = Color.black;
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
        }

        private void InitializeAudio()
        {
            GameObject audioGo = new GameObject("AudioManager");
            audioGo.AddComponent<AudioManager>();
        }

        private void BuildBattleScreen()
        {
            GameObject screenGo = new GameObject("BattleScreen");
            _battleScreen = screenGo.AddComponent<BattleScreenUI>();
        }

        private void LoadTestBattle()
        {
            // Pick 4 random classes for the party
            CharacterClass[] allClasses = {
                CharacterClass.Warrior, CharacterClass.Rogue, CharacterClass.Ranger,
                CharacterClass.Priest, CharacterClass.Elementalist, CharacterClass.Warlock
            };
            ShuffleArray(allClasses);

            string[] names = { "Aldric", "Shade", "Elara", "Maren", "Zephyr", "Nyx" };

            List<CharacterData> partyData = new();
            for (int i = 0; i < 4; i++)
            {
                CharacterData data = ClassDefinitions.CreateCharacter(names[i], allClasses[i]);
                EquipDefaultWeapon(data, allClasses[i]);
                partyData.Add(data);
            }

            List<BattleCharacter> players = new List<BattleCharacter>
            {
                new BattleCharacter(partyData[0], TeamSide.Player, GridRow.Front, GridColumn.Left),
                new BattleCharacter(partyData[1], TeamSide.Player, GridRow.Front, GridColumn.Right),
                new BattleCharacter(partyData[2], TeamSide.Player, GridRow.Back, GridColumn.Left),
                new BattleCharacter(partyData[3], TeamSide.Player, GridRow.Back, GridColumn.Right),
            };

            List<BattleCharacter> enemies = new List<BattleCharacter>
            {
                new BattleCharacter(
                    EnemyDefinitions.CreateEnemy(EnemyType.Ratman),
                    TeamSide.Enemy, GridRow.Front, GridColumn.Left),
                new BattleCharacter(
                    EnemyDefinitions.CreateEnemy(EnemyType.Ratman),
                    TeamSide.Enemy, GridRow.Front, GridColumn.Right),
                new BattleCharacter(
                    EnemyDefinitions.CreateEnemy(EnemyType.GoblinArcher),
                    TeamSide.Enemy, GridRow.Back, GridColumn.Left),
                new BattleCharacter(
                    EnemyDefinitions.CreateEnemy(EnemyType.GoblinArcher),
                    TeamSide.Enemy, GridRow.Back, GridColumn.Right),
            };

            _battleManager = gameObject.AddComponent<BattleManager>();
            _battleManager.StartBattle(players, enemies, _battleScreen);
        }

        private static void EquipDefaultWeapon(CharacterData data, CharacterClass characterClass)
        {
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
                    {
                        Name = "Iron Sword",
                        Slot = EquipmentSlot.Hand1,
                        WeaponType = WeaponType.Sword,
                        BaseDamage = 6
                    };
                    data.Equipment[(int)EquipmentSlot.Offhand] = new EquipmentData
                    {
                        Name = "Wooden Shield",
                        Slot = EquipmentSlot.Offhand,
                        WeaponType = WeaponType.Shield,
                        BaseBlockChance = 0.15f,
                        StatModifiers = new CharacterStats(0, 0, 0, 0, 0, 0, 2, 0, 0)
                    };
                    break;
                case CharacterClass.Rogue:
                    data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
                    {
                        Name = "Sharp Dagger",
                        Slot = EquipmentSlot.Hand1,
                        WeaponType = WeaponType.Dagger,
                        BaseDamage = 4
                    };
                    break;
                case CharacterClass.Ranger:
                    data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
                    {
                        Name = "Hunting Bow",
                        Slot = EquipmentSlot.Hand1,
                        WeaponType = WeaponType.Bow,
                        BaseDamage = 5
                    };
                    break;
                case CharacterClass.Priest:
                    data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
                    {
                        Name = "Oak Staff",
                        Slot = EquipmentSlot.Hand1,
                        WeaponType = WeaponType.Staff,
                        BaseDamage = 3
                    };
                    break;
                case CharacterClass.Elementalist:
                    data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
                    {
                        Name = "Crystal Staff",
                        Slot = EquipmentSlot.Hand1,
                        WeaponType = WeaponType.Staff,
                        BaseDamage = 3
                    };
                    break;
                case CharacterClass.Warlock:
                    data.Equipment[(int)EquipmentSlot.Hand1] = new EquipmentData
                    {
                        Name = "Ritual Dagger",
                        Slot = EquipmentSlot.Hand1,
                        WeaponType = WeaponType.Dagger,
                        BaseDamage = 4
                    };
                    break;
            }
        }

        private static void ShuffleArray<T>(T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
    }
}
