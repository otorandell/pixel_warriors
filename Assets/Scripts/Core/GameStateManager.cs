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
                        yield return ShopPhase();
                        break;

                    case RoomType.Recruit:
                        yield return RecruitPhase();
                        break;

                    case RoomType.Rest:
                        yield return RestPhase();
                        break;

                    case RoomType.Event:
                        yield return EventPhase();
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

            while (true)
            {
                var choiceScreen = new RoomChoiceScreen(_runData, choices);
                _screenManager.TransitionTo(choiceScreen);

                while (choiceScreen.SelectedRoom == null && !choiceScreen.InventoryRequested)
                    yield return null;

                if (choiceScreen.InventoryRequested)
                {
                    choiceScreen.Destroy();

                    var inventoryScreen = new InventoryScreen(_runData);
                    _screenManager.TransitionTo(inventoryScreen);

                    while (!inventoryScreen.ClosePressed)
                        yield return null;

                    inventoryScreen.Destroy();
                    continue;
                }

                _runData.CurrentRoom = choiceScreen.SelectedRoom;
                choiceScreen.Destroy();
                break;
            }
        }

        private IEnumerator StubRoomPhase(RoomType room)
        {
            string roomName = FloorGenerator.GetRoomName(room);
            Debug.Log($"[{roomName}] Not yet implemented — skipping.");
            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator RestPhase()
        {
            EventData restEvent = EventCatalog.GetRestEvent();
            var eventScreen = new EventScreen(restEvent, _runData);
            _screenManager.TransitionTo(eventScreen);

            while (!eventScreen.Done)
                yield return null;

            eventScreen.Destroy();
        }

        private IEnumerator EventPhase()
        {
            EventData eventData = EventCatalog.RollEvent(_runData);
            var eventScreen = new EventScreen(eventData, _runData);
            _screenManager.TransitionTo(eventScreen);

            while (!eventScreen.Done)
                yield return null;

            eventScreen.Destroy();
        }

        private IEnumerator ShopPhase()
        {
            ShopStock stock = ShopGenerator.GenerateShopStock(_runData);

            while (true)
            {
                var shopScreen = new ShopScreen(_runData, stock);
                _screenManager.TransitionTo(shopScreen);

                while (!shopScreen.ExitRequested && !shopScreen.InventoryRequested)
                    yield return null;

                if (shopScreen.InventoryRequested)
                {
                    shopScreen.Destroy();

                    var inventoryScreen = new InventoryScreen(_runData);
                    _screenManager.TransitionTo(inventoryScreen);

                    while (!inventoryScreen.ClosePressed)
                        yield return null;

                    inventoryScreen.Destroy();
                    continue;
                }

                shopScreen.Destroy();
                break;
            }
        }

        private IEnumerator RecruitPhase()
        {
            // Generate 2 candidates with classes not already in party
            List<CharacterClass> usedClasses = new();
            foreach (CharacterData c in _runData.Party)
                usedClasses.Add(c.Class);

            List<CharacterClass> availableClasses = new();
            CharacterClass[] allClasses = {
                CharacterClass.Warrior, CharacterClass.Rogue, CharacterClass.Ranger,
                CharacterClass.Priest, CharacterClass.Elementalist, CharacterClass.Warlock
            };
            foreach (CharacterClass cls in allClasses)
            {
                if (!usedClasses.Contains(cls))
                    availableClasses.Add(cls);
            }

            ShuffleList(availableClasses);
            int candidateCount = Mathf.Min(2, availableClasses.Count);

            // Pick unused names
            string[] allNames = { "Aldric", "Shade", "Elara", "Maren", "Zephyr", "Nyx" };
            List<string> usedNames = new();
            foreach (CharacterData c in _runData.Party)
                usedNames.Add(c.Name);

            List<string> freeNames = new();
            foreach (string n in allNames)
            {
                if (!usedNames.Contains(n))
                    freeNames.Add(n);
            }

            List<CharacterData> candidates = new();
            for (int i = 0; i < candidateCount; i++)
            {
                string name = i < freeNames.Count ? freeNames[i] : $"Recruit_{i}";
                CharacterData candidate = ClassDefinitions.CreateCharacter(name, availableClasses[i]);
                EquipDefaultWeapon(candidate, availableClasses[i]);
                candidates.Add(candidate);
            }

            var recruitScreen = new RecruitScreen(candidates, _runData);
            _screenManager.TransitionTo(recruitScreen);

            while (!recruitScreen.Done)
                yield return null;

            if (recruitScreen.RecruitedCharacter != null)
            {
                _runData.Party.Add(recruitScreen.RecruitedCharacter);
            }

            recruitScreen.Destroy();
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
            battleManager.StartBattle(players, enemies, encounterData, battleScreen, _runData);

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

                var postScreen = new PostBattleScreen(postResult, _runData.Party, _runData);
                _screenManager.TransitionTo(postScreen);

                while (true)
                {
                    while (!postScreen.ContinuePressed && !postScreen.InventoryRequested)
                        yield return null;

                    if (postScreen.InventoryRequested)
                    {
                        postScreen.Hide();

                        // Build inventory directly on canvas — don't use TransitionTo
                        // which would destroy the hidden post-battle screen
                        var inventoryScreen = new InventoryScreen(_runData);
                        inventoryScreen.Build(_screenManager.CanvasParent);
                        inventoryScreen.Show();

                        while (!inventoryScreen.ClosePressed)
                            yield return null;

                        inventoryScreen.Destroy();
                        postScreen.Show();
                        postScreen.RefreshLoot();
                        continue;
                    }

                    break;
                }

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

        private static void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
