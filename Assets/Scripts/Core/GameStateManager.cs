using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PixelWarriors
{
    public class GameStateManager : MonoBehaviour
    {
        private ScreenManager _screenManager;
        private RunData _runData;
        private bool _continueRun;

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
            _continueRun = false;

            var menuScreen = new MainMenuScreen();
            _screenManager.TransitionTo(menuScreen);

            while (!menuScreen.StartPressed && !menuScreen.ContinuePressed)
                yield return null;

            if (menuScreen.StartPressed && SaveManager.HasSave())
                SaveManager.DeleteSave();

            _continueRun = menuScreen.ContinuePressed;
            menuScreen.Hide();
        }

        private IEnumerator PartySetupPhase()
        {
            var setupScreen = new PartySetupScreen();
            yield return FadeTransitionTo(setupScreen);

            while (!setupScreen.Done)
                yield return null;

            List<CharacterClass> chosen = setupScreen.SelectedClasses;
            string[] names = { "Aldric", "Shade", "Elara", "Maren", "Zephyr", "Nyx" };

            // Shuffle names so party members get varied names each run
            ShuffleArray(names);

            for (int i = 0; i < chosen.Count; i++)
            {
                CharacterData data = ClassDefinitions.CreateCharacter(names[i], chosen[i]);
                EquipDefaultWeapon(data, chosen[i]);
                _runData.Party.Add(data);
            }

            setupScreen.Destroy();
        }

        private IEnumerator RunPhase()
        {
            if (_continueRun)
            {
                _runData = SaveManager.Load();
                if (_runData == null)
                {
                    Debug.LogWarning("[GameStateManager] Save corrupted, starting new run.");
                    _runData = new RunData();
                    yield return PartySetupPhase();
                }
            }
            else
            {
                _runData = new RunData();
                LevelingSystem.ResetOfferTracking();
                yield return PartySetupPhase();
            }

            SaveManager.Save(_runData);

            while (!_runData.IsRunComplete)
            {
                // Generate floor plan if needed (new act or fresh run)
                if (_runData.FloorPlan == null || _runData.FloorPlan.Count == 0)
                    _runData.FloorPlan = FloorGenerator.GenerateActPlan(_runData.CurrentAct);

                FloorPlanEntry floor = FloorGenerator.GetCurrentFloor(_runData);

                switch (floor.Category)
                {
                    case FloorCategory.Boss:
                        _runData.CurrentRoom = RoomType.BossBattle;
                        yield return BattlePhase();
                        break;

                    case FloorCategory.Battle:
                        yield return BattleOrBonusChoicePhase(floor);
                        yield return ExecuteCurrentRoom();
                        break;

                    case FloorCategory.Bonus:
                        yield return BattleOrBonusChoicePhase(floor);
                        yield return ExecuteCurrentRoom();
                        break;

                    case FloorCategory.Event:
                        _runData.CurrentRoom = RoomType.Event;
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

                // Clear floor plan when advancing to new act
                if (_runData.CurrentFloor == 1)
                    _runData.FloorPlan = FloorGenerator.GenerateActPlan(_runData.CurrentAct);

                SaveManager.Save(_runData);
            }

            yield return GameOverPhase(true);
        }

        private IEnumerator BattleOrBonusChoicePhase(FloorPlanEntry floor)
        {
            bool firstShow = true;
            while (true)
            {
                var choiceScreen = new RoomChoiceScreen(_runData, floor);
                if (firstShow)
                {
                    yield return FadeTransitionTo(choiceScreen);
                    firstShow = false;
                }
                else
                {
                    _screenManager.TransitionTo(choiceScreen);
                }

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

        private IEnumerator ExecuteCurrentRoom()
        {
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
                case RoomType.Rest:
                    yield return RestPhase();
                    break;
                case RoomType.Smithy:
                    yield return SmithyPhase();
                    break;
            }
        }

        private IEnumerator RestPhase()
        {
            EventData restEvent = EventCatalog.GetRestEvent();
            var eventScreen = new EventScreen(restEvent, _runData);
            yield return FadeTransitionTo(eventScreen);

            while (!eventScreen.Done)
                yield return null;

            eventScreen.Destroy();

            // Check if rest choice granted a level-up (STUDY THE AREA)
            if (_runData.PendingEventLevelUp)
            {
                _runData.PendingEventLevelUp = false;
                List<int> candidates = LevelingSystem.GetLevelUpCandidates(_runData.Party);
                if (candidates.Count > 0)
                {
                    var levelScreen = new LevelUpPickScreen(_runData.Party, candidates);
                    yield return FadeTransitionTo(levelScreen);

                    while (!levelScreen.Done)
                        yield return null;

                    levelScreen.Destroy();
                }
            }
        }

        private IEnumerator EventPhase()
        {
            EventData eventData = EventCatalog.RollEvent(_runData);
            var eventScreen = new EventScreen(eventData, _runData);
            yield return FadeTransitionTo(eventScreen);

            while (!eventScreen.Done)
                yield return null;

            eventScreen.Destroy();

            // Check if event granted a level-up
            if (_runData.PendingEventLevelUp)
            {
                _runData.PendingEventLevelUp = false;
                List<int> candidates = LevelingSystem.GetLevelUpCandidates(_runData.Party);
                if (candidates.Count > 0)
                {
                    var levelScreen = new LevelUpPickScreen(_runData.Party, candidates);
                    yield return FadeTransitionTo(levelScreen);

                    while (!levelScreen.Done)
                        yield return null;

                    levelScreen.Destroy();
                }
            }
        }

        private IEnumerator ShopPhase()
        {
            ShopStock stock = ShopGenerator.GenerateShopStock(_runData);

            bool firstShow = true;
            while (true)
            {
                var shopScreen = new ShopScreen(_runData, stock);
                if (firstShow)
                {
                    yield return FadeTransitionTo(shopScreen);
                    firstShow = false;
                }
                else
                {
                    _screenManager.TransitionTo(shopScreen);
                }

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

        private IEnumerator SmithyPhase()
        {
            bool firstShow = true;
            while (true)
            {
                var smithyScreen = new SmithyScreen(_runData);
                if (firstShow)
                {
                    yield return FadeTransitionTo(smithyScreen);
                    firstShow = false;
                }
                else
                {
                    _screenManager.TransitionTo(smithyScreen);
                }

                while (!smithyScreen.ExitRequested && !smithyScreen.InventoryRequested)
                    yield return null;

                if (smithyScreen.InventoryRequested)
                {
                    smithyScreen.Destroy();

                    var inventoryScreen = new InventoryScreen(_runData);
                    _screenManager.TransitionTo(inventoryScreen);

                    while (!inventoryScreen.ClosePressed)
                        yield return null;

                    inventoryScreen.Destroy();
                    continue;
                }

                smithyScreen.Destroy();
                break;
            }
        }

        private IEnumerator BattlePhase()
        {
            var battleScreen = new BattleScreenUI();
            yield return FadeTransitionTo(battleScreen);

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

                var postScreen = new PostBattleScreen(postResult, _runData);
                yield return FadeTransitionTo(postScreen);

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

                // --- Level-up picks ---
                int levelUps = postResult.IsBossBattle ? RunConfig.BossLevelUps : 1;
                for (int i = 0; i < levelUps; i++)
                {
                    List<int> candidates = LevelingSystem.GetLevelUpCandidates(_runData.Party);
                    if (candidates.Count == 0) break;

                    var levelScreen = new LevelUpPickScreen(_runData.Party, candidates);
                    yield return FadeTransitionTo(levelScreen);

                    while (!levelScreen.Done)
                        yield return null;

                    levelScreen.Destroy();
                }
            }
        }

        private IEnumerator GameOverPhase(bool victory)
        {
            SaveManager.DeleteSave();

            var gameOverScreen = new GameOverScreen(_runData, victory);
            yield return FadeTransitionTo(gameOverScreen);

            while (!gameOverScreen.MenuPressed)
                yield return null;

            gameOverScreen.Destroy();
        }


        private IEnumerator FadeTransitionTo(IScreen screen)
        {
            yield return _screenManager.FadeOut().WaitForCompletion();
            _screenManager.TransitionTo(screen);
            yield return _screenManager.FadeIn().WaitForCompletion();
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
