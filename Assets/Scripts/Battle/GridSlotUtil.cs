using System.Collections.Generic;

namespace PixelWarriors
{
    public static class GridSlotUtil
    {
        private static readonly (GridRow Row, GridColumn Col)[] AllSlots =
        {
            (GridRow.Front, GridColumn.Left),
            (GridRow.Front, GridColumn.Right),
            (GridRow.Back, GridColumn.Left),
            (GridRow.Back, GridColumn.Right)
        };

        public static List<(GridRow Row, GridColumn Col)> GetEmptySlots(List<BattleCharacter> existing)
        {
            List<(GridRow Row, GridColumn Col)> empty = new();
            foreach ((GridRow row, GridColumn col) in AllSlots)
            {
                bool occupied = false;
                foreach (BattleCharacter bc in existing)
                {
                    if (bc.IsAlive && bc.Row == row && bc.Column == col)
                    {
                        occupied = true;
                        break;
                    }
                }
                if (!occupied)
                    empty.Add((row, col));
            }
            return empty;
        }

        public static List<BattleCharacter> PlaceCharacters(
            List<CharacterData> characterData, TeamSide side,
            List<(GridRow Row, GridColumn Col)> slots = null)
        {
            slots ??= new List<(GridRow, GridColumn)>
            {
                (GridRow.Front, GridColumn.Left),
                (GridRow.Front, GridColumn.Right),
                (GridRow.Back, GridColumn.Left),
                (GridRow.Back, GridColumn.Right)
            };

            List<BattleCharacter> characters = new();
            for (int i = 0; i < characterData.Count && i < slots.Count; i++)
            {
                characters.Add(new BattleCharacter(
                    characterData[i], side, slots[i].Row, slots[i].Col));
            }
            return characters;
        }
    }
}
