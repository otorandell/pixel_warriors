using System;
using System.Collections.Generic;

namespace PixelWarriors
{
    [Serializable]
    public class SaveData
    {
        public int CurrentAct;
        public int CurrentFloor;
        public int Gold;

        public List<CharacterData> Party;
        public List<CharacterData> Fallen;
        public List<EquipmentData> Inventory;
        public List<ConsumableStack> Consumables;
        public List<string> DroppedUniques;
        public List<string> SeenEvents;

        public int CurrentRoomInt;
        public int PreviousRoomInt;

        public int TotalBattles;
        public int TotalKills;

        public static SaveData FromRunData(RunData run)
        {
            SaveData save = new SaveData
            {
                CurrentAct = run.CurrentAct,
                CurrentFloor = run.CurrentFloor,
                Gold = run.Gold,
                Party = run.Party,
                Fallen = run.Fallen,
                Inventory = run.Inventory,
                Consumables = run.Consumables,
                DroppedUniques = new List<string>(run.DroppedUniques),
                SeenEvents = new List<string>(run.SeenEvents),
                CurrentRoomInt = run.CurrentRoom.HasValue ? (int)run.CurrentRoom.Value : -1,
                PreviousRoomInt = run.PreviousRoom.HasValue ? (int)run.PreviousRoom.Value : -1,
                TotalBattles = run.TotalBattles,
                TotalKills = run.TotalKills
            };

            return save;
        }

        public RunData ToRunData()
        {
            RunData run = new RunData
            {
                CurrentAct = CurrentAct,
                CurrentFloor = CurrentFloor,
                Gold = Gold,
                Party = Party ?? new List<CharacterData>(),
                Fallen = Fallen ?? new List<CharacterData>(),
                Inventory = Inventory ?? new List<EquipmentData>(),
                Consumables = Consumables ?? new List<ConsumableStack>(),
                DroppedUniques = new HashSet<string>(DroppedUniques ?? new List<string>()),
                SeenEvents = new HashSet<string>(SeenEvents ?? new List<string>()),
                CurrentRoom = CurrentRoomInt >= 0 ? (RoomType)CurrentRoomInt : null,
                PreviousRoom = PreviousRoomInt >= 0 ? (RoomType)PreviousRoomInt : null,
                TotalBattles = TotalBattles,
                TotalKills = TotalKills
            };

            // JsonUtility deserializes null array elements as default instances.
            // Restore null equipment slots by checking for empty Name.
            RestoreNullEquipment(run.Party);
            RestoreNullEquipment(run.Fallen);

            return run;
        }

        private static void RestoreNullEquipment(List<CharacterData> characters)
        {
            if (characters == null) return;

            foreach (CharacterData c in characters)
            {
                if (c.Equipment == null) continue;

                for (int i = 0; i < c.Equipment.Length; i++)
                {
                    if (c.Equipment[i] != null && string.IsNullOrEmpty(c.Equipment[i].Name))
                        c.Equipment[i] = null;
                }
            }
        }
    }
}
