using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public class GameBootstrap : MonoBehaviour
    {
        private BattleScreenUI _battleScreen;

        private void Start()
        {
            SetupCamera();
            BuildBattleScreen();
            LoadTestBattle();
        }

        private void SetupCamera()
        {
            Camera.main.backgroundColor = Color.black;
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
        }

        private void BuildBattleScreen()
        {
            GameObject screenGo = new GameObject("BattleScreen");
            _battleScreen = screenGo.AddComponent<BattleScreenUI>();
        }

        private void LoadTestBattle()
        {
            // Create test player party
            List<BattleCharacter> players = new List<BattleCharacter>
            {
                new BattleCharacter(
                    ClassDefinitions.CreateCharacter("Aldric", CharacterClass.Warrior),
                    TeamSide.Player, GridRow.Front, GridColumn.Left),
                new BattleCharacter(
                    ClassDefinitions.CreateCharacter("Shade", CharacterClass.Rogue),
                    TeamSide.Player, GridRow.Front, GridColumn.Right),
                new BattleCharacter(
                    ClassDefinitions.CreateCharacter("Elara", CharacterClass.Ranger),
                    TeamSide.Player, GridRow.Back, GridColumn.Left),
                new BattleCharacter(
                    ClassDefinitions.CreateCharacter("Maren", CharacterClass.Priest),
                    TeamSide.Player, GridRow.Back, GridColumn.Right),
            };

            // Create test enemy group
            List<BattleCharacter> enemies = new List<BattleCharacter>
            {
                new BattleCharacter(
                    EnemyDefinitions.CreateEnemy("Ratman"),
                    TeamSide.Enemy, GridRow.Front, GridColumn.Left),
                new BattleCharacter(
                    EnemyDefinitions.CreateEnemy("Ratman"),
                    TeamSide.Enemy, GridRow.Front, GridColumn.Right),
                new BattleCharacter(
                    EnemyDefinitions.CreateEnemy("GoblinArcher"),
                    TeamSide.Enemy, GridRow.Back, GridColumn.Left),
                new BattleCharacter(
                    EnemyDefinitions.CreateEnemy("GoblinArcher"),
                    TeamSide.Enemy, GridRow.Back, GridColumn.Right),
            };

            // Populate UI
            _battleScreen.BattleGrid.SetEnemies(enemies);
            _battleScreen.BattleGrid.SetPlayers(players);
            _battleScreen.AbilityPanel.SetCharacter(players[0]);

            // Test log messages
            GameEvents.RaiseCombatLogMessage("Battle begins!");
            GameEvents.RaiseCombatLogMessage("Aldric's party vs Ratmen and Goblins!");
        }
    }
}
