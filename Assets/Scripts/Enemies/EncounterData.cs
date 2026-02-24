using System.Collections.Generic;

namespace PixelWarriors
{
    public class EncounterData
    {
        public List<CharacterData> InitialEnemies = new();
        public List<ReinforcementWave> Waves = new();
    }

    public class ReinforcementWave
    {
        public List<CharacterData> Enemies = new();
        public ReinforcementTrigger Trigger;
        public int TriggerValue;
        public string AnnouncementText;
    }
}
