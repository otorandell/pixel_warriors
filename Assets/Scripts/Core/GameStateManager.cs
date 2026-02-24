using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public class GameStateManager : MonoBehaviour
    {
        private ScreenManager _screenManager;
        private RunData _runData;

        public void Initialize(ScreenManager screenManager)
        {
            _screenManager = screenManager;
            StartCoroutine(GameLoop());
        }

        private IEnumerator GameLoop()
        {
            while (true)
            {
                yield return MainMenuPhase();
                yield return RunPhase();
            }
        }

        private IEnumerator MainMenuPhase()
        {
            var menuScreen = new MainMenuScreen();
            _screenManager.TransitionTo(menuScreen);

            while (!menuScreen.StartPressed)
                yield return null;

            menuScreen.Hide();
        }

        private IEnumerator RunPhase()
        {
            _runData = new RunData();

            // TODO Phase G: PartySetupScreen. For now, create 2 random characters.
            CreateStartingParty();

            while (!_runData.IsRunComplete)
            {
                // Room choice
                yield return RoomChoicePhase();

                // Execute room
                RoomType room = _runData.CurrentRoom ?? RoomType.Battle;
                switch (room)
                {
                    case RoomType.Battle:
                    case RoomType.EliteBattle:
                    case RoomType.BossBattle:
                        yield return BattlePhase();
                        break;

                    case RoomType.Shop:
                    case RoomType.Rest:
                    case RoomType.Event:
                    case RoomType.Recruit:
                        // TODO Phase F/G: proper screens. For now, stub with a brief pause.
                        yield return StubRoomPhase(room);
                        break;
                }

                if (_runData.Party.Count == 0)
                {
                    yield return GameOverPhase(false);
                    yield break;
                }

                _runData.PreviousRoom = _runData.CurrentRoom;
                _runData.AdvanceFloor();
            }

            yield return GameOverPhase(true);
        }

        private IEnumerator RoomChoicePhase()
        {
            List<RoomType> choices = FloorGenerator.GenerateRoomChoices(_runData);

            // Boss floors skip the choice screen
            if (choices.Count == 1 && choices[0] == RoomType.BossBattle)
            {
                _runData.CurrentRoom = RoomType.BossBattle;
                yield break;
            }

            var choiceScreen = new RoomChoiceScreen(_runData, choices);
            _screenManager.TransitionTo(choiceScreen);

            while (choiceScreen.SelectedRoom == null)
                yield return null;

            _runData.CurrentRoom = choiceScreen.SelectedRoom;
            choiceScreen.Destroy();
        }

        private IEnumerator StubRoomPhase(RoomType room)
        {
            string roomName = FloorGenerator.GetRoomName(room);

            if (room == RoomType.Rest)
            {
                // Rest heals the party
                foreach (CharacterData c in _runData.Party)
                {
                    CharacterStats total = c.GetTotalStats();
                    int maxHP = StatCalculator.CalculateMaxHP(total);
                    int missingHP = maxHP - c.CurrentHP;
                    int healAmount = Mathf.RoundToInt(missingHP * RunConfig.RestHealPercent);
                    c.CurrentHP = Mathf.Min(c.CurrentHP + healAmount, maxHP);
                    c.CurrentEnergy = StatCalculator.CalculateMaxEnergy(total);
                    c.CurrentMana = StatCalculator.CalculateMaxMana(total);
                }
                Debug.Log($"[{roomName}] Party rested. HP restored.");
            }
            else
            {
                Debug.Log($"[{roomName}] Not yet implemented — skipping.");
            }

            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator BattlePhase()
        {
            var battleScreen = new BattleScreenUI();
            _screenManager.TransitionTo(battleScreen);

            List<BattleCharacter> players = CreateBattleParty();
            RoomType roomType = _runData.CurrentRoom ?? RoomType.Battle;
            EncounterData encounterData = EncounterGenerator.GenerateEncounter(_runData, roomType);
            List<BattleCharacter> enemies = GridSlotUtil.PlaceCharacters(
                encounterData.InitialEnemies, TeamSide.Enemy);

            BattleManager battleManager = gameObject.AddComponent<BattleManager>();
            battleManager.StartBattle(players, enemies, encounterData, battleScreen);

            while (!battleManager.IsFinished)
                yield return null;

            BattleResult result = battleManager.Result;
            _runData.TotalBattles++;

            // Count kills (includes reinforcements that were added to the enemies list)
            foreach (BattleCharacter enemy in battleManager.Enemies)
            {
                if (!enemy.IsAlive) _runData.TotalKills++;
            }

            // Wait for player to see the result
            yield return new WaitForSeconds(1.5f);

            // Stop battle coroutines before destroying UI
            battleManager.StopAllCoroutines();
            Destroy(battleManager);
            battleScreen.Destroy();

            if (result == BattleResult.Defeat)
            {
                // Sync so we can show who fell
                foreach (BattleCharacter bc in players)
                    bc.SyncToData();
                _runData.Party.Clear();
            }
            else
            {
                // Victory: process XP, gold, permadeath, healing
                RoomType room = _runData.CurrentRoom ?? RoomType.Battle;
                PostBattleResult postResult = PostBattleProcessor.Process(_runData, players, room);

                var postScreen = new PostBattleScreen(postResult, _runData.Party);
                _screenManager.TransitionTo(postScreen);

                while (!postScreen.ContinuePressed)
                    yield return null;

                postScreen.Destroy();
            }
        }

        private IEnumerator GameOverPhase(bool victory)
        {
            // TODO Phase G: GameOverScreen. For now, log and return to menu.
            var menuScreen = new MainMenuScreen();
            _screenManager.TransitionTo(menuScreen);

            // Show a simple message via the menu for now
            Debug.Log(victory
                ? $"Run complete! Battles: {_runData.TotalBattles}, Kills: {_runData.TotalKills}"
                : $"Game Over. Battles: {_runData.TotalBattles}, Kills: {_runData.TotalKills}");

            while (!menuScreen.StartPressed)
                yield return null;

            menuScreen.Hide();
        }

        private void CreateStartingParty()
        {
            CharacterClass[] allClasses = {
                CharacterClass.Warrior, CharacterClass.Rogue, CharacterClass.Ranger,
                CharacterClass.Priest, CharacterClass.Elementalist, CharacterClass.Warlock
            };
            ShuffleArray(allClasses);

            string[] names = { "Aldric", "Shade", "Elara", "Maren", "Zephyr", "Nyx" };

            for (int i = 0; i < RunConfig.StartingPartySize; i++)
            {
                CharacterData data = ClassDefinitions.CreateCharacter(names[i], allClasses[i]);
                EquipDefaultWeapon(data, allClasses[i]);
                _runData.Party.Add(data);
            }
        }

        private List<BattleCharacter> CreateBattleParty()
        {
            return GridSlotUtil.PlaceCharacters(_runData.Party, TeamSide.Player);
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
