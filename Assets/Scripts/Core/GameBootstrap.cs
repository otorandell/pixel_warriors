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
    }
}
