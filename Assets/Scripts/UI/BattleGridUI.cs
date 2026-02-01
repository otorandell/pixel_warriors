using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class BattleGridUI
    {
        public RectTransform Root { get; private set; }

        private RectTransform _enemyGrid;
        private RectTransform _playerGrid;
        private RectTransform _divider;
        private readonly List<CharacterCardUI> _enemyCards = new();
        private readonly List<CharacterCardUI> _playerCards = new();

        public void Build(Transform parent)
        {
            Root = PanelBuilder.CreateContainer("BattleGrid", parent);

            // Enemy grid (top half)
            _enemyGrid = PanelBuilder.CreateContainer("EnemyGrid", Root);
            PanelBuilder.SetAnchored(_enemyGrid, 0, 0.52f, 1, 1);

            // Divider line
            GameObject dividerGo = new GameObject("Divider");
            _divider = dividerGo.AddComponent<RectTransform>();
            _divider.SetParent(Root, false);
            PanelBuilder.SetAnchored(_divider, 0.05f, 0.495f, 0.95f, 0.505f);
            Image divImg = dividerGo.AddComponent<Image>();
            divImg.color = UIStyleConfig.TextDimmed;

            // Player grid (bottom half)
            _playerGrid = PanelBuilder.CreateContainer("PlayerGrid", Root);
            PanelBuilder.SetAnchored(_playerGrid, 0, 0, 1, 0.48f);
        }

        public void SetEnemies(List<BattleCharacter> enemies)
        {
            ClearCards(_enemyCards);
            PlaceCharacters(enemies, _enemyGrid, _enemyCards, invertRows: true);
        }

        public void SetPlayers(List<BattleCharacter> players)
        {
            ClearCards(_playerCards);
            PlaceCharacters(players, _playerGrid, _playerCards, invertRows: false);
        }

        private void PlaceCharacters(List<BattleCharacter> characters, RectTransform grid,
            List<CharacterCardUI> cards, bool invertRows)
        {
            float spacing = 2f;

            foreach (BattleCharacter character in characters)
            {
                CharacterCardUI card = new CharacterCardUI();
                card.Build(grid, character);

                float xMin, xMax, yMin, yMax;
                GetCellBounds(character.Row, character.Column, characters.Count, invertRows,
                    out xMin, out yMin, out xMax, out yMax);

                PanelBuilder.SetAnchored(card.Root, xMin, yMin, xMax, yMax,
                    spacing, spacing, -spacing, -spacing);

                cards.Add(card);
            }
        }

        private void GetCellBounds(GridRow row, GridColumn column, int totalCount, bool invertRows,
            out float xMin, out float yMin, out float xMax, out float yMax)
        {
            // For players: Front = top of their grid, Back = bottom
            // For enemies (invertRows=true): Front = bottom of their grid (near divider), Back = top
            bool isTopRow;
            if (invertRows)
                isTopRow = row == GridRow.Back;
            else
                isTopRow = row == GridRow.Front;

            yMin = isTopRow ? 0.5f : 0f;
            yMax = isTopRow ? 1f : 0.5f;

            if (totalCount <= 1)
            {
                xMin = 0.25f;
                xMax = 0.75f;
                yMin = 0.25f;
                yMax = 0.75f;
            }
            else if (totalCount == 2)
            {
                xMin = column == GridColumn.Left ? 0f : 0.5f;
                xMax = column == GridColumn.Left ? 0.5f : 1f;
                yMin = 0.25f;
                yMax = 0.75f;
            }
            else
            {
                xMin = column == GridColumn.Left ? 0f : 0.5f;
                xMax = column == GridColumn.Left ? 0.5f : 1f;
            }
        }

        public void RefreshAll()
        {
            foreach (CharacterCardUI card in _enemyCards) card.Refresh();
            foreach (CharacterCardUI card in _playerCards) card.Refresh();
        }

        private void ClearCards(List<CharacterCardUI> cards)
        {
            foreach (CharacterCardUI card in cards)
            {
                Object.Destroy(card.Root.gameObject);
            }
            cards.Clear();
        }
    }
}
