using System.Collections.Generic;

namespace PixelWarriors
{
    public class RunData
    {
        public int CurrentAct;
        public int CurrentFloor;
        public int Gold;

        public List<CharacterData> Party;
        public List<CharacterData> Fallen;
        public List<EquipmentData> Inventory;
        public List<ConsumableStack> Consumables;
        public HashSet<string> DroppedUniques;
        public HashSet<string> SeenEvents;

        public RoomType? CurrentRoom;
        public RoomType? PreviousRoom;

        public int TotalBattles;
        public int TotalKills;

        public RunData()
        {
            CurrentAct = 1;
            CurrentFloor = 1;
            Gold = RunConfig.StartingGold;
            Party = new List<CharacterData>();
            Fallen = new List<CharacterData>();
            Inventory = new List<EquipmentData>();
            Consumables = new List<ConsumableStack>();
            DroppedUniques = new HashSet<string>();
            SeenEvents = new HashSet<string>();
        }

        public int TotalFloor => (CurrentAct - 1) * RunConfig.FloorsPerAct + CurrentFloor;

        public void AdvanceFloor()
        {
            CurrentFloor++;
            if (CurrentFloor > RunConfig.FloorsPerAct)
            {
                CurrentFloor = 1;
                CurrentAct++;
            }
        }

        public bool IsRunComplete => CurrentAct > RunConfig.ActCount;
        public bool IsBossFloor => CurrentFloor == RunConfig.FloorsPerAct;

        public void AddConsumable(string id, int qty = 1)
        {
            ConsumableStack existing = Consumables.Find(c => c.ConsumableId == id);
            if (existing != null)
                existing.Quantity += qty;
            else
                Consumables.Add(new ConsumableStack(id, qty));
        }

        public void RemoveConsumable(string id, int qty = 1)
        {
            ConsumableStack existing = Consumables.Find(c => c.ConsumableId == id);
            if (existing == null) return;
            existing.Quantity -= qty;
            if (existing.Quantity <= 0)
                Consumables.Remove(existing);
        }

        public int GetConsumableCount(string id)
        {
            ConsumableStack existing = Consumables.Find(c => c.ConsumableId == id);
            return existing?.Quantity ?? 0;
        }
    }
}
