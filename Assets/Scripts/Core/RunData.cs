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
    }
}
